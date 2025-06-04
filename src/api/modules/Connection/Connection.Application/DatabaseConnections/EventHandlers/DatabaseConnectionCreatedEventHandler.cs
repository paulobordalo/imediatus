using imediatus.WebApi.Connection.Domain.Events.DatabaseConnections;
using MediatR;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Connection.Application.DatabaseConnections.EventHandlers;

public class DatabaseConnectionCreatedEventHandler(ILogger<DatabaseConnectionCreatedEventHandler> logger) : INotificationHandler<DatabaseConnectionCreated>
{
    public async Task Handle(DatabaseConnectionCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling database connection created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling database connection created domain event..");
    }
}
