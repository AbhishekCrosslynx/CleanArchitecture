using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Update;

internal sealed class UpdateTodoCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<UpdateTodoCommand>
{
    public async Task<Result> Handle(UpdateTodoCommand command, CancellationToken cancellationToken)
    {
        TodoItem? todoItem = await context.TodoItems
            .SingleOrDefaultAsync(t => t.Id == command.TodoItemId, cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure(TodoItemErrors.NotFound(command.TodoItemId));
        }

        // Update all fields from command
        todoItem.Description = command.Request.Description;
        todoItem.DueDate = command.Request.DueDate;
        todoItem.Labels = command.Request.Labels ?? new List<string>();
        todoItem.Priority = command.Request.Priority;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
