using SalesOrderConfirmation.Aggregate.Contracts.Commands;
using SalesOrderConfirmation.Messages;

namespace SalesOrderConfirmation.Aggregate.Extensions;

internal static class EventsTransformationExtensions
{
    internal static SalesOrderConfirmed Adapt(this ConfirmOrderCommand command) 
        => new(command.StreamId, command.TenantId);
}