using FluentValidation;

namespace imediatus.Framework.Core.Tenant.Features.ActivateTenant;
public sealed class ActivateTenantValidator : AbstractValidator<ActivateTenantCommand>
{
    public ActivateTenantValidator() =>
       RuleFor(t => t.TenantId)
           .NotEmpty();
}
