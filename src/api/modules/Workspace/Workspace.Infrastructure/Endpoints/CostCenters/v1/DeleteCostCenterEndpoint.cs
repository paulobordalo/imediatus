using imediatus.Framework.Infrastructure.Auth.Policy;
using imediatus.WebApi.Workspace.Application.CostCenters.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace imediatus.WebApi.Workspace.Infrastructure.Endpoints.CostCenters.v1;
public static class DeleteCostCenterEndpoint
{
    internal static RouteHandlerBuilder MapCostCenterDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteCostCenterCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteCostCenterEndpoint))
            .WithSummary("deletes costcenter by id")
            .WithDescription("deletes costcenter by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.CostCenters.Delete")
            .MapToApiVersion(1);
    }
}
