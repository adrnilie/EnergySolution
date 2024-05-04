using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;

namespace SalesOrderConfirmation.Aggregate.Contracts;

public interface ICommand
{
    SalesOrderStreamId StreamId { get; }
    SalesOrderTenantId TenantId { get; }
}