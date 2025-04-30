using Ardalis.Specification;
using imediatus.Framework.Core.Paging;
using imediatus.Framework.Core.Specifications;
using imediatus.WebApi.Catalog.Application.Brands.Get.v1;
using imediatus.WebApi.Catalog.Domain.Models;

namespace imediatus.WebApi.Catalog.Application.Brands.Search.v1;
public class SearchBrandSpecs : EntitiesByBaseFilterSpec<Brand, BrandResponse>
{
    public SearchBrandSpecs(SearchBrandsCommand command) 
        : base(command) =>
        Query
            .OrderBy(c => c.Name, !command.HasOrderBy())
            .Where(b => b.Name.Contains(command.Keyword ?? string.Empty), !string.IsNullOrEmpty(command.Keyword));
}
