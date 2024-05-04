using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;

namespace SalesOrderConfirmation.Aggregate.Contracts;

public interface ISalesOrderInitializer
{
    ISalesOrder Create(SalesOrderStreamId streamId, SalesOrderTenantId tenantId);
}