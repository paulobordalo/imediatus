using imediatus.Framework.Core.Exceptions;

namespace imediatus.WebApi.Catalog.Domain.Exceptions;
public sealed class PortfolioNotFoundException : NotFoundException
{
    public PortfolioNotFoundException(Guid id)
        : base($"portfolio with id {id} not found")
    {
    }
}
