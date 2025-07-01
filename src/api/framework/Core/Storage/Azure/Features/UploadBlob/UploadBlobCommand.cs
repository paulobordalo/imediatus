using MediatR;

namespace imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;

public sealed record UploadBlobCommand(Guid ContainerId, string FileName, string Base64Content, string? ContentType) : IRequest<UploadBlobResponse>;
