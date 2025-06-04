using imediatus.Framework.Core.Exceptions;

namespace imediatus.WebApi.Workspace.Domain.Exceptions;
public sealed class CostCenterNotFoundException : NotFoundException
{
    public CostCenterNotFoundException(Guid id)
        : base($"costcenter with id {id} not found")
    {
    }
}
