using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;

namespace SalesOrderConfirmation.Aggregate.Contracts;

public interface ISalesOrderConfirmationRepository
{
    ISalesOrder LoadOrCreate(SalesOrderStreamId streamId, SalesOrderTenantId tenantId, IReadOnlyList<IEvent>? events = null);
}