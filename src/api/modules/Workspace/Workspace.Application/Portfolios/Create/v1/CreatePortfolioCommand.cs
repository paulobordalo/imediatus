using System.Collections.ObjectModel;
using System.ComponentModel;
using imediatus.WebApi.Workspace.Domain.Models;
using MediatR;

namespace imediatus.WebApi.Workspace.Application.Portfolios.Create.v1;

public sealed record CreatePortfolioCommand(
    [property: DefaultValue(null)] string? Summary,
    [property: DefaultValue(null)] int? StatusId,
    [property: DefaultValue(null)] int? ClassificationId,
    [property: DefaultValue(null)] int? PriorityId,
    [property: DefaultValue(null)] Guid? CostCenterId,
    [property: DefaultValue(null)] Guid? AssigneeId,
    [property: DefaultValue(null)] Guid? ReporterId,
    [property: DefaultValue(null)] Collection<PortfolioAttachment>? Attachments) : IRequest<CreatePortfolioResponse>;
