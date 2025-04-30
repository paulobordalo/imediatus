using imediatus.WebApi.Catalog.Domain.Events.Portfolios;
using MediatR;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Catalog.Application.Portfolios.EventHandlers;

public class PortfolioCreatedEventHandler(ILogger<PortfolioCreatedEventHandler> logger) : INotificationHandler<PortfolioCreated>
{
    public async Task Handle(PortfolioCreated notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("handling portfolio created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling portfolio created domain event..");
    }
}
