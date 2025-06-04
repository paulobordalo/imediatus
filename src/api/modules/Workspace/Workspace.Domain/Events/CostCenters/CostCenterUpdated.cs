using imediatus.Framework.Core.Domain.Events;
using imediatus.WebApi.Workspace.Domain.Models;

namespace imediatus.WebApi.Workspace.Domain.Events.CostCenters;

public sealed record CostCenterUpdated : DomainEvent
{
    public CostCenter? CostCenter { get; set; }
}
