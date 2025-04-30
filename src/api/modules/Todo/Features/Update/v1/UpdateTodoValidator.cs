using FluentValidation;
using imediatus.WebApi.Todo.Persistence;

namespace imediatus.WebApi.Todo.Features.Update.v1;
public class UpdateTodoValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoValidator(TodoDbContext context)
    {
        RuleFor(p => p.Title).NotEmpty();
        RuleFor(p => p.Note).NotEmpty();
    }
}
