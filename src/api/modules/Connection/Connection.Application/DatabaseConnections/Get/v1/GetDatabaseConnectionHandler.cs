using Microsoft.Extensions.DependencyInjection;
using imediatus.WebApi.Connection.Domain.Exceptions;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Core.Caching;
using imediatus.WebApi.Connection.Domain.Models;
using MediatR;

namespace imediatus.WebApi.Connection.Application.DatabaseConnections.Get.v1;
public sealed class GetDatabaseConnectionHandler(
    [FromKeyedServices("connection:databaseconnections")] IReadRepository<DatabaseConnection> repository,
    ICacheService cache)
    : IRequestHandler<GetDatabaseConnectionRequest, DatabaseConnectionResponse>
{
    public async Task<DatabaseConnectionResponse> Handle(GetDatabaseConnectionRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"brand:{request.Id}",
            async () =>
            {
                var entityItem = await repository.GetByIdAsync(request.Id, cancellationToken);
                if (entityItem == null) throw new DatabaseConnectionNotFoundException(request.Id);
                return new DatabaseConnectionResponse(entityItem.Id, entityItem.ApplicationKey, entityItem.Server, entityItem.DatabaseName, entityItem.Port, entityItem.Username, entityItem.Password, entityItem.UseIntegratedSecurity);
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}
