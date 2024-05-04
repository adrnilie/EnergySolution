using Common.TableStorage;
using Microsoft.Extensions.DependencyInjection;
using SalesOrderConfirmation.Aggregate;
using SalesOrderConfirmation.Storage.Contracts;

namespace SalesOrderConfirmation.Storage;

public static class SalesOrderConfirmationStorageDependencyResolver
{
    public static IServiceCollection AddSalesOrderConfirmationStorage(this IServiceCollection services)
    {
        return services
            .AddSalesOrderConfirmationAggregate()
            .AddTableStorageRepository<ISalesOrderConfirmationEventsStore, SalesOrderConfirmationEventsStore>("salesorderconfirmation", "Azure.TableStorage")
            .AddScoped<ISalesOrderConfirmationStorage, SalesOrderConfirmationStorage>();
    }
}