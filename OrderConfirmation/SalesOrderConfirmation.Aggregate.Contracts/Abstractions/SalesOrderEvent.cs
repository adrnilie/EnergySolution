using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;

namespace SalesOrderConfirmation.Aggregate.Contracts.Abstractions;

public abstract record SalesOrderEvent(SalesOrderStreamId StreamId, SalesOrderTenantId TenantId) : IEvent
{
    public int Version { get; private set; }
    public string Type => GetType().Name;

    public void SetVersion(int version) => Version = version;
}