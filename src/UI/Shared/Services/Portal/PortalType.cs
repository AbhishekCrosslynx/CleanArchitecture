namespace Shared.Services.Portal;

/// <summary>
/// Represents the different UI portals in the system.
/// Helps avoid magic strings across the UI.
/// </summary>
public enum PortalType
{
    Unknown = 0,
    Website,
    Client,
    Operations,
    Test
}
