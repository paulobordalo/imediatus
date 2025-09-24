using System.Collections.Concurrent;
using System.Text;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using imediatus.Framework.Core.Identity.Users.Abstractions;
using imediatus.Framework.Core.Storage.Azure;
using imediatus.Framework.Core.Storage.Azure.Features.DownloadBlob;
using imediatus.Framework.Core.Storage.Azure.Features.SearchBlob;
using imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;
using imediatus.Shared.Extensions;
using imediatus.Shared.Storage;
using Microsoft.Extensions.Logging;

namespace imediatus.Framework.Infrastructure.Storage.Azure;

public class AzureStorageService : IStorageAzureService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger _logger;

    public AzureStorageService(BlobServiceClient blobServiceClient, ICurrentUser currentUser, ILogger<AzureStorageService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task EnsureTenantContainerAsync(CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_currentUser.GetTenant());
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer, cancellationToken: cancellationToken);
    }

    public async Task EnsureFolderAsync(string folderName, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_currentUser.GetTenant());
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

        BlobClient blobClient = containerClient.GetBlobClient($"{folderName}/{folderName}.trash");
        await blobClient.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes("This is a auto generated file")), new BlobHttpHeaders { ContentType = "text/plain" }, cancellationToken: cancellationToken);
    }

    public async Task<UploadBlobResponse> UploadBlobsAsync(
        UploadBlobCommand request,
        CancellationToken cancellationToken = default)
    {
        var uploaded = new ConcurrentBag<UploadedBlob>();
        var errors = new ConcurrentBag<UploadError>();

        var tenant = _currentUser.GetTenant();
        var containerClient = _blobServiceClient.GetBlobContainerClient(tenant);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer, cancellationToken: cancellationToken);

        var files = request.Files?.ToList() ?? new List<UploadBlobFile>();
        if (files.Count == 0)
            return new UploadBlobResponse(true, Array.Empty<UploadedBlob>(), Array.Empty<UploadError>());

        const long FiveGb = 5L * 1024 * 1024 * 1024; // 5 GB
        // Evite paralelismo exagerado quando já recebe byte[] em memória.
        const int filesParallelism = 4;

        await Parallel.ForEachAsync(
            files,
            new ParallelOptions
            {
                MaxDegreeOfParallelism = filesParallelism,
                CancellationToken = cancellationToken
            },
            async (file, ct) =>
            {
                var safeFileName = Path.GetFileName(file.FileName);
                var blobName = $"{request.FolderName:D}/{safeFileName}";
                var blobClient = containerClient.GetBlobClient(blobName);

                try
                {
                    byte[] fileByte = file.FileData.FromBase64ToBytes();

                    // 1) Validação de tamanho (com byte[])
                    var length = fileByte.LongLength;
                    if (length > FiveGb)
                    {
                        errors.Add(new UploadError(file.FileName, $"File exceeds 5 GB limit ({length} bytes)."));
                        return;
                    }

                    // Como recebemos byte[], subimos via Stream para aproveitar o chunking.
                    using var fileStream = new MemoryStream(fileByte, writable: false);
                    fileStream.Position = 0;

                    // 2) Content-Type (deteta se vier vazio)
                    string contentType;
                    if (string.IsNullOrWhiteSpace(file.ContentType))
                    {
                        contentType = BlobContentType.DetectContentType(file.FileName, fileStream);
                        if (string.IsNullOrWhiteSpace(contentType))
                            contentType = "application/octet-stream";
                    }
                    else
                    {
                        contentType = file.ContentType;
                    }

                    // 3) Opções de transferência (chunking) — relevantes quando o SDK lê de Stream.
                    var transfer = new StorageTransferOptions
                    {
                        MaximumConcurrency = 4,
                        InitialTransferSize = 64 * 1024 * 1024, // 64 MB
                        MaximumTransferSize = 64 * 1024 * 1024  // 64 MB
                    };

                    var options = new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = contentType },
                        TransferOptions = transfer
                    };

                    await blobClient.UploadAsync(fileStream, options, ct);

                    uploaded.Add(new UploadedBlob(file.FileName, blobName, blobClient.Uri));
                    _logger.LogInformation("Uploaded blob {BlobName} (tenant {Tenant}).", blobName, tenant);
                }
                catch (RequestFailedException ex) // namespace Azure
                {
                    errors.Add(new UploadError(file.FileName, ex.Message));
                    _logger.LogError(ex,
                        "Error uploading blob {BlobName} (tenant {Tenant}, folder {FolderName}).",
                        blobName, tenant, request.FolderName);
                }
            });

        var success = errors.IsEmpty;
        return new UploadBlobResponse(success, [.. uploaded], [.. errors]);
    }

    public async Task<List<SearchBlobResponse>> SearchBlobsAsync(SearchBlobCommand request, CancellationToken cancellationToken = default)
    {
        BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(_currentUser.GetTenant());

        List<SearchBlobResponse> blobs = new List<SearchBlobResponse>();
        Queue<string> prefixes = new Queue<string>();
        prefixes.Enqueue(request.Prefix);

        do
        {
            string prefix = prefixes.Dequeue();
            await foreach (BlobHierarchyItem blobHierarchyItem in container.GetBlobsByHierarchyAsync(delimiter: "/", prefix: prefix, cancellationToken: cancellationToken))
            {
                if (blobHierarchyItem.IsPrefix)
                {
                    blobs.Add(new SearchBlobResponse(true, blobHierarchyItem.Prefix, string.Empty, string.Empty));
                    prefixes.Enqueue(blobHierarchyItem.Prefix);
                }
                else
                {
                    blobs.Add(new SearchBlobResponse(false, blobHierarchyItem.Blob.Name, new Uri(string.Concat(container.Uri.AbsoluteUri, "/", blobHierarchyItem.Blob.Name)).AbsoluteUri, string.Empty));
                }
            }
        }
        while (prefixes.Count > 0);

        return blobs;
    }

    public async Task<DownloadBlobResponse> DownloadBlobAsync(DownloadBlobCommand request, CancellationToken cancellationToken = default)
    {
        BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(_currentUser.GetTenant());

        string fullBlobName = $"{request.FolderName.ToString().TrimEnd('/')}/{request.FileName}";

        BlobClient blob = container.GetBlobClient(fullBlobName);
        var download = await blob.DownloadStreamingAsync(cancellationToken: cancellationToken);

        return new DownloadBlobResponse(request.FileName, blob.Uri.AbsoluteUri, download.Value.Content.StreamToString64());
    }

    public async Task<bool> DeleteBlobAsync(string folderName, string blobName, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenant = _currentUser.GetTenant();
            var containerClient = _blobServiceClient.GetBlobContainerClient(tenant);

            // Nome completo do blob: "{folderName}/{blobName}"
            string fullBlobName = string.IsNullOrEmpty(folderName) ? blobName : $"{folderName.TrimEnd('/')}/{blobName}";

            var blobClient = containerClient.GetBlobClient(fullBlobName);
            var result = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            return result.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blob {Blob} in folder {Folder} (tenant {Tenant}).", blobName, folderName, _currentUser.GetTenant());
            return false;
        }
    }

    public async Task<bool> DeleteFolderAsync(string folderName, CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_currentUser.GetTenant());

            string prefix = folderName.TrimEnd('/') + "/";

            // Lista todos os blobs que começam pelo prefixo da "pasta"
            var blobs = containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken);
            bool deletedAny = false;

            await foreach (var blobItem in blobs)
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
                deletedAny = true;
            }

            return deletedAny;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting folder {Folder} (tenant {Tenant}).", folderName, _currentUser.GetTenant());
            return false;
        }
    }

    // Renomeia um blob (na prática, copia e apaga o original). Opera dentro do mesmo "folder/prefix".
    public async Task<bool> RenameBlobAsync(string folderName, string currentName, string newName, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        var tenant = _currentUser.GetTenant();
        var containerClient = _blobServiceClient.GetBlobContainerClient(tenant);

        string srcFull = string.IsNullOrWhiteSpace(folderName) ? currentName : $"{folderName.TrimEnd('/')}/{currentName}";
        string dstFull = string.IsNullOrWhiteSpace(folderName) ? newName : $"{folderName.TrimEnd('/')}/{newName}";

        try
        {
            var src = containerClient.GetBlobClient(srcFull);
            var dst = containerClient.GetBlobClient(dstFull);

            // Verifica existência do source
            if (!await src.ExistsAsync(cancellationToken))
                return false;

            // Lida com overwrite
            if (!overwrite && await dst.ExistsAsync(cancellationToken))
                return false;

            if (overwrite && await dst.ExistsAsync(cancellationToken))
                await dst.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);

            // Copia server-side e aguarda conclusão
            var op = await dst.StartCopyFromUriAsync(src.Uri, cancellationToken: cancellationToken);
            await op.WaitForCompletionAsync(cancellationToken);

            // Verifica status final da cópia
            var props = await dst.GetPropertiesAsync(cancellationToken: cancellationToken);
            if (!string.Equals(props.Value.CopyStatus.ToString(), "Success", StringComparison.OrdinalIgnoreCase))
            {
                // Em caso de falha, tenta limpar o destino parcialmente criado
                await dst.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
                return false;
            }

            // Apaga o original
            await src.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);

            _logger.LogInformation("Renamed blob {Src} to {Dst} (tenant {Tenant}).", srcFull, dstFull, tenant);
            return true;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Error renaming blob {Src} to {Dst} (tenant {Tenant}).", srcFull, dstFull, tenant);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error renaming blob {Src} to {Dst} (tenant {Tenant}).", srcFull, dstFull, tenant);
            return false;
        }
    }

    // Lista todos os itens (subpastas e blobs) recursivamente a partir de uma pasta.
    public async Task<List<SearchBlobResponse>> ListFolderContentsAsync(string folderName, CancellationToken cancellationToken = default)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_currentUser.GetTenant());

        var result = new List<SearchBlobResponse>();
        var queue = new Queue<string>();

        // Normaliza prefixo. Se vazio, lista tudo.
        var rootPrefix = string.IsNullOrWhiteSpace(folderName) ? string.Empty : folderName.TrimEnd('/') + "/";
        queue.Enqueue(rootPrefix);

        do
        {
            var prefix = queue.Dequeue();
            await foreach (var item in container.GetBlobsByHierarchyAsync(delimiter: "/", prefix: prefix, cancellationToken: cancellationToken))
            {
                if (item.IsPrefix)
                {
                    result.Add(new SearchBlobResponse(true, item.Prefix, string.Empty, string.Empty));
                    queue.Enqueue(item.Prefix);
                }
                else
                {
                    var name = Path.GetFileName(item.Blob.Name);
                    var url = new Uri($"{container.Uri.AbsoluteUri}/{prefix}{name}").AbsoluteUri;
                    result.Add(new SearchBlobResponse(false, name, url, string.Empty));
                }
            }
        } while (queue.Count > 0);

        return result;
    }
}
