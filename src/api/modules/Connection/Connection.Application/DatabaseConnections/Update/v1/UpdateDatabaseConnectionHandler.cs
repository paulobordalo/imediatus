using imediatus.Framework.Core.Persistence;
using imediatus.WebApi.Connection.Domain.Models;
using imediatus.WebApi.Connection.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Connection.Application.DatabaseConnections.Update.v1;
public sealed class UpdateDatabaseConnectionHandler(
    ILogger<UpdateDatabaseConnectionHandler> logger,
    [FromKeyedServices("catalog:brands")] IRepository<DatabaseConnection> repository)
    : IRequestHandler<UpdateDatabaseConnectionCommand, UpdateDatabaseConnectionResponse>
{
    public async Task<UpdateDatabaseConnectionResponse> Handle(UpdateDatabaseConnectionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var brand = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = brand ?? throw new DatabaseConnectionNotFoundException(request.Id);
        var updatedDatabaseConnection = brand.Update(request.ApplicationKey, request.Server, request.DatabaseName, request.Port, request.Username, request.Password, request.UseIntegratedSecurity);
        await repository.UpdateAsync(updatedDatabaseConnection, cancellationToken);
        logger.LogInformation("DatabaseConnection with id : {DatabaseConnectionId} updated.", brand.Id);
        return new UpdateDatabaseConnectionResponse(brand.Id);
    }
}
