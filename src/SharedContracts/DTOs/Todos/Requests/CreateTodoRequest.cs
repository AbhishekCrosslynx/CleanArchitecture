using SharedKernel.Enums;

namespace SharedContracts.DTOs.Todos.Requests;

public record CreateTodoRequest(
    string Description,
    DateTime? DueDate,
    List<string> Labels,
    Priority Priority = Priority.Normal
);
