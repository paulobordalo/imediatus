using imediatus.Framework.Core.Domain.Events;
using imediatus.WebApi.Catalog.Domain.Models;

namespace imediatus.WebApi.Catalog.Domain.Events.Products;
public sealed record ProductUpdated : DomainEvent
{
    public Product? Product { get; set; }
}
