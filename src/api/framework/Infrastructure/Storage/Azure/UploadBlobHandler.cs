using imediatus.Framework.Core.Storage.Azure;
using imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;
using MediatR;

namespace imediatus.Framework.Infrastructure.Storage.Azure;

// Optional RequestHandler so you can Send(UploadBlobCommand) via MediatR if needed.
public sealed class UploadBlobHandler(IStorageAzureService storage)
    : IRequestHandler<UploadBlobCommand, UploadBlobResponse>
{
    public Task<UploadBlobResponse> Handle(UploadBlobCommand request, CancellationToken cancellationToken) =>
        storage.UploadBlobsAsync(request, cancellationToken);
}
