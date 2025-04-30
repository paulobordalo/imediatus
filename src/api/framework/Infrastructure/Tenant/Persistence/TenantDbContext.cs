using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using Microsoft.EntityFrameworkCore;

namespace imediatus.Framework.Infrastructure.Tenant.Persistence;
public class TenantDbContext : EFCoreStoreDbContext<ImediatusTenantInfo>
{
    public const string Schema = "tenant";
    public TenantDbContext(DbContextOptions<TenantDbContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ImediatusTenantInfo>().ToTable("Tenants", Schema);
    }
}
