using Application.Abstractions.Messaging;
using SharedContracts.DTOs.Todos.Requests;

namespace Application.Todos.Update;

public sealed record UpdateTodoCommand(
    Guid TodoItemId,
    UpdateTodoRequest Request) : ICommand;
