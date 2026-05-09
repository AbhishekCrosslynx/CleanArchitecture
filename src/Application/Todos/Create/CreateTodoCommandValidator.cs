using FluentValidation;

namespace Application.Todos.Create;

public sealed class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(255)
            .Must(BeAValidDescription)
            .WithMessage("Description contains invalid characters or is not valid.");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .When(x => x.DueDate.HasValue)
            .WithMessage("Due date cannot be in the past.");

        RuleFor(x => x.Labels)
            .NotNull()
            .Must(labels => labels.Count <= 10)
            .WithMessage("You can assign up to 10 labels only.")
            .ForEach(label =>
            {
                label.NotEmpty()
                     .MaximumLength(50);
            });

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Priority must be explicitly set to a valid value.");
    }

    /// <summary>
    /// Prevents nonsense inputs like only whitespace or repeated symbols.
    /// </summary>
    private static bool BeAValidDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return false;
        }

        string trimmed = description.Trim();

        // prevents "     " or weird junk like "!!!!!"
        if (trimmed.Length < 3)
        {
            return false;
        }

        return trimmed.Any(char.IsLetterOrDigit);
    }
}
