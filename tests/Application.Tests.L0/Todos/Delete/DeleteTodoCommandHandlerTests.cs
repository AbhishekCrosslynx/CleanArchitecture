using Application.Abstractions.Authentication;
using Application.Tests.L0.Infrastructure;
using Application.Todos.Delete;
using Domain.Todos;
using Domain.Users;

namespace Application.Tests.L0.Todos.Delete;

public class DeleteTodoCommandHandlerTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_TodoItemDoesNotExist()
    {
        using var context = TestDbContext.Create();
        _userContext.UserId.Returns(Guid.NewGuid());

        var handler = new DeleteTodoCommandHandler(context, _userContext);
        var command = new DeleteTodoCommand(Guid.NewGuid());

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("TodoItems.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_TodoBelongsToAnotherUser()
    {
        using var context = TestDbContext.Create();
        var todoId = Guid.NewGuid();
        var ownerUserId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();
        _userContext.UserId.Returns(requestingUserId);

        context.TodoItems.Add(new TodoItem { Id = todoId, UserId = ownerUserId, Description = "Todo", Priority = Priority.Low, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var handler = new DeleteTodoCommandHandler(context, _userContext);
        var command = new DeleteTodoCommand(todoId);

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("TodoItems.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var todoId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        context.TodoItems.Add(new TodoItem { Id = todoId, UserId = userId, Description = "Todo", Priority = Priority.Low, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var handler = new DeleteTodoCommandHandler(context, _userContext);
        var command = new DeleteTodoCommand(todoId);

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_Should_RemoveTodoFromDatabase_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var todoId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        context.TodoItems.Add(new TodoItem { Id = todoId, UserId = userId, Description = "To delete", Priority = Priority.Low, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var handler = new DeleteTodoCommandHandler(context, _userContext);

        await handler.Handle(new DeleteTodoCommand(todoId), CancellationToken.None);

        TodoItem? deleted = await context.TodoItems.FindAsync(todoId);
        deleted.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_Should_RaiseTodoItemDeletedDomainEvent_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var todoId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        var todoItem = new TodoItem { Id = todoId, UserId = userId, Description = "Todo", Priority = Priority.Low, CreatedAt = DateTime.UtcNow };
        context.TodoItems.Add(todoItem);
        await context.SaveChangesAsync();

        var handler = new DeleteTodoCommandHandler(context, _userContext);

        await handler.Handle(new DeleteTodoCommand(todoId), CancellationToken.None);

        todoItem.DomainEvents.ShouldContain(e => e is TodoItemDeletedDomainEvent);
    }
}
