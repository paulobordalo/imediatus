﻿using MediatR;

namespace imediatus.Framework.Core.Tenant.Features.CreateTenant;
public sealed record CreateTenantCommand(string Id,
    string Name,
    string? ConnectionString,
    string AdminEmail,
    string? Issuer) : IRequest<CreateTenantResponse>;
