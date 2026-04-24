using Application.Abstractions.Behaviors;
using Application.Abstractions.Messaging;
using Application.Todos.Create;
using Application.Todos.Delete;
using Domain.Todos;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Tests.L0.Abstractions.Behaviors;

public class ValidationDecoratorTests
{
    // ── CommandHandler<TCommand, TResponse> (commands that return a value) ──────

    [Fact]
    public async Task CommandHandler_Should_CallInnerHandler_When_NoValidatorsRegistered()
    {
        ICommandHandler<CreateTodoCommand, Guid> inner = Substitute.For<ICommandHandler<CreateTodoCommand, Guid>>();
        var command = new CreateTodoCommand { UserId = Guid.NewGuid(), Description = "Test", Priority = Priority.Low };
        inner.Handle(command, Arg.Any<CancellationToken>()).Returns(Result.Success(Guid.NewGuid()));

        var handler = new ValidationDecorator.CommandHandler<CreateTodoCommand, Guid>(inner, []);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await inner.Received(1).Handle(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CommandHandler_Should_CallInnerHandler_When_AllValidatorsPass()
    {
        ICommandHandler<CreateTodoCommand, Guid> inner = Substitute.For<ICommandHandler<CreateTodoCommand, Guid>>();
        IValidator<CreateTodoCommand> validator = Substitute.For<IValidator<CreateTodoCommand>>();
        var command = new CreateTodoCommand { UserId = Guid.NewGuid(), Description = "Test", Priority = Priority.Low };

        inner.Handle(command, Arg.Any<CancellationToken>()).Returns(Result.Success(Guid.NewGuid()));
        validator
            .ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var handler = new ValidationDecorator.CommandHandler<CreateTodoCommand, Guid>(inner, [validator]);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await inner.Received(1).Handle(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CommandHandler_Should_ReturnValidationError_And_NotCallInner_When_ValidatorFails()
    {
        ICommandHandler<CreateTodoCommand, Guid> inner = Substitute.For<ICommandHandler<CreateTodoCommand, Guid>>();
        IValidator<CreateTodoCommand> validator = Substitute.For<IValidator<CreateTodoCommand>>();
        var command = new CreateTodoCommand { UserId = Guid.Empty, Description = string.Empty };

        validator
            .ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([
                new ValidationFailure(nameof(CreateTodoCommand.UserId), "UserId must not be empty"),
                new ValidationFailure(nameof(CreateTodoCommand.Description), "Description must not be empty")
            ]));

        var handler = new ValidationDecorator.CommandHandler<CreateTodoCommand, Guid>(inner, [validator]);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeOfType<ValidationError>();
        ((ValidationError)result.Error).Errors.Length.ShouldBe(2);
        await inner.DidNotReceive().Handle(Arg.Any<CreateTodoCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CommandHandler_Should_PropagateInnerHandlerFailure_When_ValidationPasses()
    {
        ICommandHandler<CreateTodoCommand, Guid> inner = Substitute.For<ICommandHandler<CreateTodoCommand, Guid>>();
        IValidator<CreateTodoCommand> validator = Substitute.For<IValidator<CreateTodoCommand>>();
        var command = new CreateTodoCommand { UserId = Guid.NewGuid(), Description = "Test" };
        var domainError = Error.NotFound("User.NotFound", "User not found");

        validator
            .ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        inner.Handle(command, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Guid>(domainError));

        var handler = new ValidationDecorator.CommandHandler<CreateTodoCommand, Guid>(inner, [validator]);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(domainError);
    }

    // ── CommandBaseHandler<TCommand> (commands that return void/Result) ──────────

    [Fact]
    public async Task CommandBaseHandler_Should_CallInnerHandler_When_NoValidatorsRegistered()
    {
        ICommandHandler<DeleteTodoCommand> inner = Substitute.For<ICommandHandler<DeleteTodoCommand>>();
        var command = new DeleteTodoCommand(Guid.NewGuid());
        inner.Handle(command, Arg.Any<CancellationToken>()).Returns(Result.Success());

        var handler = new ValidationDecorator.CommandBaseHandler<DeleteTodoCommand>(inner, []);

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await inner.Received(1).Handle(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CommandBaseHandler_Should_ReturnValidationError_And_NotCallInner_When_ValidatorFails()
    {
        ICommandHandler<DeleteTodoCommand> inner = Substitute.For<ICommandHandler<DeleteTodoCommand>>();
        IValidator<DeleteTodoCommand> validator = Substitute.For<IValidator<DeleteTodoCommand>>();
        var command = new DeleteTodoCommand(Guid.Empty);

        validator
            .ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([
                new ValidationFailure(nameof(DeleteTodoCommand.TodoItemId), "TodoItemId must not be empty")
            ]));

        var handler = new ValidationDecorator.CommandBaseHandler<DeleteTodoCommand>(inner, [validator]);

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeOfType<ValidationError>();
        await inner.DidNotReceive().Handle(Arg.Any<DeleteTodoCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CommandBaseHandler_Should_AggregateErrors_When_MultipleValidatorsFail()
    {
        ICommandHandler<DeleteTodoCommand> inner = Substitute.For<ICommandHandler<DeleteTodoCommand>>();
        IValidator<DeleteTodoCommand> validator1 = Substitute.For<IValidator<DeleteTodoCommand>>();
        IValidator<DeleteTodoCommand> validator2 = Substitute.For<IValidator<DeleteTodoCommand>>();
        var command = new DeleteTodoCommand(Guid.Empty);

        validator1
            .ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("Field1", "Error 1")]));
        validator2
            .ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("Field2", "Error 2")]));

        var handler = new ValidationDecorator.CommandBaseHandler<DeleteTodoCommand>(inner, [validator1, validator2]);

        Result result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        ((ValidationError)result.Error).Errors.Length.ShouldBe(2);
    }
}
