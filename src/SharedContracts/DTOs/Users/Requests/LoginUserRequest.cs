namespace SharedContracts.DTOs.Users.Requests;

public sealed record LoginUserRequest(
    string Email,
    string Password
);
