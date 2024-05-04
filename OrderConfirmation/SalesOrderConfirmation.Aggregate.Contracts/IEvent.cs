using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;

namespace SalesOrderConfirmation.Aggregate.Contracts;

public interface IEvent
{
    SalesOrderStreamId StreamId { get; }
    SalesOrderTenantId TenantId { get; }
    int Version { get; }
}
