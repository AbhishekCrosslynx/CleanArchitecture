using Application.Tests.L0.Infrastructure;
using Application.Todos.Update;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Tests.L0.Todos.Update;

public class UpdateTodoCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_TodoItemDoesNotExist()
    {
        using var context = TestDbContext.Create();
        var nonExistentId = Guid.NewGuid();

        var handler = new UpdateTodoCommandHandler(context);
        var command = new UpdateTodoCommand(nonExistentId, "Updated description");

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("TodoItems.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        var todoId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        context.Users.Add(new User { Id = userId, Email = "user@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash" });
        context.TodoItems.Add(new TodoItem { Id = todoId, UserId = userId, Description = "Original", Priority = Priority.Low, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var handler = new UpdateTodoCommandHandler(context);
        var command = new UpdateTodoCommand(todoId, "Updated description");

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_Should_PersistUpdatedDescription_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        var todoId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        context.Users.Add(new User { Id = userId, Email = "user@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash" });
        context.TodoItems.Add(new TodoItem { Id = todoId, UserId = userId, Description = "Original", Priority = Priority.Low, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var handler = new UpdateTodoCommandHandler(context);
        var command = new UpdateTodoCommand(todoId, "New description");

        await handler.Handle(command, CancellationToken.None);

        TodoItem? updated = await context.TodoItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == todoId);
        updated.ShouldNotBeNull();
        updated!.Description.ShouldBe("New description");
    }
}
