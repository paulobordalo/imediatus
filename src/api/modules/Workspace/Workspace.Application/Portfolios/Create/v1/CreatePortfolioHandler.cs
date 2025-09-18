using System.Collections.ObjectModel;
using imediatus.Framework.Core.Identity.Users.Abstractions;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;
using imediatus.Shared.Enums;
using imediatus.WebApi.Workspace.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Workspace.Application.Portfolios.Create.v1;

public sealed class CreatePortfolioHandler(
    ILogger<CreatePortfolioHandler> logger,
    ICurrentUser currentUser,
    IPublisher publisher,
    [FromKeyedServices("workspace:portfolios")] IRepository<Portfolio> repository)
    : IRequestHandler<CreatePortfolioCommand, CreatePortfolioResponse>
{
    public async Task<CreatePortfolioResponse> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Create entity with sensible defaults when not provided
        var entity = Portfolio.Create(
            request.Summary,
            request.StatusId ?? PortfolioStatus.ToDo.Value,
            request.ClassificationId ?? PortfolioClassification.Public.Value,
            request.PriorityId ?? PortfolioPriority.Medium.Value,
            request.CostCenterId,
            request.AssigneeId ?? currentUser.GetUserId(),
            request.ReporterId ?? currentUser.GetUserId());

        await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("Portfolio created {Id}", entity.Id);

        // Publish event to process attachments asynchronously
        if (request.Attachments is { Count: > 0 })
        {
            // Map base64 UploadBlobFile -> byte[] UploadableBlobFile for the storage pipeline
            var uploadables = new Collection<UploadBlobFile>(
                [.. request.Attachments
                       .Where(a => !string.IsNullOrWhiteSpace(a.FileData))
                       .Select(a =>
                       {
                           // FileData is base64 from the client
                           var contentType = string.IsNullOrWhiteSpace(a.ContentType) ? "application/octet-stream" : a.ContentType;
                           return new UploadBlobFile(a.FileData, a.FileName, contentType);
                       })]);

            await publisher.Publish(new PortfolioCreatedEvent(entity.Id, uploadables), cancellationToken);
            logger.LogInformation("PortfolioCreatedEvent published for {Id} with {Count} attachment(s)", entity.Id, uploadables.Count);
        }

        return new CreatePortfolioResponse(entity.Id);
    }
}
