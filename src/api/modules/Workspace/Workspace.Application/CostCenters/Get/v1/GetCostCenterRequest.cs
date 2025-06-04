using MediatR;

namespace imediatus.WebApi.Workspace.Application.CostCenters.Get.v1;

public class GetCostCenterRequest : IRequest<CostCenterResponse>
{
    public Guid Id { get; set; }
    public GetCostCenterRequest(Guid id) => Id = id;
}
