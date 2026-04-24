using Application.Abstractions.Authentication;
using Application.Tests.L0.Infrastructure;
using Application.Todos.Copy;
using Domain.Todos;
using Domain.Users;

namespace Application.Tests.L0.Todos.Copy;

public class CopyTodoCommandHandlerTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    [Fact]
    public async Task Handle_Should_ReturnUnauthorized_When_UserIdDoesNotMatchContext()
    {
        using var context = TestDbContext.Create();
        _userContext.UserId.Returns(Guid.NewGuid());

        var command = new CopyTodoCommand { UserId = Guid.NewGuid(), TodoId = Guid.NewGuid() };
        var handler = new CopyTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Users.Unauthorized");
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_UserDoesNotExist()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        var command = new CopyTodoCommand { UserId = userId, TodoId = Guid.NewGuid() };
        var handler = new CopyTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Users.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_TodoItemDoesNotExist()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        context.Users.Add(new User { Id = userId, Email = "user@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var command = new CopyTodoCommand { UserId = userId, TodoId = Guid.NewGuid() };
        var handler = new CopyTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

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

        context.Users.Add(new User { Id = userId, Email = "user@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash" });
        context.TodoItems.Add(new TodoItem { Id = todoId, UserId = otherUserId, Description = "Other's todo", Priority = Priority.Low, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var command = new CopyTodoCommand { UserId = userId, TodoId = todoId };
        var handler = new CopyTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("TodoItems.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnNewTodoId_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var originalTodoId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        context.Users.Add(new User { Id = userId, Email = "user@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash" });
        context.TodoItems.Add(new TodoItem { Id = originalTodoId, UserId = userId, Description = "Original todo", Priority = Priority.Medium, Labels = ["tag1"], CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var command = new CopyTodoCommand { UserId = userId, TodoId = originalTodoId };
        var handler = new CopyTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
        result.Value.ShouldNotBe(originalTodoId);
    }

    [Fact]
    public async Task Handle_Should_CopyTodoDetails_And_ResetCompletion_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var originalTodoId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        context.Users.Add(new User { Id = userId, Email = "user@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash" });
        context.TodoItems.Add(new TodoItem
        {
            Id = originalTodoId,
            UserId = userId,
            Description = "Original todo",
            Priority = Priority.High,
            Labels = ["label1", "label2"],
            IsCompleted = true,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var command = new CopyTodoCommand { UserId = userId, TodoId = originalTodoId };
        var handler = new CopyTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        TodoItem? copied = await context.TodoItems.FindAsync(result.Value);
        copied.ShouldNotBeNull();
        copied!.Description.ShouldBe("Original todo");
        copied.Priority.ShouldBe(Priority.High);
        copied.IsCompleted.ShouldBeFalse();
        copied.Labels.ShouldContain("label1");
        copied.Labels.ShouldContain("label2");
    }
}
