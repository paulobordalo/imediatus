using imediatus.Framework.Core.Paging;
using imediatus.Framework.Core.Persistence;
using imediatus.Shared.Enums;
using imediatus.WebApi.Catalog.Application.Portfolios.Get.v1;
using imediatus.WebApi.Catalog.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace imediatus.WebApi.Catalog.Application.Portfolios.Search.v1;

public sealed class SearchPortfoliosHandler(
    [FromKeyedServices("catalog:portfolios")] IReadRepository<Portfolio> repository)
    : IRequestHandler<SearchPortfoliosCommand, PagedList<PortfolioResponse>>
{
    public async Task<PagedList<PortfolioResponse>> Handle(SearchPortfoliosCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchPortfolioSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);

        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        var entities = items.Select(o => new PortfolioResponse(o.Id, o.Summary, o.SecondaryKey, o.StatusId, o.ClassificationId, o.PriorityId, o.AssigneeId, o.ReporterId)).ToList();

        return new PagedList<PortfolioResponse>(entities, request!.PageNumber, request!.PageSize, totalCount);
    }
}
