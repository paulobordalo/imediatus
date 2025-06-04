using imediatus.Framework.Infrastructure.Auth.Policy;
using imediatus.WebApi.Workspace.Application.Portfolios.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace imediatus.WebApi.Workspace.Infrastructure.Endpoints.Portfolios.v1;
public static class GetPortfolioEndpoint
{
    internal static RouteHandlerBuilder MapGetPortfolioEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetPortfolioRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetPortfolioEndpoint))
            .WithSummary("gets portfolio by id")
            .WithDescription("gets portfolio by id")
            .Produces<PortfolioResponse>()
            .RequirePermission("Permissions.Portfolios.View")
            .MapToApiVersion(1);
    }
}
