using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedContracts.DTOs.Todos.Responses;
using SharedKernel;

namespace Application.Todos.Get;

internal sealed class GetTodosQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetTodosQuery, List<TodoResponse>>
{
    public async Task<Result<List<TodoResponse>>> Handle(GetTodosQuery query, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        List<TodoResponse> todos = await context.TodoItems
            .Where(todoItem => todoItem.UserId == userId)
            .Select(todoItem => new TodoResponse(
                todoItem.Id,
                todoItem.UserId,
                todoItem.Description,
                todoItem.DueDate,
                todoItem.Labels,
                todoItem.IsCompleted,
                todoItem.CreatedAt,
                todoItem.CompletedAt,
                todoItem.Priority
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(todos);
    }
}
