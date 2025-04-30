using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using imediatus.Framework.Core.Domain.Contracts;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Infrastructure.Tenant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace imediatus.Framework.Infrastructure.Persistence;
public class BaseDbContext(IMultiTenantContextAccessor<ImediatusTenantInfo> multiTenantContextAccessor,
    DbContextOptions options,
    IPublisher publisher,
    IOptions<DatabaseOptions> settings)
    : MultiTenantDbContext(multiTenantContextAccessor, options)
{
    private readonly IPublisher _publisher = publisher;
    private readonly DatabaseOptions _settings = settings.Value;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // QueryFilters need to be applied before base.OnModelCreating
        _ = modelBuilder.AppendGlobalQueryFilter<ISoftDeletable>(s => !s.Deleted.HasValue);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _ = optionsBuilder.EnableSensitiveDataLogging();

        if (!string.IsNullOrWhiteSpace(multiTenantContextAccessor?.MultiTenantContext.TenantInfo?.ConnectionString))
        {
            _ = optionsBuilder.ConfigureDatabase(_settings.Provider, multiTenantContextAccessor.MultiTenantContext.TenantInfo.ConnectionString!);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.TenantNotSetMode = TenantNotSetMode.Overwrite;
        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await PublishDomainEventsAsync().ConfigureAwait(false);
        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        List<Core.Domain.Events.DomainEvent> domainEvents = ChangeTracker.Entries<IEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .SelectMany(e =>
            {
                List<Core.Domain.Events.DomainEvent> domainEvents = [.. e.DomainEvents];
                e.DomainEvents.Clear();
                return domainEvents;
            })
            .ToList();

        foreach (Core.Domain.Events.DomainEvent domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent).ConfigureAwait(false);
        }
    }
}
