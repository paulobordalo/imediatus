using imediatus.Framework.Core.Domain.Events;
using imediatus.WebApi.Catalog.Domain.Models;

namespace imediatus.WebApi.Catalog.Domain.Events.Brands;

public sealed record BrandUpdated : DomainEvent
{
    public Brand? Brand { get; set; }
}
