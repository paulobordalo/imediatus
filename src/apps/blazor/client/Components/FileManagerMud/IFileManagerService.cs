#nullable enable
using imediatus.Blazor.Infrastructure.Api;

namespace imediatus.Blazor.Client.Components.FileManagerMud;

public interface IFileManagerService
{
    Task EnsureFolderAsync(string fullPath, CancellationToken ct = default);
    Task<IReadOnlyList<FileManagerItem>> ListAsync(string path, CancellationToken ct = default);
    Task<IReadOnlyList<FileManagerItem>> Files(string path, CancellationToken ct = default);
    Task<bool> DeleteFolderAsync(string fullPath, CancellationToken ct = default);
    Task<bool> DeleteFileAsync(string path, string fileName, CancellationToken ct = default);
    Task<bool> RenameAsync(string path, string currentName, string newName, CancellationToken ct = default);
    Task<DownloadBlobResponse?> GetDownloadAsync(string path, string fileName, CancellationToken ct = default);
    Task<string?> TryGetSasUrlAsync(string path, string fileName, CancellationToken ct = default);
    Task<string?> TryGetInlineDataUrlAsync(string path, string fileName, CancellationToken ct = default);
    Task<byte[]> DownloadBytesFromUrlAsync(string url, CancellationToken ct = default);
    Task<string?> ReadTextAsync(string path, string fileName, CancellationToken ct = default);
}
