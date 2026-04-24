using Shared.Enums;

namespace Shared.Services.UserPreferences;

public class UserPreferences
{
    /// <summary>
    /// The direction of the layout.
    /// </summary>
    public bool RightToLeft { get; set; }

    /// <summary>
    /// The preferred dark mode configuration.
    /// </summary>
    public DarkLightMode DarkLightTheme { get; set; }
}
