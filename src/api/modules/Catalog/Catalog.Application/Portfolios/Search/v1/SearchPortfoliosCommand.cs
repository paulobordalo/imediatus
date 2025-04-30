using imediatus.Framework.Core.Paging;
using imediatus.WebApi.Catalog.Application.Portfolios.Get.v1;
using MediatR;

namespace imediatus.WebApi.Catalog.Application.Portfolios.Search.v1;

public class SearchPortfoliosCommand : PaginationFilter, IRequest<PagedList<PortfolioResponse>>
{
    public DateTimeOffset? MinimumCreated { get; set; }
    public DateTimeOffset? MaximumCreated { get; set; }
}
