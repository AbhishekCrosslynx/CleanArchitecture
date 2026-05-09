using Application.Abstractions.Messaging;
using Application.Todos.Get;
using SharedContracts.ApiRoutes;
using SharedContracts.DTOs.Todos.Responses;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(TodoRoutes.GetAll, async (
            IQueryHandler<GetTodosQuery, List<TodoResponse>> handler,
            CancellationToken cancellationToken) =>
        {

            Result<List<TodoResponse>> result =
                await handler.Handle(new GetTodosQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithTags(Tags.Todos)
            .RequireAuthorization()
            .WithName("GetTodos")
            .WithSummary("Get all todos")
            .WithDescription("Retrieves all todo items.")
            .Produces<List<TodoResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
