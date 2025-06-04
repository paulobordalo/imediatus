using imediatus.Framework.Core.Persistence;
using imediatus.WebApi.Workspace.Domain.Exceptions;
using imediatus.WebApi.Workspace.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Workspace.Application.CostCenters.Update.v1;
public sealed class UpdateCostCenterHandler(
    ILogger<UpdateCostCenterHandler> logger,
    [FromKeyedServices("workspace:costcenters")] IRepository<CostCenter> repository)
    : IRequestHandler<UpdateCostCenterCommand, UpdateCostCenterResponse>
{
    public async Task<UpdateCostCenterResponse> Handle(UpdateCostCenterCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = entity ?? throw new CostCenterNotFoundException(request.Id);
        var updatedEntity = entity.Update(request.Name);
        await repository.UpdateAsync(updatedEntity , cancellationToken);
        logger.LogInformation("costcenter with id : {CostCenterId} updated.", entity.Id);
        return new UpdateCostCenterResponse(entity.Id);
    }
}
