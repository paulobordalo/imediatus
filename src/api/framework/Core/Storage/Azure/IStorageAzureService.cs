using imediatus.Framework.Core.Storage.Azure.Features.DownloadBlob;
using imediatus.Framework.Core.Storage.Azure.Features.SearchBlob;
using imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;

namespace imediatus.Framework.Core.Storage.Azure;

public interface IStorageAzureService: IStorageService
{
    Task CreateBlobRootAsync(CancellationToken cancellationToken = default);
    Task CreateBlobFolderAsync(string folderName, CancellationToken cancellationToken = default);
    Task<UploadBlobResponse> UploadBlobsAsync(UploadBlobCommand request, CancellationToken cancellationToken = default);
    Task<List<SearchBlobResponse>> SearchBlobsAsync(SearchBlobCommand request, CancellationToken cancellationToken = default);
    Task<DownloadBlobResponse> GetBlobAsync(DownloadBlobCommand request, CancellationToken cancellationToken = default);
    Task<bool> DeleteBlobFolderAsync(string folderName, CancellationToken cancellationToken = default);
    Task<bool> DeleteBlobAsync(string folderName, string blobName, CancellationToken cancellationToken = default);
}
