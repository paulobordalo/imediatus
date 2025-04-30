using imediatus.Shared.Notifications;

namespace imediatus.Blazor.Infrastructure.Notifications;

public record ConnectionStateChanged(ConnectionState State, string? Message) : INotificationMessage;
