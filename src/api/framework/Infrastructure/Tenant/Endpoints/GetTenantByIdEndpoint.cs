﻿using imediatus.Framework.Core.Tenant.Features.GetTenantById;
using imediatus.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace imediatus.Framework.Infrastructure.Tenant.Endpoints;
public static class GetTenantByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetTenantByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{id}", (ISender mediator, string id) => mediator.Send(new GetTenantByIdQuery(id)))
                                .WithName(nameof(GetTenantByIdEndpoint))
                                .WithSummary("get tenant by id")
                                .RequirePermission("Permissions.Tenants.View")
                                .WithDescription("get tenant by id");
    }
}
