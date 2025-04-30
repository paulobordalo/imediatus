namespace imediatus.WebApi.Catalog.Application.Portfolios.Get.v1;

public sealed record PortfolioResponse(Guid? Id, string? Summary, int SecondaryKey, int StatusId, int ClassificationId, int PriorityId, Guid? AssigneeId, Guid ReporterId);
