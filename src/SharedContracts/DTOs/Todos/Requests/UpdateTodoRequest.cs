using SharedKernel.Enums;

namespace SharedContracts.DTOs.Todos.Requests;

public record UpdateTodoRequest(
    string Description,
    DateTime? DueDate,
    List<string>? Labels,
    Priority Priority
);
