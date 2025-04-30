using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using imediatus.Framework.Core.Audit;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Infrastructure.Identity.RoleClaims;
using imediatus.Framework.Infrastructure.Identity.Roles;
using imediatus.Framework.Infrastructure.Identity.Users;
using imediatus.Framework.Infrastructure.Persistence;
using imediatus.Framework.Infrastructure.Tenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace imediatus.Framework.Infrastructure.Identity.Persistence;
public class IdentityDbContext : MultiTenantIdentityDbContext<ImediatusUser,
    ImediatusRole,
    string,
    IdentityUserClaim<string>,
    IdentityUserRole<string>,
    IdentityUserLogin<string>,
    ImediatusRoleClaim,
    IdentityUserToken<string>>
{
    private readonly DatabaseOptions _settings;
    private new ImediatusTenantInfo TenantInfo { get; set; }
    public IdentityDbContext(IMultiTenantContextAccessor<ImediatusTenantInfo> multiTenantContextAccessor, DbContextOptions<IdentityDbContext> options, IOptions<DatabaseOptions> settings) : base(multiTenantContextAccessor, options)
    {
        _settings = settings.Value;
        TenantInfo = multiTenantContextAccessor.MultiTenantContext.TenantInfo!;
    }

    public DbSet<AuditTrail> AuditTrails { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!string.IsNullOrWhiteSpace(TenantInfo?.ConnectionString))
        {
            optionsBuilder.ConfigureDatabase(_settings.Provider, TenantInfo.ConnectionString);
        }
    }
}
