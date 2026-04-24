using Application.Abstractions.Behaviors;
using Application.Abstractions.Messaging;
using Application.Todos.Create;
using Application.Todos.Delete;
using Application.Todos.GetById;
using Domain.Todos;
using Microsoft.Extensions.Logging.Abstractions;

namespace Application.Tests.L0.Abstractions.Behaviors;

public class LoggingDecoratorTests
{
    // ── CommandHandler<TCommand, TResponse> ─────────────────────────────────────

    [Fact]
    public async Task CommandHandler_Should_ReturnSuccessResult_When_InnerHandlerSucceeds()
    {
        ICommandHandler<CreateTodoCommand, Guid> inner = Substitute.For<ICommandHandler<CreateTodoCommand, Guid>>();
        var command = new CreateTodoCommand { UserId = Guid.NewGuid(), Description = "Test", Priority = Priority.Low };
        var todoId = Guid.NewGuid();
        inner.Handle(command, Arg.Any<CancellationToken>()).Returns(Result.Success(todoId));

        var handler = new LoggingDecorator.CommandHandler<CreateTodoCommand, Guid>(
            inner,
            NullLogger<LoggingDecorator.CommandHandler<CreateTodoCommand, Guid>>.Instance);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(todoId);
        await inner.Received(1).Handle(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CommandHandler_Should_ReturnFailureResult_When_InnerHandlerFails()
    {
        ICommandHandler<CreateTodoCommand, Guid> inner = Substitute.For<ICommandHandler<CreateTodoCommand, Guid>>();
        var command = new CreateTodoCommand { UserId = Guid.NewGuid(), Description = "Test" };
        var error = Error.NotFound("User.NotFound", "User not found");
        inner.Handle(command, Arg.Any<CancellationToken>()).Returns(Result.Failure<Guid>(error));

        var handler = new LoggingDecorator.CommandHandler<CreateTodoCommand, Guid>(
            inner,
            NullLogger<LoggingDecorator.CommandHandler<CreateTodoCommand, Guid>>.Instance);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    // ── CommandBaseHandler<TCommand> ────────────────────────────────────────────

    [Fact]
    public async Task CommandBaseHandler_Should_ReturnSuccessResult_When_InnerHandlerSucceeds()
    {
        ICommandHandler<DeleteTodoCommand> inner = Substitute.For<ICommandHandler<DeleteTodoCommand>>();
        var command = new DeleteTodoCommand(Guid.NewGuid());
        inner.Handle(command, Arg.Any<CancellationToken>()).Returns(Result.Success());

        var handler = new LoggingDecorator.CommandBaseHandler<DeleteTodoCommand>(
            inner,
            NullLogger<LoggingDecorator.CommandBaseHandler<DeleteTodoCommand>>.Instance);

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await inner.Received(1).Handle(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CommandBaseHandler_Should_ReturnFailureResult_When_InnerHandlerFails()
    {
        ICommandHandler<DeleteTodoCommand> inner = Substitute.For<ICommandHandler<DeleteTodoCommand>>();
        var command = new DeleteTodoCommand(Guid.NewGuid());
        var error = Error.NotFound("Todo.NotFound", "Todo not found");
        inner.Handle(command, Arg.Any<CancellationToken>()).Returns(Result.Failure(error));

        var handler = new LoggingDecorator.CommandBaseHandler<DeleteTodoCommand>(
            inner,
            NullLogger<LoggingDecorator.CommandBaseHandler<DeleteTodoCommand>>.Instance);

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    // ── QueryHandler<TQuery, TResponse> ─────────────────────────────────────────

    [Fact]
    public async Task QueryHandler_Should_ReturnSuccessResult_When_InnerHandlerSucceeds()
    {
        IQueryHandler<GetTodoByIdQuery, TodoResponse> inner = Substitute.For<IQueryHandler<GetTodoByIdQuery, TodoResponse>>();
        var query = new GetTodoByIdQuery(Guid.NewGuid());
        var response = new TodoResponse { Id = Guid.NewGuid(), Description = "Test" };
        inner.Handle(query, Arg.Any<CancellationToken>()).Returns(Result.Success(response));

        var handler = new LoggingDecorator.QueryHandler<GetTodoByIdQuery, TodoResponse>(
            inner,
            NullLogger<LoggingDecorator.QueryHandler<GetTodoByIdQuery, TodoResponse>>.Instance);

        Result<TodoResponse> result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(response);
    }

    [Fact]
    public async Task QueryHandler_Should_ReturnFailureResult_When_InnerHandlerFails()
    {
        IQueryHandler<GetTodoByIdQuery, TodoResponse> inner = Substitute.For<IQueryHandler<GetTodoByIdQuery, TodoResponse>>();
        var query = new GetTodoByIdQuery(Guid.NewGuid());
        var error = Error.NotFound("Todo.NotFound", "Not found");
        inner.Handle(query, Arg.Any<CancellationToken>()).Returns(Result.Failure<TodoResponse>(error));

        var handler = new LoggingDecorator.QueryHandler<GetTodoByIdQuery, TodoResponse>(
            inner,
            NullLogger<LoggingDecorator.QueryHandler<GetTodoByIdQuery, TodoResponse>>.Instance);

        Result<TodoResponse> result = await handler.Handle(query, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }
}
