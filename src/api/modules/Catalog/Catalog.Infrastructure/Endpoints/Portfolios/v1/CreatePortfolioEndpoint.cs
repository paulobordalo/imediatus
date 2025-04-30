using imediatus.Framework.Infrastructure.Auth.Policy;
using imediatus.WebApi.Catalog.Application.Portfolios.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace imediatus.WebApi.Catalog.Infrastructure.Endpoints.Portfolios.v1;
public static class CreatePortfolioEndpoint
{
    internal static RouteHandlerBuilder MapPortfolioCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreatePortfolioCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreatePortfolioEndpoint))
            .WithSummary("creates a portfolio")
            .WithDescription("creates a portfolio")
            .Produces<CreatePortfolioResponse>()
            .RequirePermission("Permissions.Portfolios.Create")
            .MapToApiVersion(1);
    }
}
