using Application.Abstractions.Authentication;
using Application.Tests.L0.Infrastructure;
using Application.Todos.Complete;
using Domain.Todos;

namespace Application.Tests.L0.Todos.Complete;

public class CompleteTodoCommandHandlerTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_TodoItemDoesNotExist()
    {
        using var context = TestDbContext.Create();
        _userContext.UserId.Returns(Guid.NewGuid());

        var command = new CompleteTodoCommand(Guid.NewGuid());
        var handler = new CompleteTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("TodoItems.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_TodoBelongsToAnotherUser()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var todoId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        context.TodoItems.Add(new TodoItem { Id = todoId, UserId = otherUserId, Description = "Other's todo", Priority = Priority.Low, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var command = new CompleteTodoCommand(todoId);
        var handler = new CompleteTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("TodoItems.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnAlreadyCompleted_When_TodoIsAlreadyCompleted()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var todoId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        context.TodoItems.Add(new TodoItem { Id = todoId, UserId = userId, Description = "Done todo", Priority = Priority.Low, IsCompleted = true, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var command = new CompleteTodoCommand(todoId);
        var handler = new CompleteTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("TodoItems.AlreadyCompleted");
    }

    [Fact]
    public async Task Handle_Should_MarkTodoAsCompleted_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var todoId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;
        _userContext.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(now);

        context.TodoItems.Add(new TodoItem { Id = todoId, UserId = userId, Description = "Complete me", Priority = Priority.Medium, IsCompleted = false, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var command = new CompleteTodoCommand(todoId);
        var handler = new CompleteTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();

        TodoItem? saved = await context.TodoItems.FindAsync(todoId);
        saved.ShouldNotBeNull();
        saved!.IsCompleted.ShouldBeTrue();
        saved.CompletedAt.ShouldBe(now);
    }

    [Fact]
    public async Task Handle_Should_RaiseTodoItemCompletedDomainEvent_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var todoId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        context.TodoItems.Add(new TodoItem { Id = todoId, UserId = userId, Description = "Complete me", Priority = Priority.Low, IsCompleted = false, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var command = new CompleteTodoCommand(todoId);
        var handler = new CompleteTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();

        TodoItem? todo = await context.TodoItems.FindAsync(todoId);
        todo.ShouldNotBeNull();
        todo!.DomainEvents.ShouldContain(e => e is TodoItemCompletedDomainEvent);
    }
}
