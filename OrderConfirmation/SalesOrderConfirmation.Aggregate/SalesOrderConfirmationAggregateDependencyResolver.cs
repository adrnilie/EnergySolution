using Microsoft.Extensions.DependencyInjection;
using SalesOrderConfirmation.Aggregate.Contracts;

namespace SalesOrderConfirmation.Aggregate;

public static class SalesOrderConfirmationAggregateDependencyResolver
{
    public static IServiceCollection AddSalesOrderConfirmationAggregate(this IServiceCollection services)
        => services
            .AddScoped<ISalesOrderInitializer, SalesOrderInitializer>();
}