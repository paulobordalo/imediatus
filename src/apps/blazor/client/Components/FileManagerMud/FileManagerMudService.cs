#nullable enable
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using imediatus.Blazor.Infrastructure.Api;

namespace imediatus.Blazor.Client.Components.FileManagerMud;

public sealed class FileManagerMudService : IFileManagerService
{
    private readonly IApiClient _api;

    public FileManagerMudService(IApiClient api) => _api = api;

    public async Task EnsureFolderAsync(string fullPath, CancellationToken ct = default)
        => await _api.EnsureFolderEndpointAsync(fullPath, ct);

    public async Task<IReadOnlyList<FileManagerItem>> ListAsync(string path, CancellationToken ct = default)
    {
        var res = await _api.ListFolderContentsEndpointAsync(path, ct);
        // Map SearchBlobResponse -> FileManagerItem
        var items = new List<FileManagerItem>(res.Count);
        foreach (var r in res)
        {
            items.Add(new FileManagerItem
            {
                Id = Guid.NewGuid().ToString("N"), // Removed r.Id, generate new GUID
                Name = r.Name ?? "",
                ParentPath = path,
                IsFolder = r.IsPrefix, // Use IsPrefix for folder detection
                Size = 0, // No Size property in SearchBlobResponse, set to 0
                ContentType = null, // No ContentType property, set to null
                Modified = null, // No LastModified property, set to null
                Url = r.Url // Map Url if needed
            });
        }
        return items;
    }

    public async Task<IReadOnlyList<FileManagerItem>> Files(string path, CancellationToken ct = default)
        => (await ListAsync(path, ct)).Where(x => !x.IsFolder).ToList();

    public async Task<bool> DeleteFolderAsync(string fullPath, CancellationToken ct = default)
        => await _api.DeleteFolderEndpointAsync(fullPath, ct);

    public async Task<bool> DeleteFileAsync(string path, string fileName, CancellationToken ct = default)
        => await _api.DeleteBlobEndpointAsync(path, fileName, ct);

    public async Task<bool> RenameAsync(string path, string currentName, string newName, CancellationToken ct = default)
        => await _api.RenameBlobEndpointAsync(new RenameBlobRequest { FolderName = path, CurrentName = currentName, NewName = newName }, ct);

    public async Task<DownloadBlobResponse?> GetDownloadAsync(string path, string fileName, CancellationToken ct = default)
        => await _api.DownloadBlobEndpointAsync(path, fileName, ct);

    public async Task<string?> TryGetSasUrlAsync(string path, string fileName, CancellationToken ct = default)
    {
        // If your backend exposes SAS URLs in Search/List response, use it. Otherwise return null.
        // Here we return null to prefer inline data URLs for previews.
        return null;
    }

    public async Task<string?> TryGetInlineDataUrlAsync(string path, string fileName, CancellationToken ct = default)
    {
        var dl = await _api.DownloadBlobEndpointAsync(path, fileName, ct);
        if (dl is null) return null;

        // If only string Content is exposed, treat as base64 if possible
        if (!string.IsNullOrEmpty(dl.Content))
            return $"data:application/octet-stream;base64,{dl.Content}";

        return null;
    }

    public Task<byte[]> DownloadBytesFromUrlAsync(string url, CancellationToken ct = default)
        => Task.FromResult(Array.Empty<byte>()); // Not used on WASM w/o HttpClient external

    public async Task<string?> ReadTextAsync(string path, string fileName, CancellationToken ct = default)
    {
        var dl = await _api.DownloadBlobEndpointAsync(path, fileName, ct);
        if (dl is null) return null;

        // dl.Content is a string, not a byte array. Convert from base64 if not null/empty.
        byte[] bytes = string.IsNullOrEmpty(dl.Content)
            ? Array.Empty<byte>()
            : Convert.FromBase64String(dl.Content);
        if (bytes.Length == 0) return null;

        var enc = DetectEncoding(bytes) ?? Encoding.UTF8;
        return enc.GetString(bytes);
    }

    private static Encoding? DetectEncoding(byte[] bytes)
    {
        // Simple UTF BOM detection
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF) return Encoding.UTF8;
        if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE) return Encoding.Unicode; // UTF-16 LE
        if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF) return Encoding.BigEndianUnicode; // UTF-16 BE
        return Encoding.UTF8;
    }
}
