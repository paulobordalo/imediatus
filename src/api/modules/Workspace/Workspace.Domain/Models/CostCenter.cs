using imediatus.Framework.Core.Domain.Contracts;
using imediatus.Framework.Core.Domain;
using imediatus.WebApi.Workspace.Domain.Events.CostCenters;

namespace imediatus.WebApi.Workspace.Domain.Models;

public class CostCenter : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;

    private CostCenter() { }

    private CostCenter(Guid id, string name)
    {
        Id = id;
        Name = name;

        QueueDomainEvent(new CostCenterCreated { CostCenter = this });
    }

    public static CostCenter Create(string name)
    {
        return new CostCenter(Guid.NewGuid(), name);
    }

    public CostCenter Update(string name)
    {
        bool isUpdated = false;

        if (!string.IsNullOrWhiteSpace(name) && !string.Equals(Name, name, StringComparison.OrdinalIgnoreCase))
        {
            Name = name;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new CostCenterUpdated { CostCenter = this });
        }

        return this;
    }
}
