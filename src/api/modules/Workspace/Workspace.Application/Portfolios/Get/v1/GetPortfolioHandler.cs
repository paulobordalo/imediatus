using Microsoft.Extensions.DependencyInjection;
using imediatus.WebApi.Workspace.Domain.Exceptions;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Core.Caching;
using imediatus.WebApi.Workspace.Domain.Models;
using MediatR;

namespace imediatus.WebApi.Workspace.Application.Portfolios.Get.v1;
public sealed class GetPortfolioHandler(
    [FromKeyedServices("workspace:portfolios")] IReadRepository<Portfolio> repository,
    ICacheService cache)
    : IRequestHandler<GetPortfolioRequest, PortfolioResponse>
{
    public async Task<PortfolioResponse> Handle(GetPortfolioRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"portfolio:{request.Id}",
            async () =>
            {
                var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
                return entity == null
                    ? throw new PortfolioNotFoundException(request.Id)
                    : new PortfolioResponse(entity.Id, entity.Summary, entity.SecondaryKey, entity.StatusId, entity.ClassificationId, entity.PriorityId, entity.CostCenterId, entity.AssigneeId, entity.ReporterId);
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}
