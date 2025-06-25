using FluentValidation;

namespace imediatus.Framework.Core.Storage.Azure.Features.SearchBlob;

public class SearchBlobValidator : AbstractValidator<SearchBlobCommand>
{
    public SearchBlobValidator()
    {
        RuleFor(p => p.Prefix)
            .NotNull()
            .NotEmpty()
            .MaximumLength(150);
    }
}
