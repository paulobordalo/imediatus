using Carter;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Infrastructure.Persistence;
using imediatus.WebApi.Todo.Domain;
using imediatus.WebApi.Todo.Features.Create.v1;
using imediatus.WebApi.Todo.Features.Delete.v1;
using imediatus.WebApi.Todo.Features.Get.v1;
using imediatus.WebApi.Todo.Features.GetList.v1;
using imediatus.WebApi.Todo.Features.Update.v1;
using imediatus.WebApi.Todo.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace imediatus.WebApi.Todo;
public static class TodoModule
{

    public class Endpoints : CarterModule
    {
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var todoGroup = app.MapGroup("todos").WithTags("todos");
            todoGroup.MapTodoItemCreationEndpoint();
            todoGroup.MapGetTodoEndpoint();
            todoGroup.MapGetTodoListEndpoint();
            todoGroup.MapTodoItemUpdationEndpoint();
            todoGroup.MapTodoItemDeletionEndpoint();
        }
    }
    public static WebApplicationBuilder RegisterTodoServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<TodoDbContext>();
        builder.Services.AddScoped<IDbInitializer, TodoDbInitializer>();
        builder.Services.AddKeyedScoped<IRepository<TodoItem>, TodoRepository<TodoItem>>("todo");
        builder.Services.AddKeyedScoped<IReadRepository<TodoItem>, TodoRepository<TodoItem>>("todo");
        return builder;
    }
    public static WebApplication UseTodoModule(this WebApplication app)
    {
        return app;
    }
}
