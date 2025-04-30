using imediatus.Framework.Core.Tenant.Features.UpgradeSubscription;
using imediatus.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace imediatus.Framework.Infrastructure.Tenant.Endpoints;

public static class UpgradeSubscriptionEndpoint
{
    internal static RouteHandlerBuilder MapUpgradeTenantSubscriptionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/upgrade", (UpgradeSubscriptionCommand command, ISender mediator) => mediator.Send(command))
                                .WithName(nameof(UpgradeSubscriptionEndpoint))
                                .WithSummary("upgrade tenant subscription")
                                .RequirePermission("Permissions.Tenants.Update")
                                .WithDescription("upgrade tenant subscription");
    }
}
