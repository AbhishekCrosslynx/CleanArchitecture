using FluentValidation;

namespace Application.Todos.Copy;

public sealed class CopyTodoCommandValidator : AbstractValidator<CopyTodoCommand>
{
    public CopyTodoCommandValidator()
    {
        RuleFor(c => c.TodoId)
            .NotEmpty()
            .NotEqual(Guid.Empty);

    }
}
