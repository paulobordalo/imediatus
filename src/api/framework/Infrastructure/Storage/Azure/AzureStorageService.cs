using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using imediatus.Framework.Core.Identity.Users.Abstractions;
using imediatus.Framework.Core.Storage.Azure;
using imediatus.Framework.Core.Storage.Azure.Features.DownloadBlob;
using imediatus.Framework.Core.Storage.Azure.Features.SearchBlob;
using imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;
using imediatus.Shared.Extensions;
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

    public async Task CreateBlobRootAsync(CancellationToken cancellationToken = default)
    {
        BlobContainerClient container = await _blobServiceClient.CreateBlobContainerAsync(_currentUser.GetTenant(), PublicAccessType.BlobContainer, cancellationToken: cancellationToken);
        await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task CreateBlobFolderAsync(string folderName, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_currentUser.GetTenant());
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

        BlobClient blobClient = containerClient.GetBlobClient($"{folderName}/{folderName}.trash");
        await blobClient.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes("This is a auto generated file")), new BlobHttpHeaders { ContentType = "text/plain" }, cancellationToken: cancellationToken);
    }

    public async Task<UploadBlobResponse> UploadBlobsAsync(UploadBlobCommand request, CancellationToken cancellationToken = default)
    {
        try
        { 
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_currentUser.GetTenant());
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

            string fullBlobName = $"{request.ContainerId.ToString().TrimEnd('/')}/{request.FileName}";

            // Cria o blob client
            var blobClient = containerClient.GetBlobClient(fullBlobName);

            // Faz upload do ficheiro (sobrescreve se já existir)
            await blobClient.UploadAsync(new BinaryData(request.Data), true, cancellationToken);

            return new UploadBlobResponse(true);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Failed to upload blob for container {ContainerId} and file {FileName}", request.ContainerId, request.FileName);
            return new UploadBlobResponse(false);
        }
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

    public async Task<DownloadBlobResponse> GetBlobAsync(DownloadBlobCommand request, CancellationToken cancellationToken = default)
    {
        BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(_currentUser.GetTenant());

        string fullBlobName = $"{request.containerId.ToString().TrimEnd('/')}/{request.fileName}";

        BlobClient blob = container.GetBlobClient(fullBlobName);
        var download = await blob.DownloadStreamingAsync(cancellationToken: cancellationToken);

        return new DownloadBlobResponse(request.fileName, blob.Uri.AbsoluteUri, download.Value.Content.StreamToString64());
    }

    public async Task<bool> DeleteBlobAsync(string folderName, string blobName, CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(folderName);

            // Nome completo do blob: "folder/ficheiro.ext"
            string fullBlobName = string.IsNullOrEmpty(folderName) ? blobName : $"{folderName.TrimEnd('/')}/{blobName}";

            var blobClient = containerClient.GetBlobClient(fullBlobName);
            var result = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            return result.Value;
        }
        catch
        {
            // Aqui podes logar o erro
            return false;
        }
    }

    public async Task<bool> DeleteBlobFolderAsync(string folderName, CancellationToken cancellationToken = default)
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
        catch
        {
            // Aqui podes logar o erro
            return false;
        }
    }

}
