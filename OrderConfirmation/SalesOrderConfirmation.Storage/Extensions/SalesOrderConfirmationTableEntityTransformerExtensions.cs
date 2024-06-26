﻿using Newtonsoft.Json;
using SalesOrderConfirmation.Aggregate.Contracts;
using SalesOrderConfirmation.Storage.Entities;

namespace SalesOrderConfirmation.Storage.Extensions;

internal static class SalesOrderConfirmationTableEntityTransformerExtensions
{
    internal static IEnumerable<SalesOrderConfirmationEventTableEntity> ToEntities(this IReadOnlyList<IEvent> events)
        => events.Select(e => e.ToEntity());

    internal static SalesOrderConfirmationEventTableEntity ToEntity(this IEvent @event)
        => new()
        {
            PartitionKey = $"{@event.StreamId.Value}",
            RowKey = $"{@event.TenantId.Value}.{@event.Version.ToString().PadLeft(4, '0')}",
            StreamId = @event.StreamId,
            TenantId = @event.TenantId,
            Version = @event.Version,
            EventName = @event.GetType().Name,
            EventNamespace = @event.GetType().Namespace ?? string.Empty,
            Type = @event.Type,
            Body = JsonConvert.SerializeObject(@event),
            CreationDate = DateTimeOffset.UtcNow,
        };
}