using Blazored.LocalStorage;
using Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Shared.Http;
using Shared.Services;
using Shared.Services.Api;
using Shared.Services.Notifications;
using Shared.Services.Portal;
using Shared.Services.UserPreferences;
using Shared.States;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddScoped<IPortalContext, PortalContext>();
builder.Services.AddScoped<LayoutService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IUserPreferencesService, UserPreferencesService>();
builder.Services.AddScoped<INotificationService, InMemoryNotificationService>();

// Register API services and States
builder.Services.AddScoped<AuthState>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ErrorNotifier>();

builder.Services.AddTransient<AuthenticationHttpMessageHandler>();
builder.Services.AddTransient<ErrorHandlingHttpMessageHandler>();

string apiUrl = builder.Configuration["ApiBaseUrl"]!;

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiUrl);
})
.AddHttpMessageHandler<AuthenticationHttpMessageHandler>()
.AddHttpMessageHandler<ErrorHandlingHttpMessageHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>()
        .CreateClient("Api"));

await builder.Build().RunAsync();
