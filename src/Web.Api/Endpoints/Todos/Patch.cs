using Application.Abstractions.Messaging;
using Application.Todos.Patch;
using SharedContracts.ApiRoutes;
using SharedContracts.DTOs.Todos.Requests;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class Patch : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(TodoRoutes.Update, async (
            Guid todoId,
            PatchTodoRequest Request,
            ICommandHandler<PatchTodoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new PatchTodoCommand(todoId, Request);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization()
        .WithName("PatchTodo")
        .WithSummary("Partially update a todo")
        .WithDescription("Applies partial updates to an existing todo item using JSON Patch semantics or partial request data.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
