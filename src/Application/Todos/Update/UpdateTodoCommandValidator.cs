using FluentValidation;

namespace Application.Todos.Update;

public sealed class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator()
    {
        RuleFor(c => c.TodoItemId).NotEmpty();
        RuleFor(c => c.Request.Description).NotEmpty().MaximumLength(500);
        RuleFor(c => c.Request.Priority).IsInEnum();
    }
}
