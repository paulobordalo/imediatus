using imediatus.Framework.Core.Paging;
using imediatus.Framework.Core.Persistence;
using imediatus.WebApi.Catalog.Application.Brands.Get.v1;
using imediatus.WebApi.Catalog.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace imediatus.WebApi.Catalog.Application.Brands.Search.v1;
public sealed class SearchBrandsHandler(
    [FromKeyedServices("catalog:brands")] IReadRepository<Brand> repository)
    : IRequestHandler<SearchBrandsCommand, PagedList<BrandResponse>>
{
    public async Task<PagedList<BrandResponse>> Handle(SearchBrandsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchBrandSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<BrandResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}
