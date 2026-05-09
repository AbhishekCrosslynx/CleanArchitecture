using SharedKernel.Enums;

namespace SharedContracts.DTOs.Todos.Requests;

public record PatchTodoRequest
{
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public List<string>? Labels { get; set; }
    public Priority? Priority { get; set; }
    public bool? IsCompleted { get; set; }
}
