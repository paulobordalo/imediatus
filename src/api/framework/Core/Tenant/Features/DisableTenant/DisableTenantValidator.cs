﻿using FluentValidation;

namespace imediatus.Framework.Core.Tenant.Features.DisableTenant;
public sealed class DisableTenantValidator : AbstractValidator<DisableTenantCommand>
{
    public DisableTenantValidator() =>
       RuleFor(t => t.TenantId)
           .NotEmpty();
}
