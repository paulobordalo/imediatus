using FluentValidation;
using imediatus.Framework.Core.Identity.Users.Abstractions;

namespace imediatus.WebApi.Workspace.Application.Portfolios.Update.v1;
public class UpdatePortfolioCommandValidator : AbstractValidator<UpdatePortfolioCommand>
{
    public UpdatePortfolioCommandValidator(IUserService userService)
    {
        RuleFor(p => p.Id)
            .NotNull()
            .NotEmpty();
        RuleFor(p => p.StatusId)
            .NotNull()
            .NotEmpty();
        RuleFor(p => p.ClassificationId)
            .NotNull()
            .NotEmpty();
        RuleFor(p => p.PriorityId)
            .NotNull()
            .NotEmpty();
        RuleFor(p => p.Summary)
            .MinimumLength(2)
            .MaximumLength(64);
        RuleFor(p => p.ReporterId)
            .NotNull()
            .NotEmpty()
            .MustAsync(async (id, ct) => await userService.GetAsync(id.ToString(), ct) is not null)
            .WithMessage("Reporter {PropertyValue} Not Found.");
    }
}
