using imediatus.Framework.Core.Paging;
using imediatus.Framework.Core.Persistence;
using imediatus.WebApi.Workspace.Application.CostCenters.Get.v1;
using imediatus.WebApi.Workspace.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace imediatus.WebApi.Workspace.Application.CostCenters.Search.v1;

public sealed class SearchCostCentersHandler(
    [FromKeyedServices("workspace:costcenters")] IReadRepository<CostCenter> repository)
    : IRequestHandler<SearchCostCentersCommand, PagedList<CostCenterResponse>>
{
    public async Task<PagedList<CostCenterResponse>> Handle(SearchCostCentersCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchCostCenterSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);

        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        var entities = items.Select(o => new CostCenterResponse(o.Id, o.Name)).ToList();

        return new PagedList<CostCenterResponse>(entities, request!.PageNumber, request!.PageSize, totalCount);
    }
}
