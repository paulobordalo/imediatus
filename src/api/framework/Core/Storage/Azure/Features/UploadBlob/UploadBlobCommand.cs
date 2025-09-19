using MediatR;

namespace imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;

public sealed record UploadBlobCommand(string FolderName, IEnumerable<UploadBlobFile> Files) : IRequest<UploadBlobResponse>;
