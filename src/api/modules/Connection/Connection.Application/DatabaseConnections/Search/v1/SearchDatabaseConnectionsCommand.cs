using imediatus.Framework.Core.Paging;
using imediatus.WebApi.Connection.Application.DatabaseConnections.Get.v1;
using MediatR;

namespace imediatus.WebApi.Connection.Application.DatabaseConnections.Search.v1;

public class SearchDatabaseConnectionsCommand : PaginationFilter, IRequest<PagedList<DatabaseConnectionResponse>>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}
