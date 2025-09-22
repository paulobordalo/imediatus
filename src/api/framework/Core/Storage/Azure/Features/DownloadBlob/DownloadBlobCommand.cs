using MediatR;

namespace imediatus.Framework.Core.Storage.Azure.Features.DownloadBlob;

public sealed record DownloadBlobCommand(string FolderName, string FileName) : IRequest<DownloadBlobResponse>;
