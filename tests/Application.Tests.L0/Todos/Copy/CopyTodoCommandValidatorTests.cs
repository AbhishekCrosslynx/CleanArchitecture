using Application.Todos.Copy;
using FluentValidation.TestHelper;

namespace Application.Tests.L0.Todos.Copy;

public class CopyTodoCommandValidatorTests
{
    private readonly CopyTodoCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_HaveError_When_UserIdIsEmpty()
    {
        var command = new CopyTodoCommand { UserId = Guid.Empty, TodoId = Guid.NewGuid() };

        TestValidationResult<CopyTodoCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UserId);
    }

    [Fact]
    public void Validate_Should_HaveError_When_TodoIdIsEmpty()
    {
        var command = new CopyTodoCommand { UserId = Guid.NewGuid(), TodoId = Guid.Empty };

        TestValidationResult<CopyTodoCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.TodoId);
    }

    [Fact]
    public void Validate_Should_HaveErrors_When_BothFieldsAreEmpty()
    {
        var command = new CopyTodoCommand { UserId = Guid.Empty, TodoId = Guid.Empty };

        TestValidationResult<CopyTodoCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UserId);
        result.ShouldHaveValidationErrorFor(c => c.TodoId);
    }

    [Fact]
    public void Validate_Should_NotHaveAnyErrors_When_CommandIsValid()
    {
        var command = new CopyTodoCommand { UserId = Guid.NewGuid(), TodoId = Guid.NewGuid() };

        TestValidationResult<CopyTodoCommand> result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
