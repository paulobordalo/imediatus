using imediatus.Shared.Enums;
using MediatR;
using System.ComponentModel;

namespace imediatus.WebApi.Catalog.Application.Portfolios.Create.v1;

public sealed record CreatePortfolioCommand(
    [property: DefaultValue(null)] string? Summary,
    [property: DefaultValue(null)] int? StatusId,
    [property: DefaultValue(null)] int? ClassificationId,
    [property: DefaultValue(null)] int? PriorityId,
    [property: DefaultValue(null)] Guid? AssigneeId,
    [property: DefaultValue(null)] Guid? ReporterId) : IRequest<CreatePortfolioResponse>;
