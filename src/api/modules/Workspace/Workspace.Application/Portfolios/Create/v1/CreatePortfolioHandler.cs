using imediatus.Framework.Core.Identity.Users.Abstractions;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Core.Storage.Azure;
using imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;
using imediatus.Shared.Enums;
using imediatus.WebApi.Workspace.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Workspace.Application.Portfolios.Create.v1;

public sealed class CreatePortfolioHandler(
    ILogger<CreatePortfolioHandler> logger,
    IStorageAzureService storageAzureService,
    ICurrentUser currentUser,
    [FromKeyedServices("workspace:portfolios")] IRepository<Portfolio> repository)
    : IRequestHandler<CreatePortfolioCommand, CreatePortfolioResponse>
{
    public async Task<CreatePortfolioResponse> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var entity = Portfolio.Create(request.Summary, request.StatusId.HasValue ? request.StatusId.Value : PortfolioStatus.ToDo.Value, request.ClassificationId.HasValue ? request.ClassificationId.Value : PortfolioClassification.Public.Value, request.PriorityId.HasValue ? request.PriorityId.Value : PortfolioPriority.Medium.Value, request.CostCenterId, request.AssigneeId.HasValue ? request.AssigneeId.Value : currentUser.GetUserId(), request.ReporterId.HasValue ? request.ReporterId.Value : currentUser.GetUserId());
        await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("portfolio created {Id}", entity.Id);

        if (request.Attachments != null && request.Attachments.Count > 0)
        {
            foreach (var file in request.Attachments)
            {
                // Assuming UploadBlobCommand constructor takes file details
                await storageAzureService.UploadBlobsAsync(new UploadBlobCommand(entity.Id, file.FileName, file.Extension, file.ContentType, file.Data, file.Url, file.Path), cancellationToken);
            }

            logger.LogInformation("Uploads completed for portfolio {Id}", entity.Id);
        }

        return new CreatePortfolioResponse(entity.Id);
    }
}
