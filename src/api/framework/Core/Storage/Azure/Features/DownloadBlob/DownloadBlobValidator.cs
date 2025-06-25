using FluentValidation;

namespace imediatus.Framework.Core.Storage.Azure.Features.DownloadBlob;

public class DownloadBlobValidator : AbstractValidator<DownloadBlobCommand>
{
    public DownloadBlobValidator()
    {
        RuleFor(p => p.fileName)
            .NotNull()
            .NotEmpty();

        RuleFor(p => p.containerId)
            .NotNull();
    }
}
