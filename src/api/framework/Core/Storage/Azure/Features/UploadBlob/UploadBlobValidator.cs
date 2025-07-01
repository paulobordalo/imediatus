using FluentValidation;

namespace imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;

public class UploadBlobValidator : AbstractValidator<UploadBlobCommand>
{
    public UploadBlobValidator()
    {
        RuleFor(p => p.Base64Content)
            .NotNull()
            .NotEmpty();

        RuleFor(p => p.FileName)
            .NotNull()
            .NotEmpty();
    }
}
