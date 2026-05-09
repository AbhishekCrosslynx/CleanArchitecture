using Application.Abstractions.Messaging;
using Application.Todos.Update;
using SharedContracts.ApiRoutes;
using SharedContracts.DTOs.Todos.Requests;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class Update : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(TodoRoutes.Update, async (
            Guid todoId,
            UpdateTodoRequest request,
            ICommandHandler<UpdateTodoCommand> handler,
            CancellationToken ct) =>
        {
            var command = new UpdateTodoCommand(todoId, request);

            Result result = await handler.Handle(command, ct);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization()
        .WithName("UpdateTodo")
        .WithSummary("Update a todo")
        .WithDescription("Updates an existing todo item using its unique identifier.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
