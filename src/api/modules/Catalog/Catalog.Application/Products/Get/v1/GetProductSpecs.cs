using Ardalis.Specification;
using imediatus.WebApi.Catalog.Domain.Models;

namespace imediatus.WebApi.Catalog.Application.Products.Get.v1;

public class GetProductSpecs : Specification<Product, ProductResponse>
{
    public GetProductSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            .Include(p => p.Brand);
    }
}
