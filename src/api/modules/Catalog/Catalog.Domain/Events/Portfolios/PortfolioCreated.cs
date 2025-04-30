using imediatus.Framework.Core.Domain.Events;
using imediatus.WebApi.Catalog.Domain.Models;

namespace imediatus.WebApi.Catalog.Domain.Events.Portfolios;

public sealed record PortfolioCreated : DomainEvent
{
    public Portfolio? Portfolio { get; set; }
}
