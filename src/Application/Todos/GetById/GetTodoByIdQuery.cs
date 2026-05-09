using Application.Abstractions.Messaging;
using SharedContracts.DTOs.Todos.Responses;

namespace Application.Todos.GetById;

public sealed record GetTodoByIdQuery(Guid TodoItemId) : IQuery<TodoResponse>;
