namespace Web.Api.Options;

/// <summary>
/// Represents configuration options for Cross-Origin Resource Sharing (CORS) policies.
/// </summary>
/// <remarks>Use this class to specify allowed origins and related CORS settings for the application. Typically
/// bound from configuration sources such as appsettings.json.</remarks>
public sealed class CorsOptions
{
    public const string DevCors = "DevCors";

    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; init; } = [];
}
