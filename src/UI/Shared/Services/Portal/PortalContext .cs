namespace Shared.Services.Portal;

/// <summary>
/// Holds the current portal state for the UI.
/// This is a scoped service → one instance per user session.
/// </summary>
public sealed class PortalContext : IPortalContext
{
    private PortalType _current = PortalType.Unknown;

    /// <inheritdoc />
    public PortalType Current => _current;

    /// <inheritdoc />
    public event EventHandler? OnChange;

    /// <inheritdoc />
    public void SetPortal(PortalType portal)
    {
        // Avoid unnecessary re-renders if same portal
        if (_current == portal)
        {
            return;
        }
        _current = portal;

        // Notify subscribers (components)
        NotifyStateChanged();
    }

    /// <inheritdoc />
    public bool Is(PortalType portal) => _current == portal;

    /// <summary>
    /// Triggers UI updates for all subscribed components.
    /// </summary>
    private void NotifyStateChanged()
    {
        OnChange?.Invoke(this, EventArgs.Empty);
    }
}
