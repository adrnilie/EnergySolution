using SalesOrderConfirmation.Aggregate.Contracts;
using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;

namespace SalesOrderConfirmation.Storage.Contracts;

public interface ISalesOrderConfirmationStorage
{
    Task<ISalesOrder> LoadOrCreateAsync(SalesOrderStreamId streamId, SalesOrderTenantId tenantId, CancellationToken cancellationToken = new());
    Task SaveChangesAsync(ISalesOrder salesOrder, CancellationToken cancellationToken = new());
}
