namespace imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;

public sealed record UploadedBlob(string FileName, string BlobName, Uri Uri);
