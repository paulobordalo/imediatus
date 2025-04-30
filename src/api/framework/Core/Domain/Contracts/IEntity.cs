using System.Collections.ObjectModel;
using imediatus.Framework.Core.Domain.Events;

namespace imediatus.Framework.Core.Domain.Contracts;

public interface IEntity
{
    Collection<DomainEvent> DomainEvents { get; }
}

public interface IEntity<out TId> : IEntity
{
    TId Id { get; }
}
