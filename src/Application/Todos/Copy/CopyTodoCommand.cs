using Application.Abstractions.Messaging;
using SharedContracts.DTOs.Todos.Responses;

namespace Application.Todos.Copy;

public sealed class CopyTodoCommand : ICommand<TodoResponse>
{
    public Guid TodoId { get; set; }
}
