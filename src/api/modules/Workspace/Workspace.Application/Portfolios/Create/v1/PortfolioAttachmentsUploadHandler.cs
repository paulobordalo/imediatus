using imediatus.Framework.Core.Storage.Azure;
using imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;
using MediatR;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Workspace.Application.Portfolios.Create.v1;

// Handles the upload work separately from data persistence.
public sealed class PortfolioAttachmentsUploadHandler(
    ILogger<PortfolioAttachmentsUploadHandler> logger,
    IStorageAzureService storageAzureService)
    : INotificationHandler<PortfolioCreatedEvent>
{
    public async Task Handle(PortfolioCreatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Attachments is null || notification.Attachments.Count == 0)
            return;

        var result = await storageAzureService.UploadBlobsAsync(
            new UploadBlobCommand(notification.PortfolioId, notification.Attachments),
            cancellationToken);

        if (!result.Success)
            logger.LogWarning("Some attachments failed to upload for portfolio {Id}: {Errors}",
                notification.PortfolioId, string.Join(", ", result.Errors.Select(e => $"{e.FileName}: {e.Error}")));
        else
            logger.LogInformation("Uploads completed for portfolio {Id}", notification.PortfolioId);
    }
}
