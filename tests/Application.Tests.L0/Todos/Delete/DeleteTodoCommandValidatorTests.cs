using Application.Todos.Delete;
using FluentValidation.TestHelper;

namespace Application.Tests.L0.Todos.Delete;

public class DeleteTodoCommandValidatorTests
{
    private readonly DeleteTodoCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_HaveError_When_TodoItemIdIsEmpty()
    {
        var command = new DeleteTodoCommand(Guid.Empty);

        TestValidationResult<DeleteTodoCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.TodoItemId);
    }

    [Fact]
    public void Validate_Should_NotHaveAnyErrors_When_CommandIsValid()
    {
        var command = new DeleteTodoCommand(Guid.NewGuid());

        TestValidationResult<DeleteTodoCommand> result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
