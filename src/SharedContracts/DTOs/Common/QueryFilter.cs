namespace SharedContracts.DTOs.Common;

public record QueryFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? Search { get; set; }
}
