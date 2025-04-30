using imediatus.Framework.Core.Exceptions;

namespace imediatus.WebApi.Todo.Exceptions;
internal sealed class TodoItemNotFoundException : NotFoundException
{
    public TodoItemNotFoundException(Guid id)
        : base($"todo item with id {id} not found")
    {
    }
}
