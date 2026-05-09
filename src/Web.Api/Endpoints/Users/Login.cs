using Application.Abstractions.Messaging;
using Application.Users.Login;
using SharedContracts.ApiRoutes;
using SharedContracts.DTOs.Users.Requests;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

internal sealed class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(AuthRoutes.Login, async (
            LoginUserRequest request,
            ICommandHandler<LoginUserCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginUserCommand(request.Email, request.Password);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .WithName("LoginUser")
        .WithSummary("Authenticate user and generate token")
        .WithDescription("Validates user credentials and returns a JWT authentication token.")
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
