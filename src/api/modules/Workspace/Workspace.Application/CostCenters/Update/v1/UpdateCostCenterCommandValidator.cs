using FluentValidation;
using imediatus.Framework.Core.Identity.Users.Abstractions;

namespace imediatus.WebApi.Workspace.Application.CostCenters.Update.v1;
public class UpdateCostCenterCommandValidator : AbstractValidator<UpdateCostCenterCommand>
{
    public UpdateCostCenterCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotNull()
            .NotEmpty();
        RuleFor(p => p.Name)
            .NotNull()
            .NotEmpty();
    }
}
