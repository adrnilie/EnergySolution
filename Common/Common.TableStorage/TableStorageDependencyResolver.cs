using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.TableStorage;

public static class TableStorageDependencyResolver
{
    public static IServiceCollection AddTableStorageRepository<TRepositoryType, TRepository>(
        this IServiceCollection services, string tableName, string connectionStringSettingKey)
        where TRepository : TRepositoryType
        where TRepositoryType : class
        => services
            .AddSingleton<TRepositoryType>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var tableStorageConfiguration = new TableStorageConfiguration(configuration[connectionStringSettingKey]!);
                return ActivatorUtilities.CreateInstance<TRepository>(sp, tableStorageConfiguration, tableName);
            });
}