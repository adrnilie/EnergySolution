using Azure;
using Azure.Data.Tables;
using Common.TableStorage.Contracts;
using Common.TableStorage.Models;
using System.Linq.Expressions;

namespace Common.TableStorage;

public class AzureTableStorageRepository<T> where T : AzureTableEntity, new()
{
    protected TableClient TableClient { get; }

    protected internal AzureTableStorageRepository(TableStorageConfiguration configuration, string tableName)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        TableClient = CreateTableServiceClient(configuration, tableName);
    }

    protected internal async Task<T> Get(string partitionKeyValue, string rowKeyValue, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await TableClient.GetEntityIfExistsAsync<T>(partitionKeyValue, rowKeyValue, cancellationToken: cancellationToken).ConfigureAwait(false);

            return response.HasValue ? response.Value : null;
        }
        catch (Exception ex)
        {
            throw new TableStorageException($"TableStorageRepository.Get partitionKeyValue={partitionKeyValue} rowKeyValue={rowKeyValue} failed", ex);
        }
    }

    protected internal async Task<IEnumerable<T>> GetAll(string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        return await Query(e => e.PartitionKey.Equals(partitionKeyValue), cancellationToken: cancellationToken);
    }

    protected internal async Task<SegmentedResult<T>> StartsWithPartialRowkey(string partitionKey, string partialRowKey, int limit = 100, string continuationToken = default, CancellationToken cancellationToken = default)
    {
        var length = partialRowKey.Length - 1;
        var nextChar = partialRowKey[length] + 1;
        var startWithEnd = partialRowKey.Substring(0, length) + (char)nextChar;

        var entities = TableClient.QueryAsync<T>(TableClient.CreateQueryFilter($"PartitionKey eq {partitionKey} and RowKey ge {partialRowKey} and RowKey lt {startWithEnd}"), limit, cancellationToken: cancellationToken);
        var page = await GetNextPage(entities, continuationToken);

        return new SegmentedResult<T>
        {
            Results = page.Values,
            ContinuationToken = page.ContinuationToken
        };
    }

    protected async Task Add(T tableStorageEntity, CancellationToken cancellationToken = default)
    {
        ValidateTableStorageEntity(tableStorageEntity);
        try
        {
            await TableClient.AddEntityAsync(tableStorageEntity, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new TableStorageException($"TableStorageRepository.Add type={typeof(T)} partitionKeyValue={tableStorageEntity.PartitionKey} rowKeyValue={tableStorageEntity.RowKey} failed", ex);
        }
    }

    protected internal async Task AddOrUpdate(T tableStorageEntity, CancellationToken cancellationToken = default)
    {
        ValidateTableStorageEntity(tableStorageEntity);

        try
        {
            await TableClient.UpsertEntityAsync(tableStorageEntity, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new TableStorageException($"TableStorageRepository.Upsert type={typeof(T)} partitionKeyValue={tableStorageEntity.PartitionKey} rowKeyValue={tableStorageEntity.RowKey} failed", ex);
        }
    }

    protected internal async Task AddOrUpdate(IEnumerable<T> tableStorageEntities, CancellationToken cancellationToken = default)
    {
        ValidateTableStorageEntities(tableStorageEntities);

        try
        {
            var tasks = tableStorageEntities
                    .GroupBy(x => x.PartitionKey)
                    .Select(UpsertByPartition);

            await Task.WhenAll(tasks).ConfigureAwait(false);

        }
        catch (Exception ex)
        {
            throw new TableStorageException($"TableStorageRepository.AddOrUpdate type={typeof(T)} partitionKeyValues - rowKeyValue={string.Join(", ", tableStorageEntities.Select(x => x.PartitionKey + " - " + x.RowKey))} failed", ex);
        }

        async Task UpsertByPartition(IEnumerable<T> entities)
        {
            List<TableTransactionAction> batch = new();
            foreach (T entity in entities)
            {
                batch.Add(new TableTransactionAction(TableTransactionActionType.UpdateMerge, entity));
            }

            await TableClient.SubmitTransactionAsync(batch, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    protected internal async Task DeleteAll(string partitionKeyValue, CancellationToken cancellationToken)
    {
        var entities = await GetAll(partitionKeyValue, cancellationToken);

        if (!entities.Any())
            return;

        List<TableTransactionAction> batch = new() { };
        foreach (T entity in entities)
        {
            batch.Add(new TableTransactionAction(TableTransactionActionType.Delete, entity));
        }

        try
        {
            await TableClient.SubmitTransactionAsync(batch, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new TableStorageException($"TableStorageRepository.DeleteAll type={typeof(T)} partitionKeyValue={partitionKeyValue} failed", ex);
        }
    }

    protected internal async Task Delete(T entity, CancellationToken cancellationToken = default)
    {
        await Delete(entity.PartitionKey, entity.RowKey, entity.ETag, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    protected internal async Task Delete(string partitionKey, string rowKey, ETag eTag = default, CancellationToken cancellationToken = default)
    {
        await TableClient.DeleteEntityAsync(partitionKey, rowKey, eTag, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    protected internal async Task DeleteBatch(IEnumerable<AzureTableEntity> entities, CancellationToken cancellationToken = default)
    {
        var batch = entities.Select(e => new TableTransactionAction(TableTransactionActionType.Delete, e));

        if (!batch.Any())
            return;

        await TableClient.SubmitTransactionAsync(batch, cancellationToken).ConfigureAwait(false);
    }

    protected internal async Task<IEnumerable<T>> Query(Expression<Func<T, bool>> query, IEnumerable<string> select = null, CancellationToken cancellationToken = new())
    {
        AsyncPageable<T> pageableResponse = TableClient.QueryAsync(query, select: select, cancellationToken: cancellationToken);

        List<T> result = new();

        await foreach (T item in pageableResponse)
        {
            result.Add(item);
        }

        return result;
    }

    protected internal async Task<SegmentedResult<T>> GetSegmentedResult(Expression<Func<T, bool>> query, int limit, string continuationToken = default)
    {
        var results = TableClient.QueryAsync<T>(query, limit);

        var page = await GetNextPage(results, continuationToken).ConfigureAwait(false);

        return new SegmentedResult<T>
        {
            ContinuationToken = page.ContinuationToken,
            Results = page.Values
        };
    }

    //public async Task CleanUpOldEntries(string partitionKey, TimeSpan retentionPeriod, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var entities = TableClient.QueryAsync<AzureTableEntity>(e => e.PartitionKey.Equals(partitionKey) &&
    //        e.Timestamp < DateTimeProvider.UtcNowDateTimeOffset().Subtract(retentionPeriod), cancellationToken: cancellationToken);

    //        await DeleteBatch(entities, cancellationToken: cancellationToken).ConfigureAwait(false);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new PassException($"TableStorageRepository.DeleteBatch partitionKey: {partitionKey}, retentionPeriod: {retentionPeriod} failed", ex);
    //    }
    //}

    private static void ValidateTableStorageEntity(T tableStorageEntity)
    {
        if (tableStorageEntity == null)
        {
            throw new ArgumentNullException(nameof(tableStorageEntity));
        }
    }

    private static void ValidateTableStorageEntities(IEnumerable<T> tableStorageEntities)
    {
        if (tableStorageEntities == null)
        {
            throw new ArgumentNullException(nameof(tableStorageEntities));
        }
    }

    private async Task DeleteBatch(AsyncPageable<AzureTableEntity> entities, CancellationToken cancellationToken)
    {
        List<TableTransactionAction> batch = new();

        await foreach (AzureTableEntity entity in entities)
        {
            batch.Add(new TableTransactionAction(TableTransactionActionType.Delete, entity));
        }

        if (!batch.Any())
            return;

        await TableClient.SubmitTransactionAsync(batch, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Page<T>> GetNextPage(AsyncPageable<T> results, string continuationToken)
    {
        var pages = results.AsPages(continuationToken);
        var pageEnumerator = pages.GetAsyncEnumerator();
        await pageEnumerator.MoveNextAsync().ConfigureAwait(false);

        return pageEnumerator.Current;
    }

    private TableClient CreateTableServiceClient(TableStorageConfiguration configuration, string tableName)
    {
        try
        {
            var tableServiceClient = new TableServiceClient(configuration.ConnectionString).GetTableClient(tableName);
            tableServiceClient.CreateIfNotExists();
            return tableServiceClient;
        }
        catch (Exception ex)
        {
            throw new InvalidTableStorageAccountException("Invalid storage account configuration provided.", ex);
        }
    }
}