using imediatus.Framework.Infrastructure.Auth.Policy;
using imediatus.WebApi.Catalog.Application.Brands.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace imediatus.WebApi.Catalog.Infrastructure.Endpoints.Brands.v1;
public static class DeleteBrandEndpoint
{
    internal static RouteHandlerBuilder MapBrandDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteBrandCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteBrandEndpoint))
            .WithSummary("deletes brand by id")
            .WithDescription("deletes brand by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.Brands.Delete")
            .MapToApiVersion(1);
    }
}
