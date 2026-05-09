using SharedKernel.Enums;

namespace SharedContracts.DTOs.Todos.Responses;

public record TodoResponse(
    Guid Id,
    Guid UserId,
    string Description,
    DateTime? DueDate,
    List<string> Labels,
    bool IsCompleted,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    Priority Priority
);
