using imediatus.Framework.Core.Domain;
using imediatus.Framework.Core.Domain.Contracts;
using imediatus.WebApi.Todo.Domain.Events;

namespace imediatus.WebApi.Todo.Domain;

public sealed class TodoItem : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;

    private TodoItem() { }

    private TodoItem(string title, string note)
    {
        Title = title;
        Note = note;
        QueueDomainEvent(new TodoItemCreated(Id, Title, Note));
        TodoMetrics.Created.Add(1);
    }

    public static TodoItem Create(string title, string note) => new(title, note);

    public TodoItem Update(string? title, string? note)
    {
        bool isUpdated = false;

        if (!string.IsNullOrWhiteSpace(title) && !string.Equals(Title, title, StringComparison.OrdinalIgnoreCase))
        {
            Title = title;
            isUpdated = true;
        }

        if (!string.IsNullOrWhiteSpace(note) && !string.Equals(Note, note, StringComparison.OrdinalIgnoreCase))
        {
            Note = note;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new TodoItemUpdated(this));
        }

        return this;
    }
}
