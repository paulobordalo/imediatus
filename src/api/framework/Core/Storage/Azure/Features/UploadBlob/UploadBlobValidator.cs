using FluentValidation;

namespace imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;

public class UploadBlobValidator : AbstractValidator<UploadBlobCommand>
{
    public UploadBlobValidator()
    {
        RuleFor(p => p.PortfolioId)
            .NotNull()
            .NotEmpty();

        RuleFor(p => p.Files)
            .NotNull()
            .NotEmpty();
    }
}
