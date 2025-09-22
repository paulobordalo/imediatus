using FluentValidation;

namespace imediatus.Framework.Core.Storage.Azure.Features.DownloadBlob;

public class DownloadBlobValidator : AbstractValidator<DownloadBlobCommand>
{
    public DownloadBlobValidator()
    {
        RuleFor(p => p.FolderName)
            .NotNull();

        RuleFor(p => p.FileName)
            .NotNull()
            .NotEmpty();
    }
}
