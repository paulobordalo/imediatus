using Ardalis.Specification;
using imediatus.Framework.Core.Paging;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Core.Specifications;
using imediatus.WebApi.Todo.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace imediatus.WebApi.Todo.Features.GetList.v1;

public sealed class GetTodoListHandler(
    [FromKeyedServices("todo")] IReadRepository<TodoItem> repository)
    : IRequestHandler<GetTodoListRequest, PagedList<TodoDto>>
{
    public async Task<PagedList<TodoDto>> Handle(GetTodoListRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new EntitiesByPaginationFilterSpec<TodoItem, TodoDto>(request.Filter);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<TodoDto>(items, request.Filter.PageNumber, request.Filter.PageSize, totalCount);
    }
}
