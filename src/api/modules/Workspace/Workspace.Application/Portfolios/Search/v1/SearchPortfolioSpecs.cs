using Ardalis.Specification;
using imediatus.Framework.Core.Paging;
using imediatus.Framework.Core.Specifications;
using imediatus.WebApi.Workspace.Application.Portfolios.Get.v1;
using imediatus.WebApi.Workspace.Domain.Models;

namespace imediatus.WebApi.Workspace.Application.Portfolios.Search.v1;

public class SearchPortfolioSpecs : EntitiesByBaseFilterSpec<Portfolio, PortfolioResponse>
{
    public SearchPortfolioSpecs(SearchPortfoliosCommand command) 
        : base(command) =>
        Query
            .OrderByDescending(c => c.LastModified, !command.HasOrderBy())
            .Where(p => p.Created >= command.MinimumCreated!.Value, command.MinimumCreated.HasValue)
            .Where(p => p.Created <= command.MaximumCreated!.Value, command.MaximumCreated.HasValue);
}
