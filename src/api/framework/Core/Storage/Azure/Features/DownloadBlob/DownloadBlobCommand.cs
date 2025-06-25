using MediatR;

namespace imediatus.Framework.Core.Storage.Azure.Features.DownloadBlob;

public sealed record DownloadBlobCommand(Guid containerId, string fileName) : IRequest<DownloadBlobResponse>;
