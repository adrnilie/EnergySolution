using SalesOrderConfirmation.Aggregate.Contracts;
using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;
using System.Runtime.InteropServices.JavaScript;

namespace SalesOrderConfirmation.Aggregate;

internal sealed class SalesOrderConfirmationRepository : ISalesOrderConfirmationRepository
{
    public ISalesOrder LoadOrCreate(SalesOrderStreamId streamId, SalesOrderTenantId tenantId, IReadOnlyList<IEvent>? events = null)
    {
        if (events is null || !events.Any())
        {
            return SalesOrder.Create(streamId, tenantId);
        }

        var salesOrder = SalesOrder.Create(streamId, tenantId);
        salesOrder.RestoreFrom(events);

        return salesOrder;
    }
}