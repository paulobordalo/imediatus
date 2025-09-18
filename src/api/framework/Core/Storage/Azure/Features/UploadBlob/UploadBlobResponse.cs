namespace imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;

public sealed record UploadBlobResponse(
    bool Success,
    IReadOnlyList<UploadedBlob> Uploaded,
    IReadOnlyList<UploadError> Errors);

