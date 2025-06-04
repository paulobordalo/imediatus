using MediatR;
using System.ComponentModel;

namespace imediatus.WebApi.Workspace.Application.CostCenters.Create.v1;

public sealed record CreateCostCenterCommand(
    [property: DefaultValue(null)] string Name) : IRequest<CreateCostCenterResponse>;
