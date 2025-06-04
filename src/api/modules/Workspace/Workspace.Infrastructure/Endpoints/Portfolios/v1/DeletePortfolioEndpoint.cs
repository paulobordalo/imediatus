using imediatus.Framework.Infrastructure.Auth.Policy;
using imediatus.WebApi.Workspace.Application.Portfolios.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace imediatus.WebApi.Workspace.Infrastructure.Endpoints.Portfolios.v1;
public static class DeletePortfolioEndpoint
{
    internal static RouteHandlerBuilder MapPortfolioDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeletePortfolioCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeletePortfolioEndpoint))
            .WithSummary("deletes portfolio by id")
            .WithDescription("deletes portfolio by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.Portfolios.Delete")
            .MapToApiVersion(1);
    }
}
