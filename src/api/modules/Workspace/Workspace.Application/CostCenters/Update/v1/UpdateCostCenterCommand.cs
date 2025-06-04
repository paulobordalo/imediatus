using System.ComponentModel;
using MediatR;

namespace imediatus.WebApi.Workspace.Application.CostCenters.Update.v1;
public sealed record UpdateCostCenterCommand(
    Guid Id,
    [property: DefaultValue(null)] string Name) : IRequest<UpdateCostCenterResponse>;
