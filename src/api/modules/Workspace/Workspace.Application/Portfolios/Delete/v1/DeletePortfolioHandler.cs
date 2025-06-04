using imediatus.Framework.Core.Persistence;
using imediatus.WebApi.Workspace.Domain.Exceptions;
using imediatus.WebApi.Workspace.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Workspace.Application.Portfolios.Delete.v1;
public sealed class DeletePortfolioHandler(
    ILogger<DeletePortfolioHandler> logger,
    [FromKeyedServices("workspace:portfolios")] IRepository<Portfolio> repository)
    : IRequestHandler<DeletePortfolioCommand>
{
    public async Task Handle(DeletePortfolioCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = entity ?? throw new PortfolioNotFoundException(request.Id);
        await repository.DeleteAsync(entity, cancellationToken);
        logger.LogInformation("portfolio with id : {Id} deleted", entity.Id);
    }
}
