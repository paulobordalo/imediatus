using System.ComponentModel;
using MediatR;

namespace imediatus.WebApi.Catalog.Application.Portfolios.Update.v1;
public sealed record UpdatePortfolioCommand(
    Guid Id,
    [property: DefaultValue(null)] string? Summary,
    int StatusId,
    int ClassificationId,
    int PriorityId,
    [property: DefaultValue(null)] Guid? AssigneeId,
    Guid ReporterId) : IRequest<UpdatePortfolioResponse>;
