using SalesOrderConfirmation.Aggregate.Contracts;
using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;
using SalesOrderConfirmation.Messages;
using SalesOrderConfirmation.Storage.Contracts;
using SalesOrderConfirmation.Storage.Entities;
using SalesOrderConfirmation.Storage.Extensions;

namespace SalesOrderConfirmation.Storage;

internal sealed class SalesOrderConfirmationStorage : ISalesOrderConfirmationStorage
{
    private readonly ISalesOrderConfirmationEventsStore _salesOrderConfirmationEventsStore;
    private readonly ISalesOrderInitializer _salesOrderInitializer;

    public SalesOrderConfirmationStorage(ISalesOrderConfirmationEventsStore salesOrderConfirmationEventsStore, ISalesOrderInitializer salesOrderInitializer)
    {
        _salesOrderConfirmationEventsStore = salesOrderConfirmationEventsStore;
        _salesOrderInitializer = salesOrderInitializer;
    }

    public async Task<ISalesOrder> LoadOrCreateAsync(SalesOrderStreamId streamId, SalesOrderTenantId tenantId, CancellationToken cancellationToken = new())
    {
        var eventEntities = await _salesOrderConfirmationEventsStore.GetEventStreamAsync(streamId, tenantId, cancellationToken);
        var events = DeserializeEvents(eventEntities);

        var salesOrder = _salesOrderInitializer.Create(streamId, tenantId);
        salesOrder.RestoreFrom(events);

        return salesOrder;
    }

    public async Task SaveChangesAsync(ISalesOrder salesOrder, CancellationToken cancellationToken = new())
    {
        var events = salesOrder.Commit();
        if (!events.Any())
        {
            return;
        }

        var eventEntities = events.ToEntities();

        await _salesOrderConfirmationEventsStore.AddOrUpdateAsync(eventEntities, cancellationToken);
    }

    private static IReadOnlyList<IEvent> DeserializeEvents(IEnumerable<SalesOrderConfirmationTableEntity> eventEntities)
    {
        var events = new List<IEvent>();

        foreach (var eventEntity in eventEntities)
        {
            switch (eventEntity.Body)
            {
                case SalesOrderConfirmed salesOrderConfirmed:
                    events.Add(salesOrderConfirmed);
                    break;
                default:
                    continue;
            }
        }

        return events;
    }
}