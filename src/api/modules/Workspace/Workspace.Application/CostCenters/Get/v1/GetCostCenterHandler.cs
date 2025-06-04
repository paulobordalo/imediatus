using Microsoft.Extensions.DependencyInjection;
using imediatus.WebApi.Workspace.Domain.Exceptions;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Core.Caching;
using MediatR;
using imediatus.WebApi.Workspace.Domain.Models;

namespace imediatus.WebApi.Workspace.Application.CostCenters.Get.v1;
public sealed class GetCostCenterHandler(
    [FromKeyedServices("workspace:costcenters")] IReadRepository<CostCenter> repository,
    ICacheService cache)
    : IRequestHandler<GetCostCenterRequest, CostCenterResponse>
{
    public async Task<CostCenterResponse> Handle(GetCostCenterRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"costcenter:{request.Id}",
            async () =>
            {
                var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
                return entity == null
                    ? throw new CostCenterNotFoundException(request.Id)
                    : new CostCenterResponse(entity.Id, entity.Name);
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}
