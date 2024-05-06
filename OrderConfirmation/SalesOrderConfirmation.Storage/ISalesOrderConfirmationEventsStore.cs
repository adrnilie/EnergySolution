using SalesOrderConfirmation.Storage.Entities;

namespace SalesOrderConfirmation.Storage;

internal interface ISalesOrderConfirmationEventsStore
{
    Task<IEnumerable<SalesOrderConfirmationEventTableEntity>> GetEventStreamAsync(string streamId, string tenantId, CancellationToken cancellationToken = new());
    Task AddOrUpdateAsync(IEnumerable<SalesOrderConfirmationEventTableEntity> entities, CancellationToken cancellationToken = new());
}