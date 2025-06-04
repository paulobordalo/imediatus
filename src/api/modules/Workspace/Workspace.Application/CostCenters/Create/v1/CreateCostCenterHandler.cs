using imediatus.Framework.Core.Persistence;
using imediatus.WebApi.Workspace.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Workspace.Application.CostCenters.Create.v1;
public sealed class CreateCostCenterHandler(
    ILogger<CreateCostCenterHandler> logger,
    [FromKeyedServices("workspace:costcenters")] IRepository<CostCenter> repository)
    : IRequestHandler<CreateCostCenterCommand, CreateCostCenterResponse>
{
    public async Task<CreateCostCenterResponse> Handle(CreateCostCenterCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var entity = CostCenter.Create(request.Name);
        await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("costcenter created {Id}", entity.Id);
        return new CreateCostCenterResponse(entity.Id);
    }
}
