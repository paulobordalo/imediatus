﻿using imediatus.Framework.Core.Paging;
using imediatus.Framework.Infrastructure.Auth.Policy;
using imediatus.WebApi.Catalog.Application.Brands.Get.v1;
using imediatus.WebApi.Catalog.Application.Brands.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace imediatus.WebApi.Catalog.Infrastructure.Endpoints.Brands.v1;

public static class SearchBrandsEndpoint
{
    internal static RouteHandlerBuilder MapGetBrandListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchBrandsCommand command) =>
            {
                PagedList<BrandResponse> response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchBrandsEndpoint))
            .WithSummary("Gets a list of brands")
            .WithDescription("Gets a list of brands with pagination and filtering support")
            .Produces<PagedList<BrandResponse>>()
            .RequirePermission("Permissions.Brands.View")
            .MapToApiVersion(1);
    }
}
