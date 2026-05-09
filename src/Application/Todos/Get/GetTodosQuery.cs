using Application.Abstractions.Messaging;
using SharedContracts.DTOs.Todos.Responses;

namespace Application.Todos.Get;

public sealed record GetTodosQuery() : IQuery<List<TodoResponse>>;
