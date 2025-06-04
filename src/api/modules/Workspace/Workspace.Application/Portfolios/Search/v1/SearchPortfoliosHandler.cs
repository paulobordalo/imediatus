using imediatus.Framework.Core.Paging;
using imediatus.Framework.Core.Persistence;
using imediatus.WebApi.Workspace.Application.Portfolios.Get.v1;
using imediatus.WebApi.Workspace.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace imediatus.WebApi.Workspace.Application.Portfolios.Search.v1;

public sealed class SearchPortfoliosHandler(
    [FromKeyedServices("workspace:portfolios")] IReadRepository<Portfolio> repository)
    : IRequestHandler<SearchPortfoliosCommand, PagedList<PortfolioResponse>>
{
    public async Task<PagedList<PortfolioResponse>> Handle(SearchPortfoliosCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchPortfolioSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);

        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        var entities = items.Select(o => new PortfolioResponse(o.Id, o.Summary, o.SecondaryKey, o.StatusId, o.ClassificationId, o.PriorityId, o.CostCenterId, o.AssigneeId, o.ReporterId)).ToList();

        return new PagedList<PortfolioResponse>(entities, request!.PageNumber, request!.PageSize, totalCount);
    }
}
