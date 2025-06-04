using imediatus.Framework.Infrastructure.Auth.Policy;
using imediatus.WebApi.Workspace.Application.CostCenters.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace imediatus.WebApi.Workspace.Infrastructure.Endpoints.CostCenters.v1;
public static class CreateCostCenterEndpoint
{
    internal static RouteHandlerBuilder MapCostCenterCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateCostCenterCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateCostCenterEndpoint))
            .WithSummary("creates a costcenter")
            .WithDescription("creates a costcenter")
            .Produces<CreateCostCenterResponse>()
            .RequirePermission("Permissions.CostCenters.Create")
            .MapToApiVersion(1);
    }
}
