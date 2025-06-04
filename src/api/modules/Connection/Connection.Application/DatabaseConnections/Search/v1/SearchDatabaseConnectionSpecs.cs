using Ardalis.Specification;
using imediatus.Framework.Core.Paging;
using imediatus.Framework.Core.Specifications;
using imediatus.WebApi.Connection.Application.DatabaseConnections.Get.v1;
using imediatus.WebApi.Connection.Domain.Models;

namespace imediatus.WebApi.Connection.Application.DatabaseConnections.Search.v1;
public class SearchDatabaseConnectionSpecs : EntitiesByBaseFilterSpec<DatabaseConnection, DatabaseConnectionResponse>
{
    public SearchDatabaseConnectionSpecs(SearchDatabaseConnectionsCommand command) 
        : base(command) =>
        Query
            .OrderBy(c => c.ApplicationKey, !command.HasOrderBy())
            .Where(b => b.ApplicationKey.Contains(command.Keyword ?? string.Empty), !string.IsNullOrEmpty(command.Keyword));
}
