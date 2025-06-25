using MediatR;

namespace imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;

public sealed record UploadBlobCommand(Guid ContainerId, string FileName, string? Extension, string? ContentType, byte[] Data, string? Url, string? Path) : IRequest<UploadBlobResponse>;
