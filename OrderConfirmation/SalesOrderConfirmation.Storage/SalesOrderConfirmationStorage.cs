using Newtonsoft.Json;
using SalesOrderConfirmation.Aggregate.Contracts;
using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;
using SalesOrderConfirmation.Messages;
using SalesOrderConfirmation.Storage.Entities;
using SalesOrderConfirmation.Storage.Extensions;
using System.Reflection;

namespace SalesOrderConfirmation.Storage;

internal sealed class SalesOrderConfirmationStorage : ISalesOrderConfirmationStorage
{
    private readonly ISalesOrderConfirmationEventsStore _salesOrderConfirmationEventsStore;
    private readonly ISalesOrderConfirmationRepository _salesOrderConfirmationRepository;

    public SalesOrderConfirmationStorage(
        ISalesOrderConfirmationEventsStore salesOrderConfirmationEventsStore, 
        ISalesOrderConfirmationRepository salesOrderConfirmationRepository)
    {
        _salesOrderConfirmationEventsStore = salesOrderConfirmationEventsStore;
        _salesOrderConfirmationRepository = salesOrderConfirmationRepository;
    }

    public async Task<ISalesOrder> LoadOrCreateAsync(SalesOrderStreamId streamId, SalesOrderTenantId tenantId, CancellationToken cancellationToken = new())
    {
        var eventEntities = await _salesOrderConfirmationEventsStore.GetEventStreamAsync(streamId, tenantId, cancellationToken);
        var events = DeserializeEvents(eventEntities.ToList());

        var salesOrder = _salesOrderConfirmationRepository.LoadOrCreate(streamId, tenantId, events);

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

    private static IReadOnlyList<IEvent> DeserializeEvents(List<SalesOrderConfirmationEventTableEntity> eventEntities)
    {
        var events = new List<IEvent>();

        if (!eventEntities.Any())
        {
            return Array.Empty<IEvent>();
        }

        var salesOrderConfirmationAssembly = Assembly.Load(SalesOrderConfirmationMessages.Instance.FullName!);

        foreach (var eventEntity in eventEntities)
        {
            var eventType = salesOrderConfirmationAssembly.GetType(eventEntity.Type);
            if (eventType is null)
            {
                continue;
            }

            try
            {
                var @event = JsonConvert.DeserializeObject(eventEntity.Body, eventType);
                if (@event is null)
                {
                    continue;
                }

                events.Add((IEvent)@event);
            }
            catch (Exception)
            {
                continue;
            }
        }

        return events;
    }
}