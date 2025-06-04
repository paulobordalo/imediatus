using Finbuckle.MultiTenant.Abstractions;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Infrastructure.Persistence;
using imediatus.Framework.Infrastructure.Tenant;
using imediatus.Shared.Constants;
using imediatus.WebApi.Workspace.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace imediatus.WebApi.Workspace.Infrastructure.Persistence;

public sealed class WorkspaceDbContext(IMultiTenantContextAccessor<ImediatusTenantInfo> multiTenantContextAccessor, DbContextOptions<WorkspaceDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings) : BaseDbContext(multiTenantContextAccessor, options, publisher, settings)
{
    public DbSet<CostCenter> CostCenters { get; set; } = null!;
    public DbSet<Portfolio> Portfolios { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkspaceDbContext).Assembly);
        _ = modelBuilder.HasDefaultSchema(SchemaNames.Workspace);
    }
}
