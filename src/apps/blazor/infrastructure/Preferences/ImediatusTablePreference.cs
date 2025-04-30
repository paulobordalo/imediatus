using imediatus.Shared.Notifications;

namespace imediatus.Blazor.Infrastructure.Preferences;

public class ImediatusTablePreference : INotificationMessage
{
    public bool IsDense { get; set; }
    public bool IsStriped { get; set; }
    public bool HasBorder { get; set; }
    public bool IsHoverable { get; set; }
}
