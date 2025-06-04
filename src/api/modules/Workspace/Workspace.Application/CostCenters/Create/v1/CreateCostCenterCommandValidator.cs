using FluentValidation;
using imediatus.Framework.Core.Identity.Users.Abstractions;

namespace imediatus.WebApi.Workspace.Application.CostCenters.Create.v1;
public class CreateCostCenterCommandValidator : AbstractValidator<CreateCostCenterCommand>
{
    public CreateCostCenterCommandValidator()
    {
    }
}
