using imediatus.Framework.Core.Paging;
using imediatus.WebApi.Workspace.Application.CostCenters.Get.v1;
using MediatR;

namespace imediatus.WebApi.Workspace.Application.CostCenters.Search.v1;

public class SearchCostCentersCommand : PaginationFilter, IRequest<PagedList<CostCenterResponse>>
{
    public DateTimeOffset? MinimumCreated { get; set; }
    public DateTimeOffset? MaximumCreated { get; set; }
}
