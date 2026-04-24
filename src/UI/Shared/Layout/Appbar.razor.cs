using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;
using Shared.Services.Portal;

namespace Shared.Layout;

#nullable enable
public partial class Appbar : IDisposable
{
    //private DocsBasePage _activePage;
    private string _lastUri;
    private bool _searchDialogOpen;
    private bool _searchDialogAutocompleteOpen;
    private int _searchDialogReturnedItemsCount;

    private MudAutocomplete<ApiLinkServiceEntry>? _searchBarAutocomplete;
    private MudAutocomplete<ApiLinkServiceEntry>? _searchDialogAutocomplete;

    private readonly DialogOptions _dialogOptions = new DialogOptions()
    {
        Position = DialogPosition.TopCenter,
        NoHeader = true,
        CloseOnEscapeKey = true
    };

    private static readonly JsKeyModifier[] CtrlLeftKeyModifiers = [JsKeyModifier.ControlLeft];
    private static readonly JsKeyModifier[] CtrlRightKeyModifiers = [JsKeyModifier.ControlRight];
    private bool isMenuOpen;  // use this to track whether the Projects menu is open or closed
    //private readonly string PortalType = "Website"; // This can be set to "Client" or "Website" based on the portal type

    [Inject]
    private IPortalContext Portal { get; set; } = default!;


    private MudMessageBox _mudMessageBox;

    private async Task OnButtonClickedAsync()
    {
        bool? result = await _mudMessageBox.ShowAsync(new DialogOptions
        {
            Position = DialogPosition.TopCenter,
            BackdropClick = false,
            CloseOnEscapeKey = false,
        });
        if (result == true)
        {
            Logout();
        }

        StateHasChanged();
    }

    public bool IsSearchDialogOpen
    {
        get => _searchDialogOpen;
        set
        {
            _searchDialogAutocompleteOpen = default;
            _searchDialogReturnedItemsCount = default;
            _searchDialogOpen = value;
        }
    }

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    //[Inject]
    //private IApiLinkService ApiLinkService { get; set; } = null!;

    //[Inject]
    //private LayoutService LayoutService { get; set; } = null!;

    [Parameter]
    public EventCallback<MouseEventArgs> DrawerToggleCallback { get; set; }

    [Parameter]
    public bool DisplaySearchBar { get; set; } = true;

    private bool isAuthenticated = true;
    public readonly string userName = "John Doe";

    public void Logout()
    {
        isAuthenticated = false;

        // Later replace with real auth logic:
        // await AuthService.Logout();
        // NavigationManager.NavigateTo("/");
    }

    protected override void OnInitialized()
    {
        //UpdateActivePage(NavigationManager.Uri);
        NavigationManager.LocationChanged += OnLocationChanged;
        base.OnInitialized();
    }

    private async Task OnSearchResult(ApiLinkServiceEntry? entry)
    {
        if (entry is null)
        {
            return;
        }

        NavigationManager.NavigateTo(entry.Link);
        await Task.Delay(1000);
        if (_searchBarAutocomplete is not null)
        {
            await _searchBarAutocomplete.ClearAsync();
        }

        if (_searchDialogAutocomplete is not null)
        {
            await _searchDialogAutocomplete.ClearAsync();
        }
    }

    //private string GetActiveClass(DocsBasePage page)
    //{
    //    return page == _activePage ? "mud-chip-text mud-chip-color-primary mx-1 px-3" : "mx-1 px-3";
    //}
    //private string GetActiveClass(string pageUrl)
    //{
    //    return pageUrl == NavigationManager.Uri
    //        ? "mud-chip-text mud-chip-color-primary mx-1 px-3"
    //        : "mx-1 px-3";
    //}

    private string GetActiveClass(string pageUrl)
    {
        string currentPath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);

        if (!string.IsNullOrEmpty(pageUrl) && pageUrl.StartsWith('/'))
        {
            pageUrl = pageUrl.Substring(1);
        }

