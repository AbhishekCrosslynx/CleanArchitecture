namespace Shared.Services.Portal;

/// <summary>
/// Contract for accessing and managing the current portal context.
/// This allows components to react dynamically based on portal.
/// </summary>
public interface IPortalContext
{
    /// <summary>
    /// Gets the currently active portal.
    /// </summary>
    PortalType Current { get; }

    /// <summary>
    /// Event triggered whenever the portal changes.
    /// UI components can subscribe to re-render.
    /// </summary>
    event EventHandler? OnChange;

    /// <summary>
    /// Sets the current portal.
    /// This should be called during app initialization or routing.
    /// </summary>
    void SetPortal(PortalType portal);

    /// <summary>
    /// Helper to check current portal.
    /// </summary>
    bool Is(PortalType portal);
}
