namespace imediatus.Framework.Core.Storage.Azure.Features.RenameBlob;

public sealed record RenameBlobRequest(string FolderName, string CurrentName, string NewName, bool Overwrite);
