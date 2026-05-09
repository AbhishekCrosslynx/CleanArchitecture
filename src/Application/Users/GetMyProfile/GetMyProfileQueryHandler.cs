using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedContracts.DTOs.Users.Responses;
using SharedKernel;

namespace Application.Users.GetMyProfile;

internal sealed class GetMyProfileQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetMyProfileQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetMyProfileQuery query, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        UserResponse? user = await context.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserResponse(
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName
            ))
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(userId));
        }

        return user;
    }
}
