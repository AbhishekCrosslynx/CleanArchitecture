using Application.Abstractions.Authentication;
using Application.Tests.L0.Infrastructure;
using Application.Todos.Get;
using Domain.Todos;
using Domain.Users;

namespace Application.Tests.L0.Todos.Get;

public class GetTodosQueryHandlerTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    [Fact]
    public async Task Handle_Should_ReturnUnauthorized_When_UserIdDoesNotMatchContext()
    {
        using var context = TestDbContext.Create();
        _userContext.UserId.Returns(Guid.NewGuid());

        var query = new GetTodosQuery(Guid.NewGuid());
        var handler = new GetTodosQueryHandler(context, _userContext);

        Result<List<TodoResponse>> result = await handler.Handle(query, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Users.Unauthorized");
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_When_UserHasNoTodos()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        var query = new GetTodosQuery(userId);
        var handler = new GetTodosQueryHandler(context, _userContext);

        Result<List<TodoResponse>> result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnOnlyUserTodos_When_MultipleUsersHaveTodos()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        context.TodoItems.Add(new TodoItem { Id = Guid.NewGuid(), UserId = userId, Description = "My todo 1", Priority = Priority.Low, CreatedAt = DateTime.UtcNow });
        context.TodoItems.Add(new TodoItem { Id = Guid.NewGuid(), UserId = userId, Description = "My todo 2", Priority = Priority.Medium, CreatedAt = DateTime.UtcNow });
        context.TodoItems.Add(new TodoItem { Id = Guid.NewGuid(), UserId = otherUserId, Description = "Other's todo", Priority = Priority.Low, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var query = new GetTodosQuery(userId);
        var handler = new GetTodosQueryHandler(context, _userContext);

        Result<List<TodoResponse>> result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
        result.Value.ShouldAllBe(t => t.UserId == userId);
    }

    [Fact]
    public async Task Handle_Should_ReturnCorrectTodoData_When_TodosExist()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var todoId = Guid.NewGuid();
        DateTime now = new(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        _userContext.UserId.Returns(userId);

        context.TodoItems.Add(new TodoItem
        {
            Id = todoId,
            UserId = userId,
            Description = "Buy milk",
            Priority = Priority.Low,
            IsCompleted = false,
            Labels = ["shopping"],
            CreatedAt = now
        });
        await context.SaveChangesAsync();

        var query = new GetTodosQuery(userId);
        var handler = new GetTodosQueryHandler(context, _userContext);

        Result<List<TodoResponse>> result = await handler.Handle(query, CancellationToken.None);

        TodoResponse todo = result.Value[0];
        todo.Id.ShouldBe(todoId);
        todo.Description.ShouldBe("Buy milk");
        todo.IsCompleted.ShouldBeFalse();
        todo.Labels.ShouldContain("shopping");
    }
}
