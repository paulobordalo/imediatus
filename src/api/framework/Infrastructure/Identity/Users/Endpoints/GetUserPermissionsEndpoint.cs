﻿using System.Security.Claims;
using imediatus.Framework.Core.Exceptions;
using imediatus.Framework.Core.Identity.Users.Abstractions;
using imediatus.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace imediatus.Framework.Infrastructure.Identity.Users.Endpoints;
public static class GetUserPermissionsEndpoint
{
    internal static RouteHandlerBuilder MapGetCurrentUserPermissionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/permissions", async (ClaimsPrincipal user, IUserService service, CancellationToken cancellationToken) =>
        {
            if (user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException();
            }

            return await service.GetPermissionsAsync(userId, cancellationToken);
        })
        .WithName("GetUserPermissions")
        .WithSummary("Get current user permissions")
        .WithDescription("Get current user permissions");
    }
}
