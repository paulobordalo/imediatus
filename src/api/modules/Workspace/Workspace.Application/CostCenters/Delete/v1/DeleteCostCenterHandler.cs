using imediatus.Framework.Core.Persistence;
using imediatus.WebApi.Workspace.Domain.Exceptions;
using imediatus.WebApi.Workspace.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Workspace.Application.CostCenters.Delete.v1;
public sealed class DeleteCostCenterHandler(
    ILogger<DeleteCostCenterHandler> logger,
    [FromKeyedServices("workspace:costcenters")] IRepository<CostCenter> repository)
    : IRequestHandler<DeleteCostCenterCommand>
{
    public async Task Handle(DeleteCostCenterCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = entity ?? throw new CostCenterNotFoundException(request.Id);
        await repository.DeleteAsync(entity, cancellationToken);
        logger.LogInformation("costcenter with id : {Id} deleted", entity.Id);
    }
}
