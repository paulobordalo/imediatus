using FluentValidation;

namespace imediatus.WebApi.Connection.Application.DatabaseConnections.Update.v1;
public class UpdateDatabaseConnectionCommandValidator : AbstractValidator<UpdateDatabaseConnectionCommand>
{
    public UpdateDatabaseConnectionCommandValidator()
    {
        RuleFor(b => b.ApplicationKey)
            .NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(b => b.Server).NotEmpty().NotNull().MaximumLength(128);
        RuleFor(b => b.DatabaseName).NotEmpty().NotNull().MaximumLength(64);
        RuleFor(b => b.Port).NotEmpty().NotNull();
        RuleFor(b => b.Username).NotEmpty().NotNull().MaximumLength(64);
        RuleFor(b => b.Password).NotEmpty().NotNull().MaximumLength(64);
        RuleFor(b => b.UseIntegratedSecurity).NotNull();
    }
}
