using Application.Abstractions.Authentication;
using Application.Tests.L0.Infrastructure;
using Application.Todos.GetById;
using Domain.Todos;
using Domain.Users;

namespace Application.Tests.L0.Todos.GetById;

public class GetTodoByIdQueryHandlerTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_TodoItemDoesNotExist()
    {
        using var context = TestDbContext.Create();
        _userContext.UserId.Returns(Guid.NewGuid());

        var query = new GetTodoByIdQuery(Guid.NewGuid());
        var handler = new GetTodoByIdQueryHandler(context, _userContext);

        Result<TodoResponse> result = await handler.Handle(query, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("TodoItems.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_TodoBelongsToAnotherUser()
    {
        using var context = TestDbContext.Create();
        var todoId = Guid.NewGuid();
        var ownerUserId = Guid.NewGuid();
        _userContext.UserId.Returns(Guid.NewGuid());

        context.TodoItems.Add(new TodoItem { Id = todoId, UserId = ownerUserId, Description = "Other user todo", Priority = Priority.Low, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var query = new GetTodoByIdQuery(todoId);
        var handler = new GetTodoByIdQueryHandler(context, _userContext);

        Result<TodoResponse> result = await handler.Handle(query, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("TodoItems.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnTodoResponse_When_TodoExists()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var todoId = Guid.NewGuid();
        DateTime createdAt = new(2025, 3, 10, 8, 0, 0, DateTimeKind.Utc);
        _userContext.UserId.Returns(userId);

        context.TodoItems.Add(new TodoItem
        {
            Id = todoId,
            UserId = userId,
            Description = "Learn EF Core",
            Priority = Priority.High,
            IsCompleted = false,
            Labels = ["study"],
            CreatedAt = createdAt
        });
        await context.SaveChangesAsync();

        var query = new GetTodoByIdQuery(todoId);
        var handler = new GetTodoByIdQueryHandler(context, _userContext);

        Result<TodoResponse> result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(todoId);
        result.Value.UserId.ShouldBe(userId);
        result.Value.Description.ShouldBe("Learn EF Core");
        result.Value.IsCompleted.ShouldBeFalse();
        result.Value.Labels.ShouldContain("study");
    }

    [Fact]
    public async Task Handle_Should_ReturnCompletedTodoWithCompletedAt_When_TodoIsCompleted()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var todoId = Guid.NewGuid();
        DateTime completedAt = new(2025, 4, 1, 14, 30, 0, DateTimeKind.Utc);
        _userContext.UserId.Returns(userId);

        context.TodoItems.Add(new TodoItem
        {
            Id = todoId,
            UserId = userId,
            Description = "Completed task",
            Priority = Priority.Medium,
            IsCompleted = true,
            CompletedAt = completedAt,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var query = new GetTodoByIdQuery(todoId);
        var handler = new GetTodoByIdQueryHandler(context, _userContext);

        Result<TodoResponse> result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.IsCompleted.ShouldBeTrue();
        result.Value.CompletedAt.ShouldBe(completedAt);
    }
}
