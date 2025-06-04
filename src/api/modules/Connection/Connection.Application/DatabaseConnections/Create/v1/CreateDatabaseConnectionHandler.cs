using imediatus.Framework.Core.Persistence;
using imediatus.WebApi.Connection.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Connection.Application.DatabaseConnections.Create.v1;
public sealed class CreateDatabaseConnectionHandler(
    ILogger<CreateDatabaseConnectionHandler> logger,
    [FromKeyedServices("connection:databaseconnections")] IRepository<DatabaseConnection> repository)
    : IRequestHandler<CreateDatabaseConnectionCommand, CreateDatabaseConnectionResponse>
{
    public async Task<CreateDatabaseConnectionResponse> Handle(CreateDatabaseConnectionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var entity = DatabaseConnection.Create(request.ApplicationKey, request.Server, request.DatabaseName, request.Port, request.Username, request.Password, request.UseIntegratedSecurity);
        await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("database connection created {DatabaseConnectionId}", entity.Id);
        return new CreateDatabaseConnectionResponse(entity.Id);
    }
}
