//using Blazored.LocalStorage;
//using Microsoft.AspNetCore.Components.Web;
//using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
//using MudBlazor.Services;
//using Shared.Services;
//using Shared.Services.Notifications;
//using Shared.Services.UserPreferences;
//using Test;

//var builder = WebAssemblyHostBuilder.CreateDefault(args);
//builder.RootComponents.Add<App>("#app");
//builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddMudServices();

//builder.Services.AddScoped<LayoutService>();
//builder.Services.AddBlazoredLocalStorage();
//builder.Services.AddScoped<IUserPreferencesService, UserPreferencesService>();
//builder.Services.AddScoped<INotificationService, InMemoryNotificationService>();

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//await builder.Build().RunAsync();


using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Shared.Services;
using Shared.Services.Notifications;
using Shared.Services.Portal;
using Shared.Services.UserPreferences;
using Test;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Move all service registrations into ConfigureServices
ConfigureServices(builder.Services, builder.HostEnvironment);

WebAssemblyHost host = builder.Build();

//INotificationService? notificationService = build.Services.GetService<INotificationService>();
//if (notificationService is InMemoryNotificationService inMemoryService)
//{
//    inMemoryService.Preload();
//}

await host.RunAsync();

//await builder.Build().RunAsync();

static void ConfigureServices(IServiceCollection services, IWebAssemblyHostEnvironment hostEnv)
{
    // MudBlazor
    services.AddMudServices();

    // Your custom services
    services.AddScoped<IPortalContext, PortalContext>();
    services.AddScoped<LayoutService>();
    services.AddBlazoredLocalStorage();
    services.AddScoped<IUserPreferencesService, UserPreferencesService>();
    services.AddScoped<INotificationService, InMemoryNotificationService>();

    // HttpClient
    services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(hostEnv.BaseAddress) });
}
