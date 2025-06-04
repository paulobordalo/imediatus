using imediatus.Framework.Core.Domain.Events;
using imediatus.WebApi.Workspace.Domain.Models;

namespace imediatus.WebApi.Workspace.Domain.Events.Portfolios;

public sealed record PortfolioCreated : DomainEvent
{
    public Portfolio? Portfolio { get; set; }
}
