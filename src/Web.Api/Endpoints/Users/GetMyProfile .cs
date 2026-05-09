using Application.Abstractions.Messaging;
using Application.Users.GetMyProfile;
using SharedContracts.ApiRoutes;
using SharedContracts.DTOs.Users.Responses;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

internal sealed class GetMyProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(AuthRoutes.Profile, async (
            IQueryHandler<GetMyProfileQuery, UserResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<UserResponse> result = await handler.Handle(new GetMyProfileQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.UsersAccess)
        .WithTags(Tags.Users)
        .RequireAuthorization()
        .WithName("GetMyProfile")
        .WithSummary("Get current user profile")
        .WithDescription("Retrieves the profile information of the currently authenticated user.")
        .Produces<UserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
