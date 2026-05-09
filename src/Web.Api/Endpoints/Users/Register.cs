using Application.Abstractions.Messaging;
using Application.Users.Register;
using SharedContracts.ApiRoutes;
using SharedContracts.DTOs.Users.Requests;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

internal sealed class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(AuthRoutes.Register, async (
            RegisterUserRequest request,
            ICommandHandler<RegisterUserCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .WithName("RegisterUser")
        .WithSummary("Register a new user")
        .WithDescription("Creates a new user account and returns the generated user identifier.")
        .Produces<Guid>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
