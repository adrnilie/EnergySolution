using SalesOrderConfirmation.Aggregate.Contracts.Commands;
using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;

namespace SalesOrderConfirmation.Aggregate.Contracts;

public interface ISalesOrder
{
    SalesOrderStreamId StreamId { get; }
    SalesOrderTenantId TenantId { get; }

    void RestoreFrom(IReadOnlyList<IEvent> events);
    void ConfirmOrder(ConfirmOrderCommand command);
    IReadOnlyList<IEvent> Commit();
}