using MediatR;

namespace imediatus.WebApi.Catalog.Application.Brands.Delete.v1;
public sealed record DeleteBrandCommand(
    Guid Id) : IRequest;
