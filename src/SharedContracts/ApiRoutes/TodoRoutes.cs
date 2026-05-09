namespace SharedContracts.ApiRoutes;

public static class TodoRoutes
{
    public const string GetAll = "/api/todos";

    public const string Create = "/api/todos";

    public const string GetById = "/api/todos/{id:guid}";

    public const string Delete = "/api/todos/{id:guid}";

    public const string Update = "/api/todos/{id:guid}";

    public const string Copy = "/api/todos/{todoId:guid}/copy";

    // URL BUILDERS FOR FRONTEND
    public static string GetByIdPath(Guid id)
        => $"/api/todos/{id}";

    public static string DeletePath(Guid id)
        => $"/api/todos/{id}";

    public static string UpdatePath(Guid id)
        => $"/api/todos/{id}";

    public static string CopyPath(Guid todoId)
        => $"/api/todos/{todoId}/copy";

}
