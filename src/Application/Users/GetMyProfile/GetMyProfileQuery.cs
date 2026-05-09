using Application.Abstractions.Messaging;
using SharedContracts.DTOs.Users.Responses;

namespace Application.Users.GetMyProfile;

public sealed record GetMyProfileQuery() : IQuery<UserResponse>;
