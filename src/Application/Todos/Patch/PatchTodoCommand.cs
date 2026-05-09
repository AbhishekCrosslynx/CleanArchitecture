using Application.Abstractions.Messaging;
using SharedContracts.DTOs.Todos.Requests;

namespace Application.Todos.Patch;

public sealed record PatchTodoCommand(
    Guid TodoItemId,
    PatchTodoRequest Request
) : ICommand;
