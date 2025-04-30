
using imediatus.Framework.Core.Caching;
using imediatus.Framework.Core.Domain.Events;
using imediatus.WebApi.Todo.Features.Get.v1;
using MediatR;
using Microsoft.Extensions.Logging;

namespace imediatus.WebApi.Todo.Domain.Events;
public record TodoItemUpdated(TodoItem item) : DomainEvent;

public class TodoItemUpdatedEventHandler(
    ILogger<TodoItemUpdatedEventHandler> logger,
    ICacheService cache)
    : INotificationHandler<TodoItemUpdated>
{
    public async Task Handle(TodoItemUpdated notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("handling todo item update domain event..");
        var cacheResponse = new GetTodoResponse(notification.item.Id, notification.item.Title, notification.item.Note);
        await cache.SetAsync($"todo:{notification.item.Id}", cacheResponse, cancellationToken: cancellationToken);
    }
}
