using imediatus.Framework.Core.Storage.Azure.Features.DownloadBlob;
using imediatus.Framework.Core.Storage.Azure.Features.SearchBlob;
using imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;

namespace imediatus.Framework.Core.Storage.Azure;

public interface IStorageAzureService: IStorageService
{
    Task EnsureTenantContainerAsync(CancellationToken cancellationToken = default);
    Task EnsureFolderAsync(string folderName, CancellationToken cancellationToken = default);
    Task<UploadBlobResponse> UploadBlobsAsync(UploadBlobCommand request, CancellationToken cancellationToken = default);
    Task<List<SearchBlobResponse>> SearchBlobsAsync(SearchBlobCommand request, CancellationToken cancellationToken = default);
    Task<DownloadBlobResponse> DownloadBlobAsync(DownloadBlobCommand request, CancellationToken cancellationToken = default);
    Task<bool> DeleteFolderAsync(string folderName, CancellationToken cancellationToken = default);
    Task<bool> DeleteBlobAsync(string folderName, string blobName, CancellationToken cancellationToken = default);
    Task<bool> RenameBlobAsync(string folderName, string currentName, string newName, bool overwrite = false, CancellationToken cancellationToken = default);
    Task<List<SearchBlobResponse>> ListFolderContentsAsync(string folderName, CancellationToken cancellationToken = default);
}
