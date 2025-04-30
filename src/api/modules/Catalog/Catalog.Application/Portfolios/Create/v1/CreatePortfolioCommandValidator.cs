using FluentValidation;
using imediatus.Framework.Core.Identity.Users.Abstractions;

namespace imediatus.WebApi.Catalog.Application.Portfolios.Create.v1;
public class CreatePortfolioCommandValidator : AbstractValidator<CreatePortfolioCommand>
{
    public CreatePortfolioCommandValidator(IUserService userService)
    {
    }
}
