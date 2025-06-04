using MediatR;

namespace imediatus.WebApi.Workspace.Application.CostCenters.Delete.v1;
public sealed record DeleteCostCenterCommand(Guid Id) : IRequest;
