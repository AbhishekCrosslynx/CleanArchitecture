using Application.Users.Register;
using FluentValidation.TestHelper;

namespace Application.Tests.L0.Users.Register;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_HaveError_When_FirstNameIsEmpty()
    {
        var command = new RegisterUserCommand("user@test.com", string.Empty, "Doe", "password123");

        TestValidationResult<RegisterUserCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.FirstName);
    }

    [Fact]
    public void Validate_Should_HaveError_When_LastNameIsEmpty()
    {
        var command = new RegisterUserCommand("user@test.com", "John", string.Empty, "password123");

        TestValidationResult<RegisterUserCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.LastName);
    }

    [Fact]
    public void Validate_Should_HaveError_When_EmailIsEmpty()
    {
        var command = new RegisterUserCommand(string.Empty, "John", "Doe", "password123");

        TestValidationResult<RegisterUserCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void Validate_Should_HaveError_When_EmailIsInvalidFormat()
    {
        var command = new RegisterUserCommand("not-an-email", "John", "Doe", "password123");

        TestValidationResult<RegisterUserCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void Validate_Should_HaveError_When_PasswordIsEmpty()
    {
        var command = new RegisterUserCommand("user@test.com", "John", "Doe", string.Empty);

        TestValidationResult<RegisterUserCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public void Validate_Should_HaveError_When_PasswordIsShorterThanMinimum()
    {
        var command = new RegisterUserCommand("user@test.com", "John", "Doe", "short");

        TestValidationResult<RegisterUserCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public void Validate_Should_NotHaveError_When_PasswordIsExactlyMinimumLength()
    {
        var command = new RegisterUserCommand("user@test.com", "John", "Doe", "exactly8");

        TestValidationResult<RegisterUserCommand> result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public void Validate_Should_NotHaveAnyErrors_When_CommandIsValid()
    {
        var command = new RegisterUserCommand("valid@email.com", "John", "Doe", "securePassword1!");

        TestValidationResult<RegisterUserCommand> result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
