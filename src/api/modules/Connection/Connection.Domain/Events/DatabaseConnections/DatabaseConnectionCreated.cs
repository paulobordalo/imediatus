using imediatus.Framework.Core.Domain.Events;
using imediatus.WebApi.Connection.Domain.Models;

namespace imediatus.WebApi.Connection.Domain.Events.DatabaseConnections;

public sealed record DatabaseConnectionCreated : DomainEvent
{
    public DatabaseConnection? DatabaseConnection { get; set; }
}
