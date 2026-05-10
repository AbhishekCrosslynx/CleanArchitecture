using Web.Api.Options;

namespace Web.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring Cross-Origin Resource Sharing (CORS) policies in an ASP.NET Core application.
/// </summary>
public static class CorsExtensions
{
    /// <summary>
    /// Adds and configures CORS policies for the application using allowed origins specified in the configuration.
    /// </summary>
    /// <remarks>This method registers a CORS policy named 'DevCors' that allows any header, any method,
    /// credentials, and restricts origins to those specified in the configuration. Use this method during service
    /// configuration to ensure CORS is set up according to application requirements.</remarks>
    /// <param name="services">The service collection to which the CORS services and policies will be added.</param>
    /// <param name="configuration">The application configuration containing the CORS settings, specifically the 'Cors:AllowedOrigins' section.</param>
    /// <returns>The same IServiceCollection instance so that additional calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the 'Cors:AllowedOrigins' configuration section is missing.</exception>
    public static IServiceCollection AddConfiguredCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //string[] allowedOrigins =
        //    configuration
        //        .GetSection("Cors:AllowedOrigins")
        //        .Get<string[]>() ?? [];

        string[] allowedOrigins =
            configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>()
            ?? throw new InvalidOperationException(
                "Cors:AllowedOrigins configuration is missing.");

        services.AddCors(options =>
        {
            options.AddPolicy(CorsOptions.DevCors, policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromDays(1));
            });
        });

        return services;
    }
}
