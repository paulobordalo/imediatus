using imediatus.Framework.Core.Storage;
using imediatus.Framework.Core.Storage.Azure;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace imediatus.Framework.Infrastructure.Storage.Azure;

internal static class Extension
{
    internal static IServiceCollection ConfigureAzureStorage(this IServiceCollection services, IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddTransient<IStorageService, AzureStorageService>();
        services.AddTransient<IStorageAzureService, AzureStorageService>();

        services.AddAzureClients(builder =>
        {
            // Add a storage account client
            builder.AddBlobServiceClient(config.GetConnectionString("AzureBlobStorage"));

            // Set up any default settings
            builder.ConfigureDefaults(config.GetSection("AzureDefaults"));
        });

        return services;
    }
}
