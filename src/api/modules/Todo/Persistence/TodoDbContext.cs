using Finbuckle.MultiTenant.Abstractions;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Infrastructure.Persistence;
using imediatus.Framework.Infrastructure.Tenant;
using imediatus.Shared.Constants;
using imediatus.WebApi.Todo.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace imediatus.WebApi.Todo.Persistence;
public sealed class TodoDbContext(IMultiTenantContextAccessor<ImediatusTenantInfo> multiTenantContextAccessor, DbContextOptions<TodoDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings) : BaseDbContext(multiTenantContextAccessor, options, publisher, settings)
{
    public DbSet<TodoItem> Todos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoDbContext).Assembly);
        _ = modelBuilder.HasDefaultSchema(SchemaNames.Todo);
    }
}
