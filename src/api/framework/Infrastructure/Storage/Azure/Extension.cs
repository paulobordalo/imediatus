using Azure.Identity;
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
            // Add a KeyVault client
            // builder.AddSecretClient(config.GetSection("KeyVault"));

            // Add a storage account client
            // builder.AddBlobServiceClient(config.GetSection("Storage"));
            builder.AddBlobServiceClient(config.GetConnectionString("AzureBlobStorage"));

            // Use DefaultAzureCredential by default
            //builder.UseCredential(new DefaultAzureCredential());

            // Set up any default settings
            builder.ConfigureDefaults(config.GetSection("AzureDefaults"));
        });

        return services;
    }
}
