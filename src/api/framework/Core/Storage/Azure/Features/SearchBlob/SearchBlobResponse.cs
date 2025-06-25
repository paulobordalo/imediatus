namespace imediatus.Framework.Core.Storage.Azure.Features.SearchBlob;

public sealed record SearchBlobResponse(bool IsPrefix, string Name, string Url, string Content);
