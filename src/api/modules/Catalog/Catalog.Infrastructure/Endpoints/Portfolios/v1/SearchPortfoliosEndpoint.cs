using imediatus.Framework.Core.Paging;
using imediatus.Framework.Infrastructure.Auth.Policy;
using imediatus.WebApi.Catalog.Application.Portfolios.Get.v1;
using imediatus.WebApi.Catalog.Application.Portfolios.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace imediatus.WebApi.Catalog.Infrastructure.Endpoints.Portfolios.v1;

public static class SearchPortfoliosEndpoint
{
    internal static RouteHandlerBuilder MapGetPortfolioListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchPortfoliosCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchPortfoliosEndpoint))
            .WithSummary("Gets a list of portfolios")
            .WithDescription("Gets a list of portfolios with pagination and filtering support")
            .Produces<PagedList<PortfolioResponse>>()
            .RequirePermission("Permissions.Portfolios.View")
            .MapToApiVersion(1);
    }
}
