using MediatR;

namespace imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;

public sealed record UploadBlobCommand(Guid PortfolioId, IEnumerable<UploadBlobFile> Files) : IRequest<UploadBlobResponse>;
