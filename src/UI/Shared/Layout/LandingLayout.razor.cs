using Microsoft.AspNetCore.Components;
using Shared.Services;

namespace Shared.Layout;

public partial class LandingLayout : LayoutComponentBase
{
    [Inject] protected LayoutService LayoutService { get; set; }

    private bool _drawerOpen;

    protected override void OnInitialized()
    {
        LayoutService.SetBaseTheme(Theme.LandingPageTheme());

        base.OnInitialized();
    }

    private void ToggleDrawer()
    {
        _drawerOpen = !_drawerOpen;
    }
}
