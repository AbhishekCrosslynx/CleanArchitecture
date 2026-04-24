using Application.Todos.Create;
using Domain.Todos;
using FluentValidation.TestHelper;

namespace Application.Tests.L0.Todos.Create;

public class CreateTodoCommandValidatorTests
{
    private readonly CreateTodoCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_HaveError_When_UserIdIsEmpty()
    {
        var command = new CreateTodoCommand { UserId = Guid.Empty, Description = "Test", Priority = Priority.Low };

        TestValidationResult<CreateTodoCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UserId);
    }

    [Fact]
    public void Validate_Should_HaveError_When_DescriptionIsEmpty()
    {
        var command = new CreateTodoCommand { UserId = Guid.NewGuid(), Description = string.Empty, Priority = Priority.Low };

        TestValidationResult<CreateTodoCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Description);
    }

    [Fact]
    public void Validate_Should_HaveError_When_DescriptionExceedsMaxLength()
    {
        var command = new CreateTodoCommand { UserId = Guid.NewGuid(), Description = new string('x', 256), Priority = Priority.Low };

        TestValidationResult<CreateTodoCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Description);
    }

    [Fact]
    public void Validate_Should_HaveError_When_PriorityIsNotDefinedInEnum()
    {
        var command = new CreateTodoCommand { UserId = Guid.NewGuid(), Description = "Test", Priority = (Priority)99 };

        TestValidationResult<CreateTodoCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Priority);
    }

    [Fact]
    public void Validate_Should_HaveError_When_DueDateIsInThePast()
    {
        var command = new CreateTodoCommand
        {
            UserId = Guid.NewGuid(),
            Description = "Test",
            Priority = Priority.Low,
            DueDate = DateTime.Today.AddDays(-1)
        };

        TestValidationResult<CreateTodoCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.DueDate);
    }

    [Fact]
    public void Validate_Should_NotHaveError_When_DueDateIsToday()
    {
        var command = new CreateTodoCommand
        {
            UserId = Guid.NewGuid(),
            Description = "Test",
            Priority = Priority.Low,
            DueDate = DateTime.Today
        };

        TestValidationResult<CreateTodoCommand> result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(c => c.DueDate);
    }

    [Fact]
    public void Validate_Should_NotHaveError_When_DueDateIsNull()
    {
        var command = new CreateTodoCommand { UserId = Guid.NewGuid(), Description = "Test", Priority = Priority.Low, DueDate = null };

        TestValidationResult<CreateTodoCommand> result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(c => c.DueDate);
    }

    [Fact]
    public void Validate_Should_NotHaveAnyErrors_When_CommandIsValid()
    {
        var command = new CreateTodoCommand
        {
            UserId = Guid.NewGuid(),
            Description = "Valid description",
            Priority = Priority.High,
            DueDate = DateTime.Today.AddDays(7),
            Labels = ["work", "important"]
        };

        TestValidationResult<CreateTodoCommand> result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
