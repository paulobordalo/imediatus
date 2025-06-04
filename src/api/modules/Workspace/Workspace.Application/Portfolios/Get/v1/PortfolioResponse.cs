namespace imediatus.WebApi.Workspace.Application.Portfolios.Get.v1;

public sealed record PortfolioResponse(Guid? Id, string? Summary, int SecondaryKey, int StatusId, int ClassificationId, int PriorityId, Guid? CostCenterId, Guid? AssigneeId, Guid ReporterId);
