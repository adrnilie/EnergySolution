using Azure;
using Azure.Data.Tables;

namespace Common.TableStorage.Models;

public class AzureTableEntity : ITableEntity
{
    public AzureTableEntity()
    {
        
    }

    public AzureTableEntity(string partitionKey, string rowKey)
    {
        PartitionKey = partitionKey;
        RowKey = rowKey;
    }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}