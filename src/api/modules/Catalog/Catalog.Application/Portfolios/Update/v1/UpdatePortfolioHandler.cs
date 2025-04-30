using imediatus.Framework.Core.Persistence;
using imediatus.WebApi.Catalog.Domain.Exceptions;
using imediatus.WebApi.Catalog.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Catalog.Application.Portfolios.Update.v1;
public sealed class UpdatePortfolioHandler(
    ILogger<UpdatePortfolioHandler> logger,
    [FromKeyedServices("catalog:portfolios")] IRepository<Portfolio> repository)
    : IRequestHandler<UpdatePortfolioCommand, UpdatePortfolioResponse>
{
    public async Task<UpdatePortfolioResponse> Handle(UpdatePortfolioCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = entity ?? throw new PortfolioNotFoundException(request.Id);
        var updatedEntity = entity.Update(request.Summary, request.StatusId, request.ClassificationId, request.PriorityId, request.AssigneeId, request.ReporterId);
        await repository.UpdateAsync(updatedEntity , cancellationToken);
        logger.LogInformation("portfolio with id : {PortfolioId} updated.", entity.Id);
        return new UpdatePortfolioResponse(entity.Id);
    }
}
