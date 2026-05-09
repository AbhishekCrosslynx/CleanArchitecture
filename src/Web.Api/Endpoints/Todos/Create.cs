using Application.Abstractions.Messaging;
using Application.Todos.Create;
using SharedContracts.ApiRoutes;
using SharedContracts.DTOs.Todos.Requests;
using SharedContracts.DTOs.Todos.Responses;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(TodoRoutes.Create, async (
            CreateTodoRequest request,
            ICommandHandler<CreateTodoCommand, TodoResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateTodoCommand
            {
                Description = request.Description,
                DueDate = request.DueDate,
                Labels = request.Labels,
                Priority = request.Priority
            };

            Result<TodoResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization()
        .WithName("CreateTodo")
        .WithSummary("Create a new todo")
        .WithDescription("Creates a new todo item and returns the created resource with its generated ID.")
        .Produces<TodoResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
