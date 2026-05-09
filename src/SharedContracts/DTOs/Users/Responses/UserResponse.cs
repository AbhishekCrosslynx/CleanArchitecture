namespace SharedContracts.DTOs.Users.Responses;

public record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName
);
