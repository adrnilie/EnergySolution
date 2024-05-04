using SalesOrderConfirmation.Aggregate.Contracts;
using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;

namespace SalesOrderConfirmation.Aggregate;

internal sealed class SalesOrderInitializer : ISalesOrderInitializer
{
    public ISalesOrder Create(SalesOrderStreamId streamId, SalesOrderTenantId tenantId)
        => SalesOrder.Create(streamId, tenantId);
}