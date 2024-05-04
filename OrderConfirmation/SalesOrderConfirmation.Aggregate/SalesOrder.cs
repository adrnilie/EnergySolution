using SalesOrderConfirmation.Aggregate.Contracts;
using SalesOrderConfirmation.Aggregate.Contracts.Abstractions;
using SalesOrderConfirmation.Aggregate.Contracts.Commands;
using SalesOrderConfirmation.Aggregate.Contracts.Exceptions;
using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;
using SalesOrderConfirmation.Aggregate.Extensions;
using SalesOrderConfirmation.Messages;

namespace SalesOrderConfirmation.Aggregate;

internal class SalesOrder : ISalesOrder
{
    private IList<IEvent> _pendingEvents = new List<IEvent>();

    // State
    private bool _salesOrderConfirmed;
    private int _currentVersion;

    public SalesOrderStreamId StreamId { get; private set; }
    public SalesOrderTenantId TenantId { get; private set; }

    protected internal SalesOrder(SalesOrderStreamId streamId, SalesOrderTenantId tenantId)
    {
        StreamId = streamId;
        TenantId = tenantId;
    }

    public static SalesOrder Create(SalesOrderStreamId streamId, SalesOrderTenantId tenantId) => new(streamId, tenantId);

    public void RestoreFrom(IReadOnlyList<IEvent> events)
    {
        if (!events.Any())
        {
            return;
        }

        EnsureEventsRepresentSameOrder(events);

        var orderedEvents = events
            .OrderByDescending(x => x.Version);

        _currentVersion = orderedEvents.First().Version;

        foreach (var @event in events)
        {
            Apply(@event);
        }
    }

    public void ConfirmOrder(ConfirmOrderCommand command)
    {
        EnsureSameOrder(command);
        Emit(command.Adapt());
    }

    public IReadOnlyList<IEvent> Commit()
    {
        var pendingEvents = _pendingEvents.ToList();
        _pendingEvents = new List<IEvent>();
        return pendingEvents;
    }

    private void Emit(SalesOrderEvent @event)
    {
        @event.SetVersion(++_currentVersion);

        Apply(@event);
        _pendingEvents.Add(@event);
    }

    private void Apply(IEvent @event)
    {
        switch (@event)
        {
            case SalesOrderConfirmed salesOrderConfirmed:
                Apply(salesOrderConfirmed);
                break;
            default:
                throw new InvalidOperationException("Unsupported event.");
        }
    }

    private void Apply(SalesOrderConfirmed _)
    {
        if (_salesOrderConfirmed)
        {
            return;
        }

        _salesOrderConfirmed = true;
    }

    private void EnsureSameOrder(ICommand command)
    {
        if (!command.StreamId.Equals(StreamId) || !command.TenantId.Equals(TenantId))
        {
            throw new InvalidSalesOrderException("Invalid order. You are trying to update the aggregate with a different order details.");
        }
    }

    private static void EnsureEventsRepresentSameOrder(IReadOnlyList<IEvent> events)
    {
        var foundDifferences = events
            .Select(x => new
            {
                x.StreamId,
                x.TenantId
            }).Distinct()
            .Skip(1)
            .Any();

        if (foundDifferences)
        {
            throw new InvalidOperationException("We found different orders in the event stream provided.");
        }
    }
}

