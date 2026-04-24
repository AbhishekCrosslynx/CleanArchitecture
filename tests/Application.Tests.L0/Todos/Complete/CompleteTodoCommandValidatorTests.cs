using Application.Todos.Complete;
using FluentValidation.TestHelper;

namespace Application.Tests.L0.Todos.Complete;

public class CompleteTodoCommandValidatorTests
{
    private readonly CompleteTodoCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_HaveError_When_TodoItemIdIsEmpty()
    {
        var command = new CompleteTodoCommand(Guid.Empty);

        TestValidationResult<CompleteTodoCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.TodoItemId);
    }

    [Fact]
    public void Validate_Should_NotHaveAnyErrors_When_CommandIsValid()
    {
        var command = new CompleteTodoCommand(Guid.NewGuid());

        TestValidationResult<CompleteTodoCommand> result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
