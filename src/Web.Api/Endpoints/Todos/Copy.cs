using Application.Abstractions.Messaging;
using Application.Todos.Copy;
using SharedContracts.ApiRoutes;
using SharedContracts.DTOs.Todos.Responses;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class Copy : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(TodoRoutes.Copy, async (
            Guid todoId,
            ICommandHandler<CopyTodoCommand, TodoResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CopyTodoCommand { TodoId = todoId };

            Result<TodoResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization()
        .WithName("CopyTodo")
        .WithSummary("Copy a todo")
        .WithDescription("Creates a duplicate of an existing todo item and returns the copied todo.")
        .Produces<TodoResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
