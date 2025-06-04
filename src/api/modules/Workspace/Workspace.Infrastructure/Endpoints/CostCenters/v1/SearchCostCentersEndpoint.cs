using imediatus.Framework.Core.Paging;
using imediatus.Framework.Infrastructure.Auth.Policy;
using imediatus.WebApi.Workspace.Application.CostCenters.Get.v1;
using imediatus.WebApi.Workspace.Application.CostCenters.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace imediatus.WebApi.Workspace.Infrastructure.Endpoints.CostCenters.v1;

public static class SearchCostCentersEndpoint
{
    internal static RouteHandlerBuilder MapGetCostCenterListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchCostCentersCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchCostCentersEndpoint))
            .WithSummary("Gets a list of costcenters")
            .WithDescription("Gets a list of costcenters with pagination and filtering support")
            .Produces<PagedList<CostCenterResponse>>()
            .RequirePermission("Permissions.CostCenters.View")
            .MapToApiVersion(1);
    }
}
