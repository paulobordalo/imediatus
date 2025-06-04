using Finbuckle.MultiTenant.Abstractions;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Infrastructure.Persistence;
using imediatus.Framework.Infrastructure.Tenant;
using imediatus.Shared.Constants;
using imediatus.WebApi.Catalog.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace imediatus.WebApi.Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContext(IMultiTenantContextAccessor<ImediatusTenantInfo> multiTenantContextAccessor, DbContextOptions<CatalogDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings) : BaseDbContext(multiTenantContextAccessor, options, publisher, settings)
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        _ = modelBuilder.HasDefaultSchema(SchemaNames.Catalog);
    }
}
