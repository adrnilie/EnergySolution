using SalesOrderConfirmation.Aggregate.Contracts.Abstractions;
using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;

namespace SalesOrderConfirmation.Messages;

public sealed record SalesOrderConfirmed(SalesOrderStreamId StreamId, SalesOrderTenantId TenantId) : SalesOrderEvent(StreamId, TenantId);
