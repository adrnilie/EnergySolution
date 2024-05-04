using Common.TableStorage;
using Common.TableStorage.Contracts;
using SalesOrderConfirmation.Storage.Entities;

namespace SalesOrderConfirmation.Storage;

internal sealed class SalesOrderConfirmationEventsStore : AzureTableStorageRepository<SalesOrderConfirmationTableEntity>, ISalesOrderConfirmationEventsStore
{
    public SalesOrderConfirmationEventsStore(TableStorageConfiguration configuration, string tableName) : base(configuration, tableName)
    {
    }

    public async Task<IEnumerable<SalesOrderConfirmationTableEntity>> GetEventStreamAsync(string streamId, string tenantId, CancellationToken cancellationToken = new())
    {
        var events = new List<SalesOrderConfirmationTableEntity>();
        SegmentedResult<SalesOrderConfirmationTableEntity> page = new();
        do
        {
            page = await StartsWithPartialRowkey(streamId, tenantId, 100, page.ContinuationToken!, cancellationToken);
            events.AddRange(page.Results);
        } while (!string.IsNullOrWhiteSpace(page.ContinuationToken));

        return events.OrderBy(x => x.Version);
    }

    public async Task AddOrUpdateAsync(IEnumerable<SalesOrderConfirmationTableEntity> entities, CancellationToken cancellationToken = new())
        => await AddOrUpdate(entities, cancellationToken);
}