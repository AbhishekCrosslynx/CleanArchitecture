using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using SharedContracts.DTOs.Todos.Responses;
using SharedKernel;

namespace Application.Todos.Copy;

internal sealed class CopyTodoCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext)
    : ICommandHandler<CopyTodoCommand, TodoResponse>
{
    public async Task<Result<TodoResponse>> Handle(CopyTodoCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        TodoItem? existingTodo = await context.TodoItems
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Id == command.TodoId && t.UserId == userId, cancellationToken);

        if (existingTodo is null)
        {
            return Result.Failure<TodoResponse>(TodoItemErrors.NotFound(command.TodoId));
        }
        var copiedTodo = new TodoItem
        {
            UserId = userId,
            Description = existingTodo.Description,
            Priority = existingTodo.Priority,
            DueDate = existingTodo.DueDate,
            Labels = existingTodo.Labels.ToList(),
            IsCompleted = false, // reset completion
            CreatedAt = dateTimeProvider.UtcNow
        };

        copiedTodo.Raise(new TodoItemCreatedDomainEvent(copiedTodo.Id));

        context.TodoItems.Add(copiedTodo);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new TodoResponse(
            copiedTodo.Id,
            copiedTodo.UserId,
            copiedTodo.Description,
            copiedTodo.DueDate,
            copiedTodo.Labels,
            copiedTodo.IsCompleted,
            copiedTodo.CreatedAt,
            copiedTodo.CompletedAt,
            copiedTodo.Priority
        ));
    }
}
