using imediatus.Framework.Core.Storage.File.Features;

namespace imediatus.Framework.Core.Storage.File;

public interface IStorageFileService: IStorageService
{
    public Task<Uri> UploadAsync<T>(FileUploadCommand? request, FileType supportedFileType, CancellationToken cancellationToken = default)
    where T : class;

    public void Remove(Uri? path);
}
