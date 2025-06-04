using imediatus.Framework.Infrastructure.Auth.Policy;
using imediatus.WebApi.Workspace.Application.CostCenters.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace imediatus.WebApi.Workspace.Infrastructure.Endpoints.CostCenters.v1;
public static class UpdateCostCenterEndpoint
{
    internal static RouteHandlerBuilder MapCostCenterUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateCostCenterCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateCostCenterEndpoint))
            .WithSummary("update a costcenter")
            .WithDescription("update a costcenter")
            .Produces<UpdateCostCenterResponse>()
            .RequirePermission("Permissions.CostCenters.Update")
            .MapToApiVersion(1);
    }
}
