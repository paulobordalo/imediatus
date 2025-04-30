using System.Collections.ObjectModel;
using imediatus.Framework.Core.Audit;
using imediatus.Framework.Core.Domain;
using imediatus.Framework.Core.Domain.Contracts;
using imediatus.Framework.Core.Identity.Users.Abstractions;
using imediatus.Framework.Infrastructure.Identity.Audit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace imediatus.Framework.Infrastructure.Persistence.Interceptors;
public class AuditInterceptor(ICurrentUser currentUser, TimeProvider timeProvider, IPublisher publisher) : SaveChangesInterceptor
{

    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        await PublishAuditTrailsAsync(eventData);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task PublishAuditTrailsAsync(DbContextEventData eventData)
    {
        if (eventData.Context == null)
        {
            return;
        }

        eventData.Context.ChangeTracker.DetectChanges();
        List<TrailDto> trails = [];
        DateTimeOffset utcNow = timeProvider.GetUtcNow();
        foreach (EntityEntry<IAuditable>? entry in eventData.Context.ChangeTracker.Entries<IAuditable>().Where(x => x.State is EntityState.Added or EntityState.Deleted or EntityState.Modified && x.Entity.GetType().GetCustomAttributes(typeof(IgnoreAuditTrailAttribute), false).Length == 0).ToList())
        {
            Guid userId = currentUser.GetUserId();
            TrailDto trail = new()
            {
                Id = Guid.NewGuid(),
                TableName = entry.Entity.GetType().Name,
                UserId = userId,
                DateTime = utcNow
            };

            foreach (PropertyEntry? property in entry.Properties.Where(p => p.Metadata.PropertyInfo?.GetCustomAttributes(typeof(IgnoreAuditTrailAttribute), false).Length == 0))
            {
                if (property.IsTemporary)
                {
                    continue;
                }
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    trail.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        trail.Type = TrailType.Create;
                        trail.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        trail.Type = TrailType.Delete;
                        trail.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            if (entry.Entity is ISoftDeletable && property.OriginalValue == null && property.CurrentValue != null)
                            {
                                trail.ModifiedProperties.Add(propertyName);
                                trail.Type = TrailType.Delete;
                                trail.OldValues[propertyName] = property.OriginalValue;
                                trail.NewValues[propertyName] = property.CurrentValue;
                            }
                            else if (property.OriginalValue?.Equals(property.CurrentValue) == false)
                            {
                                trail.ModifiedProperties.Add(propertyName);
                                trail.Type = TrailType.Update;
                                trail.OldValues[propertyName] = property.OriginalValue;
                                trail.NewValues[propertyName] = property.CurrentValue;
                            }
                            else
                            {
                                property.IsModified = false;
                            }
                        }
                        break;

                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    default:
                        break;
                }
            }

            trails.Add(trail);
        }
        if (trails.Count == 0)
        {
            return;
        }

        Collection<AuditTrail> auditTrails = [];
        foreach (TrailDto trail in trails)
        {
            auditTrails.Add(trail.ToAuditTrail());
        }
        await publisher.Publish(new AuditPublishedEvent(auditTrails));
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        foreach (EntityEntry<AuditableEntity> entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            DateTimeOffset utcNow = timeProvider.GetUtcNow();
            if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = currentUser.GetUserId();
                    entry.Entity.Created = utcNow;
                }
                entry.Entity.LastModifiedBy = currentUser.GetUserId();
                entry.Entity.LastModified = utcNow;
            }
            if (entry.State is EntityState.Deleted && entry.Entity is ISoftDeletable softDelete)
            {
                softDelete.DeletedBy = currentUser.GetUserId();
                softDelete.Deleted = utcNow;
                entry.State = EntityState.Modified;
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry)
    {
        return entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
}
