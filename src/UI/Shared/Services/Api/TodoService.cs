using System.Net.Http.Json;
using SharedContracts.ApiRoutes;
using SharedContracts.DTOs.Todos.Requests;
using SharedContracts.DTOs.Todos.Responses;

namespace Shared.Services.Api;

/// <summary>
/// TodoAPI Service - handles direct HTTP calls to the TodoAPI.
/// 
/// Responsibilities:
/// - Send requests to API endpoints
/// - Deserialize responses into DTOs
/// - Throws exceptions on HTTP errors (via EnsureSuccessStatusCode)
/// 
/// Patterns:
/// - Create: returns the newly created entity
/// - Update/Complete/Delete: return void (success indicated by HTTP 204)
/// - GetAll: returns list of entities
/// - GetById: returns entity or null if not found
/// </summary>
public class TodoService
{
    private readonly HttpClient _httpClient;

    public TodoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Get all todos for the current user.
    /// </summary>
    public async Task<List<TodoResponse>> GetAllAsync()
    {
        List<TodoResponse>? todos = await _httpClient.GetFromJsonAsync<List<TodoResponse>>(TodoRoutes.GetAll);
        return todos ?? new List<TodoResponse>();
    }

    /// <summary>
    /// Get a single TodoItem by its ID. Returns null if not found.
    /// </summary>
    public async Task<TodoResponse?> GetByIdAsync(Guid todoId)
    {
        return await _httpClient.GetFromJsonAsync<TodoResponse>(TodoRoutes.GetByIdPath(todoId));
    }

    /// <summary>
    /// Create a new TodoItem.
    /// </summary>
    public async Task<TodoResponse?> CreateAsync(CreateTodoRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(TodoRoutes.Create, request);
        return await response.Content.ReadFromJsonAsync<TodoResponse>();
    }

    /// <summary>
    /// Partially update a TodoItem.
    /// </summary>
    public async Task PatchAsync(Guid todoId, PatchTodoRequest request)
    {
        await _httpClient.PatchAsJsonAsync(TodoRoutes.UpdatePath(todoId), request);
    }

    /// <summary>
    /// Update a TodoItem.
    /// </summary>
    public async Task UpdateAsync(Guid todoId, UpdateTodoRequest request)
    {
        await _httpClient.PutAsJsonAsync(TodoRoutes.UpdatePath(todoId), request);
    }

    /// <summary>
    /// Delete a TodoItem by ID.
    /// </summary>
    public async Task DeleteAsync(Guid todoId)
    {
        await _httpClient.DeleteAsync(TodoRoutes.DeletePath(todoId));
    }

    /// <summary>
    /// Create a copy of an existing TodoItem and return the copied TodoItem object.
    /// </summary>
    public async Task<TodoResponse?> CopyAsync(Guid todoId)
    {
        HttpResponseMessage response = await _httpClient.PostAsync(TodoRoutes.CopyPath(todoId), null);
        return await response.Content.ReadFromJsonAsync<TodoResponse>();
    }
}
