using Microsoft.AspNetCore.Components;
using Shared.Services;

namespace Shared.Layout;

public partial class DocsLayout : LayoutComponentBase
{
    [Inject] private LayoutService LayoutService { get; set; }
    //[Inject] private NavigationManager NavigationManager { get; set; }

    //private NavMenu _navMenuRef;
    private bool _drawerOpen;
    //private bool _topMenuOpen;
    protected override void OnInitialized()
    {
        LayoutService.SetBaseTheme(Theme.DocsTheme());
    }

    //protected override void OnAfterRender(bool firstRender)
    //{
    //    //refresh nav menu because no parameters change in nav menu but internal data does
    //    _navMenuRef?.Refresh();
    //}

    private void ToggleDrawer()
    {
        _drawerOpen = !_drawerOpen;
    }

    //private void OnDrawerOpenChanged(bool value)
    //{
    //    _topMenuOpen = false;
    //    _drawerOpen = value;
    //    StateHasChanged();
    //}
}
