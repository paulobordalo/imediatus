using imediatus.Framework.Core.Persistence;
using imediatus.WebApi.Catalog.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Catalog.Infrastructure.Persistence;
internal sealed class CatalogDbInitializer(
    ILogger<CatalogDbInitializer> logger,
    CatalogDbContext context) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{Tenant}] applied database migrations for catalog module", context.TenantInfo!.Identifier);
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        const string name = "Keychron V6 QMK Custom Wired Mechanical Keyboard";
        const string description = "A full-size layout QMK/VIA custom mechanical keyboard";
        const decimal price = 79;
        Guid? brandId = null;
        if (await context.Products.FirstOrDefaultAsync(t => t.Name == name, cancellationToken).ConfigureAwait(false) is null)
        {
            Product product = Product.Create(name, description, price, brandId);
            _ = await context.Products.AddAsync(product, cancellationToken);
            _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{Tenant}] seeding default catalog data", context.TenantInfo!.Identifier);
        }
    }
}
