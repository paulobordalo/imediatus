using imediatus.Framework.Core.Identity.Users.Abstractions;
using imediatus.Framework.Infrastructure.Auth.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace imediatus.Framework.Infrastructure.Identity.Users.Endpoints;
public static class GetUsersListEndpoint
{
    internal static RouteHandlerBuilder MapGetUsersListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", (CancellationToken cancellationToken, IUserService service) => service.GetListAsync(cancellationToken))
        .WithName(nameof(GetUsersListEndpoint))
        .WithSummary("get users list")
        .RequirePermission("Permissions.Users.View")
        .WithDescription("get users list");
    }
}
