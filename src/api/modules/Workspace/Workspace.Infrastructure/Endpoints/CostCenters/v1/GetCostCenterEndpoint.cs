using imediatus.Framework.Infrastructure.Auth.Policy;
using imediatus.WebApi.Workspace.Application.CostCenters.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace imediatus.WebApi.Workspace.Infrastructure.Endpoints.CostCenters.v1;
public static class GetCostCenterEndpoint
{
    internal static RouteHandlerBuilder MapGetCostCenterEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetCostCenterRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetCostCenterEndpoint))
            .WithSummary("gets costcenter by id")
            .WithDescription("gets costcenter by id")
            .Produces<CostCenterResponse>()
            .RequirePermission("Permissions.CostCenters.View")
            .MapToApiVersion(1);
    }
}
