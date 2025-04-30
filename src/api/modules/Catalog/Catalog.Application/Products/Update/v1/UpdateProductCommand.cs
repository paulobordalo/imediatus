using MediatR;

namespace imediatus.WebApi.Catalog.Application.Products.Update.v1;
public sealed record UpdateProductCommand(
    Guid Id,
    string? Name,
    decimal Price,
    string? Description = null,
    Guid? BrandId = null) : IRequest<UpdateProductResponse>;
