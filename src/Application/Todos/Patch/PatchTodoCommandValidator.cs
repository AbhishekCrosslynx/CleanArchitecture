using FluentValidation;

namespace Application.Todos.Patch;

internal sealed class PatchTodoCommandValidator : AbstractValidator<PatchTodoCommand>
{
    public PatchTodoCommandValidator()
    {
        RuleFor(c => c.TodoItemId).NotEmpty();
    }
}
