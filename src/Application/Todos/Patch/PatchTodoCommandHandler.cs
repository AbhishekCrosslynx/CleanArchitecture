using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Patch;

internal sealed class PatchTodoCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext
) : ICommandHandler<PatchTodoCommand>
{
    public async Task<Result> Handle(PatchTodoCommand command, CancellationToken cancellationToken)
    {
        TodoItem? todoItem = await context.TodoItems
            .SingleOrDefaultAsync(t => t.Id == command.TodoItemId && t.UserId == userContext.UserId, cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure(TodoItemErrors.NotFound(command.TodoItemId));
        }

        // Only update fields that are non-null
        if (command.Request.Description is not null)
        {
            todoItem.Description = command.Request.Description;
        }

        if (command.Request.DueDate.HasValue)
        {
            todoItem.DueDate = command.Request.DueDate.Value;
        }

        if (command.Request.Labels is not null)
        {
            todoItem.Labels = command.Request.Labels;
        }

        if (command.Request.Priority.HasValue)
        {
            todoItem.Priority = command.Request.Priority.Value;
        }

        if (command.Request.IsCompleted.HasValue && command.Request.IsCompleted.Value && !todoItem.IsCompleted)
        {
            todoItem.IsCompleted = true;
            todoItem.CompletedAt = dateTimeProvider.UtcNow;
            todoItem.Raise(new TodoItemCompletedDomainEvent(todoItem.Id));
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
