namespace SharedContracts.DTOs.Users.Requests;

public sealed record RegisterUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string Password
);
