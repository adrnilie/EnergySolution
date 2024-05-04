using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;

namespace SalesOrderConfirmation.Aggregate.Contracts.Commands;

public readonly record struct ConfirmOrderCommand(
    SalesOrderStreamId StreamId, 
    SalesOrderTenantId TenantId) : ICommand;