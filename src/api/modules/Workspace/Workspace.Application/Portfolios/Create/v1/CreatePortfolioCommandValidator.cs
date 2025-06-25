using FluentValidation;
using imediatus.Framework.Core.Identity.Users.Abstractions;

namespace imediatus.WebApi.Workspace.Application.Portfolios.Create.v1;
public class CreatePortfolioCommandValidator : AbstractValidator<CreatePortfolioCommand>
{
    public CreatePortfolioCommandValidator(IUserService userService)
    {
        RuleFor(command => command.Summary)
            .NotEmpty().WithMessage("Summary is required.");
        
        RuleFor(command => command.StatusId)
            .NotNull().WithMessage("StatusId is required.");
        
        RuleFor(command => command.ClassificationId)
            .NotNull().WithMessage("ClassificationId is required.");
        
        RuleFor(command => command.PriorityId)
            .NotNull().WithMessage("PriorityId is required.");
        
        RuleFor(command => command.AssigneeId)
            .NotNull().WithMessage("AssigneeId is required.");
        
        RuleFor(command => command.ReporterId)
            .NotNull().WithMessage("ReporterId is required.");
    }
}
