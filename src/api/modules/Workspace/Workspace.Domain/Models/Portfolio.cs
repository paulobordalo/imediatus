using imediatus.Framework.Core.Domain.Contracts;
using imediatus.Framework.Core.Domain;
using imediatus.WebApi.Workspace.Domain.Events.Portfolios;
using imediatus.Shared.Enums;

namespace imediatus.WebApi.Workspace.Domain.Models;

public class Portfolio : AuditableEntity, IAggregateRoot
{
    public string? Summary { get; private set; } = string.Empty;
    public int SecondaryKey { get; private set; } = default!;
    public int StatusId { get; private set; } = default!;
    public int ClassificationId { get; private set; } = default!;
    public int PriorityId { get; private set; } = default!;
    public Guid? CostCenterId { get; private set; } = default!;
    public Guid? AssigneeId { get; private set; } = default!;
    public Guid ReporterId { get; private set; }


    private Portfolio() { }

    private Portfolio(Guid id, string? summary, int statusId, int classificationId, int priorityId, Guid? costCenterId, Guid? assigneeId, Guid reporterId)
    {
        Id = id;
        Summary = summary;
        StatusId = statusId;
        ClassificationId = classificationId;
        PriorityId = priorityId;
        CostCenterId = costCenterId;
        AssigneeId = assigneeId;
        ReporterId = reporterId;

        QueueDomainEvent(new PortfolioCreated { Portfolio = this });
    }

    public static Portfolio Create(string? summary, int statusId, int classificationId, int priorityId, Guid? costCenterId, Guid? assigneeId, Guid reporterId)
    {
        return new Portfolio(Guid.NewGuid(), summary, statusId, classificationId, priorityId, costCenterId, assigneeId, reporterId);
    }

    public Portfolio Update(string? summary, int? statusId, int? classificationId, int? priorityId, Guid? costCenterId, Guid? assigneeId, Guid? reporterId)
    {
        bool isUpdated = false;

        if (!string.IsNullOrWhiteSpace(summary) && !string.Equals(Summary, summary, StringComparison.OrdinalIgnoreCase))
        {
            Summary = summary;
            isUpdated = true;
        }

        if (statusId.HasValue && StatusId != statusId.Value)
        {
            StatusId = statusId.Value;
            isUpdated = true;
        }

        if (classificationId.HasValue && ClassificationId != classificationId.Value)
        {
            ClassificationId = classificationId.Value;
            isUpdated = true;
        }

        if (priorityId.HasValue && PriorityId != priorityId.Value)
        {
            PriorityId = priorityId.Value;
            isUpdated = true;
        }

        if (costCenterId.HasValue && CostCenterId != costCenterId.Value)
        {
            CostCenterId = costCenterId.Value;
            isUpdated = true;
        }

        if (assigneeId.HasValue && AssigneeId != assigneeId.Value)
        {
            AssigneeId = assigneeId.Value;
            isUpdated = true;
        }

        if (reporterId.HasValue && ReporterId != reporterId.Value)
        {
            ReporterId = reporterId.Value;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new PortfolioUpdated { Portfolio = this });
        }

        return this;
    }
}
