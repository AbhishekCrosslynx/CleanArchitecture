using Application.Abstractions.Authentication;
using Application.Tests.L0.Infrastructure;
using Application.Todos.Create;
using Domain.Todos;
using Domain.Users;

namespace Application.Tests.L0.Todos.Create;

public class CreateTodoCommandHandlerTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    [Fact]
    public async Task Handle_Should_ReturnUnauthorized_When_UserIdDoesNotMatchContext()
    {
        using var context = TestDbContext.Create();
        _userContext.UserId.Returns(Guid.NewGuid());

        var command = new CreateTodoCommand { UserId = Guid.NewGuid(), Description = "Test", Priority = Priority.Low };
        var handler = new CreateTodoCommandHandler(context, _dateTimeProvider, _userContext);

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

        var command = new CreateTodoCommand { UserId = userId, Description = "Test", Priority = Priority.Low };
        var handler = new CreateTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Users.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnTodoId_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        context.Users.Add(new User { Id = userId, Email = "user@test.com", FirstName = "John", LastName = "Doe", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var command = new CreateTodoCommand
        {
            UserId = userId,
            Description = "Buy groceries",
            Priority = Priority.Medium,
            DueDate = DateTime.Today.AddDays(3),
            Labels = ["shopping", "personal"]
        };
        var handler = new CreateTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_Should_PersistTodoItem_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;
        _userContext.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(now);

        context.Users.Add(new User { Id = userId, Email = "user@test.com", FirstName = "Jane", LastName = "Doe", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var command = new CreateTodoCommand
        {
            UserId = userId,
            Description = "Write unit tests",
            Priority = Priority.High,
            Labels = ["work"]
        };
        var handler = new CreateTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        TodoItem? savedTodo = await context.TodoItems.FindAsync(result.Value);
        savedTodo.ShouldNotBeNull();
        savedTodo!.Description.ShouldBe("Write unit tests");
        savedTodo.Priority.ShouldBe(Priority.High);
        savedTodo.UserId.ShouldBe(userId);
        savedTodo.IsCompleted.ShouldBeFalse();
        savedTodo.CreatedAt.ShouldBe(now);
    }

    [Fact]
    public async Task Handle_Should_RaiseTodoItemCreatedDomainEvent_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        context.Users.Add(new User { Id = userId, Email = "user@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var command = new CreateTodoCommand { UserId = userId, Description = "Test", Priority = Priority.Low };
        var handler = new CreateTodoCommandHandler(context, _dateTimeProvider, _userContext);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        TodoItem? todo = await context.TodoItems.FindAsync(result.Value);
        todo.ShouldNotBeNull();
        todo!.DomainEvents.ShouldContain(e => e is TodoItemCreatedDomainEvent);
    }
}
