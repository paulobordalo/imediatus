using imediatus.Framework.Infrastructure.Auth.Policy;
using imediatus.WebApi.Workspace.Application.Portfolios.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace imediatus.WebApi.Workspace.Infrastructure.Endpoints.Portfolios.v1;
public static class UpdatePortfolioEndpoint
{
    internal static RouteHandlerBuilder MapPortfolioUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdatePortfolioCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdatePortfolioEndpoint))
            .WithSummary("update a portfolio")
            .WithDescription("update a portfolio")
            .Produces<UpdatePortfolioResponse>()
            .RequirePermission("Permissions.Portfolios.Update")
            .MapToApiVersion(1);
    }
}
