using imediatus.Framework.Core.Paging;
using imediatus.Framework.Core.Persistence;
using imediatus.WebApi.Connection.Application.DatabaseConnections.Get.v1;
using imediatus.WebApi.Connection.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace imediatus.WebApi.Connection.Application.DatabaseConnections.Search.v1;
public sealed class SearchDatabaseConnectionsHandler(
    [FromKeyedServices("connection:databaseconnections")] IReadRepository<DatabaseConnection> repository)
    : IRequestHandler<SearchDatabaseConnectionsCommand, PagedList<DatabaseConnectionResponse>>
{
    public async Task<PagedList<DatabaseConnectionResponse>> Handle(SearchDatabaseConnectionsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchDatabaseConnectionSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<DatabaseConnectionResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}
