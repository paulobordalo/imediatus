﻿using FluentValidation;
using imediatus.Framework.Core.Identity.Roles;
using imediatus.Framework.Core.Identity.Roles.Features.UpdatePermissions;
using imediatus.Framework.Infrastructure.Auth.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace imediatus.Framework.Infrastructure.Identity.Roles.Endpoints;
public static class UpdateRolePermissionsEndpoint
{
    public static RouteHandlerBuilder MapUpdateRolePermissionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{id}/permissions", async (
            UpdatePermissionsCommand request,
            IRoleService roleService,
            string id,
            [FromServices] IValidator<UpdatePermissionsCommand> validator) =>
        {
            if (id != request.RoleId) return Results.BadRequest();
            var response = await roleService.UpdatePermissionsAsync(request);
            return Results.Ok(response);
        })
        .WithName(nameof(UpdateRolePermissionsEndpoint))
        .WithSummary("update role permissions")
        .RequirePermission("Permissions.Roles.Create")
        .WithDescription("update role permissions");
    }
}
