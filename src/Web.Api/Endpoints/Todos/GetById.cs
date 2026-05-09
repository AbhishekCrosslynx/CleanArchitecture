using Application.Abstractions.Messaging;
using Application.Todos.GetById;
using SharedContracts.ApiRoutes;
using SharedContracts.DTOs.Todos.Responses;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(TodoRoutes.GetById, async (
            Guid id,
            IQueryHandler<GetTodoByIdQuery, TodoResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetTodoByIdQuery(id);

            Result<TodoResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization()
        .WithName("GetTodoById")
        .WithSummary("Get todo by ID")
        .WithDescription("Retrieves a specific todo item using its unique identifier.")
        .Produces<TodoResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
