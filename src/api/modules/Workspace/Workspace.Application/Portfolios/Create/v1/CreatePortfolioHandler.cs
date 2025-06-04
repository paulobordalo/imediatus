using imediatus.Framework.Core.Identity.Users.Abstractions;
using imediatus.Framework.Core.Persistence;
using imediatus.Shared.Enums;
using imediatus.WebApi.Workspace.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Workspace.Application.Portfolios.Create.v1;
public sealed class CreatePortfolioHandler(
    ILogger<CreatePortfolioHandler> logger,
    ICurrentUser currentUser,
    [FromKeyedServices("workspace:portfolios")] IRepository<Portfolio> repository)
    : IRequestHandler<CreatePortfolioCommand, CreatePortfolioResponse>
{
    public async Task<CreatePortfolioResponse> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var entity = Portfolio.Create(request.Summary, request.StatusId.HasValue ? request.StatusId.Value : PortfolioStatus.ToDo.Value, request.ClassificationId.HasValue ? request.ClassificationId.Value : PortfolioClassification.Public.Value, request.PriorityId.HasValue ? request.PriorityId.Value : PortfolioPriority.Medium.Value, request.CostCenterId, request.AssigneeId, request.ReporterId.HasValue ? request.ReporterId.Value : currentUser.GetUserId());
        await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("portfolio created {Id}", entity.Id);
        return new CreatePortfolioResponse(entity.Id);
    }
}
