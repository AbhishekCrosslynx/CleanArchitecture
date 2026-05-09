using Application.Abstractions.Messaging;
using SharedContracts.DTOs.Todos.Responses;
using SharedKernel.Enums;

namespace Application.Todos.Create;

public sealed class CreateTodoCommand : ICommand<TodoResponse>
{
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public List<string> Labels { get; set; } = [];
    public Priority Priority { get; set; }
}
