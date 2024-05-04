using SalesOrderConfirmation.Storage.Entities;

namespace SalesOrderConfirmation.Storage;

internal interface ISalesOrderConfirmationEventsStore
{
    Task<IEnumerable<SalesOrderConfirmationTableEntity>> GetEventStreamAsync(string streamId, string tenantId, CancellationToken cancellationToken = new());
    Task AddOrUpdateAsync(IEnumerable<SalesOrderConfirmationTableEntity> entities, CancellationToken cancellationToken = new());
}