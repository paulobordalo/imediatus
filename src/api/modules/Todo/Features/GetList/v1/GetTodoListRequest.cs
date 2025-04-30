using imediatus.Framework.Core.Paging;
using MediatR;

namespace imediatus.WebApi.Todo.Features.GetList.v1;
public record GetTodoListRequest(PaginationFilter Filter) : IRequest<PagedList<TodoDto>>;