        return currentPath.StartsWith(pageUrl, StringComparison.OrdinalIgnoreCase)
            ? "mud-chip-text mud-chip-color-primary mx-1 px-3"
            : "mx-1 px-3";
    }

    //private async void OnLocationChanged(object? sender, LocationChangedEventArgs args)
    //{
    //    if (UpdateActivePage(args.Location))
    //    {
    //        await InvokeAsync(StateHasChanged);
    //    }
    //}

    private void OnLocationChanged(object? sender, LocationChangedEventArgs args)
    {
        if (_lastUri == args.Location)
        {
            return;
        }

        _lastUri = args.Location;
        InvokeAsync(StateHasChanged);
    }

    //private bool UpdateActivePage(string location)
    //{
    //    var activePage = LayoutService.GetDocsBasePage(location);
    //    if (_activePage == activePage)
    //    {
    //        return false;
    //    }

    //    _activePage = activePage;
    //    return true;
    //}

    //private Task<IReadOnlyCollection<ApiLinkServiceEntry>> Search(string text, CancellationToken token)
    //{
    //    if (string.IsNullOrWhiteSpace(text))
    //    {
    //        // The user just opened the popover so show the most popular pages according to our analytics data as search results.
    //        return Task.FromResult(ApiLinkService.GetFeaturedEntries());
    //    }

    //    return ApiLinkService.Search(text);
    //}

    private void OpenSearchDialog() => IsSearchDialogOpen = true;

    private async Task HandleSearchHotkeyAsync()
    {
        if (DisplaySearchBar && _searchBarAutocomplete is not null)
        {
            await _searchBarAutocomplete.FocusAsync();
            return;
        }

        OpenSearchDialog();
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }

    public class ApiLinkServiceEntry
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Link { get; set; }
    }

    private readonly List<ApiLinkServiceEntry> _blogEntries = new List<ApiLinkServiceEntry>
{
    new ApiLinkServiceEntry { Title = "Understanding Blazor", SubTitle = "A beginner's guide to Blazor.", Link = "/blog/understanding-blazor" },
    new ApiLinkServiceEntry { Title = "MudBlazor: The UI Component", SubTitle = "An overview of MudBlazor components.", Link = "/blog/mudblazor-ui" },
};

    private readonly List<ApiLinkServiceEntry> _orderDetails = new List<ApiLinkServiceEntry>
{
    new ApiLinkServiceEntry { Title = "Order #12345", SubTitle = "Pending", Link = "/order/12345" },
    new ApiLinkServiceEntry { Title = "Order #54321", SubTitle = "Shipped", Link = "/order/54321" },
};

    private Task<IReadOnlyCollection<ApiLinkServiceEntry>> Search(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Task.FromResult<IReadOnlyCollection<ApiLinkServiceEntry>>(new List<ApiLinkServiceEntry>());
        }

        // Depending on portal type, search in blogs or orders
        if (Portal.Current == PortalType.Website)
        {
            // Search in Blogs
            var results = _blogEntries.Where(b => b.Title.Contains(text, StringComparison.OrdinalIgnoreCase) || b.SubTitle.Contains(text, StringComparison.OrdinalIgnoreCase)).ToList();
            return Task.FromResult<IReadOnlyCollection<ApiLinkServiceEntry>>(results);
        }
        else if (Portal.Current == PortalType.Client)
        {
            // Search in Orders for clients
            var results = _orderDetails.Where(o => o.Title.Contains(text, StringComparison.OrdinalIgnoreCase) || o.SubTitle.Contains(text, StringComparison.OrdinalIgnoreCase)).ToList();
            return Task.FromResult<IReadOnlyCollection<ApiLinkServiceEntry>>(results);
        }

        // Default return empty results
        return Task.FromResult<IReadOnlyCollection<ApiLinkServiceEntry>>(new List<ApiLinkServiceEntry>());
    }

}
