using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedContracts.DTOs.Todos.Responses;
using SharedKernel;

namespace Application.Todos.Create;

internal sealed class CreateTodoCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext)
    : ICommandHandler<CreateTodoCommand, TodoResponse>
{
    public async Task<Result<TodoResponse>> Handle(
        CreateTodoCommand command,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        User? user = await context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(
                u => u.Id == userId,
                cancellationToken);

        if (user is null)
        {
            return Result.Failure<TodoResponse>(
                UserErrors.NotFound(userId));
        }

        var todoItem = new TodoItem
        {
            UserId = userId,
            Description = command.Description,
            Priority = command.Priority,
            DueDate = command.DueDate,
            Labels = command.Labels,
            IsCompleted = false,
            CreatedAt = dateTimeProvider.UtcNow
        };

        todoItem.Raise(new TodoItemCreatedDomainEvent(todoItem.Id));

        context.TodoItems.Add(todoItem);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new TodoResponse(
            todoItem.Id,
            todoItem.UserId,
            todoItem.Description,
            todoItem.DueDate,
            todoItem.Labels,
            todoItem.IsCompleted,
            todoItem.CreatedAt,
            todoItem.CompletedAt,
            todoItem.Priority
        ));
    }
}
