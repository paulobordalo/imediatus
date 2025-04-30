using System.Collections.ObjectModel;
using imediatus.Framework.Core.Audit;
using MediatR;

namespace imediatus.Framework.Infrastructure.Identity.Audit;
public class AuditPublishedEvent : INotification
{
    public AuditPublishedEvent(Collection<AuditTrail>? trails)
    {
        Trails = trails;
    }
    public Collection<AuditTrail>? Trails { get; }
}
