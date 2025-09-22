using System.Collections.ObjectModel;
using imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;
using MediatR;

namespace imediatus.WebApi.Workspace.Application.Portfolios.Create.v1;

// Event raised after the portfolio data is saved.
public sealed record PortfolioCreatedEvent(
    string FolderName,
    Collection<UploadBlobFile> Attachments
) : INotification;
