using imediatus.Framework.Core.Persistence;
using imediatus.WebApi.Connection.Domain.Models;
using imediatus.WebApi.Connection.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Connection.Application.DatabaseConnections.Delete.v1;
public sealed class DeleteDatabaseConnectionHandler(
    ILogger<DeleteDatabaseConnectionHandler> logger,
    [FromKeyedServices("connection:databaseconnections")] IRepository<DatabaseConnection> repository)
    : IRequestHandler<DeleteDatabaseConnectionCommand>
{
    public async Task Handle(DeleteDatabaseConnectionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = entity ?? throw new DatabaseConnectionNotFoundException(request.Id);
        await repository.DeleteAsync(entity, cancellationToken);
        logger.LogInformation("DatabaseConnection with id : {DatabaseConnectionId} deleted", entity.Id);
    }
}
