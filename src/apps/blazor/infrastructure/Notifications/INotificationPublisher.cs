using imediatus.Shared.Notifications;

namespace imediatus.Blazor.Infrastructure.Notifications;

public interface INotificationPublisher
{
    Task PublishAsync(INotificationMessage notification);
}
