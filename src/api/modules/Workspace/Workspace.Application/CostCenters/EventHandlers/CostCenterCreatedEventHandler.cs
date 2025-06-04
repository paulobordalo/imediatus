using imediatus.WebApi.Workspace.Domain.Events.CostCenters;
using MediatR;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Workspace.Application.CostCenters.EventHandlers;

public class CostCenterCreatedEventHandler(ILogger<CostCenterCreatedEventHandler> logger) : INotificationHandler<CostCenterCreated>
{
    public async Task Handle(CostCenterCreated notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("handling costcenter created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling costcenter created domain event..");
    }
}
