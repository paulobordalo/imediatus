using imediatus.Blazor.Client.Pages.Identity.Users;
using imediatus.Blazor.Infrastructure.Auth;
using imediatus.Blazor.Infrastructure.Preferences;
using imediatus.Shared.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace imediatus.Blazor.Client.Layout;

public partial class MainLayout
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;
    [Parameter]
    public EventCallback<bool> OnDarkModeToggle { get; set; }
    [Parameter]
    public EventCallback<bool> OnRightToLeftToggle { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    protected IAuthenticationService AuthService { get; set; } = default!;

    protected UserInfo _loggedUser;

    private bool _drawerOpen;
    private bool _isDarkMode;

    protected override async Task OnInitializedAsync()
    {

        var user = (await AuthState).User;
        if (user.Identity?.IsAuthenticated == true)
        {
            _loggedUser = new UserInfo()
            {
                Name = user.GetFullName() ?? string.Empty,
                UserId = user.GetUserId() ?? string.Empty
            };
        }

        if (await ClientPreferences.GetPreference() is ClientPreference preferences)
        {
            _drawerOpen = preferences.IsDrawerOpen;
            _isDarkMode = preferences.IsDarkMode;
        }
    }
    private Task<MudBlazor.IDialogReference> OpenDialogAsync()
    {
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseOnEscapeKey = true };

        var parameters = new DialogParameters { { "LoggedUser", _loggedUser } };

        return DialogService.ShowAsync<Components.Dialogs.Portfolio>("Portfolio Dialog", parameters, options);
    }


    public async Task ToggleDarkMode()
    {
        _isDarkMode = !_isDarkMode;
        await OnDarkModeToggle.InvokeAsync(_isDarkMode);
    }

    private async Task DrawerToggle()
    {
        _drawerOpen = await ClientPreferences.ToggleDrawerAsync();
    }
    private async Task LogoutAsync()
    {
        var parameters = new DialogParameters
        {
            { nameof(Components.Dialogs.Logout.ContentText), "Do you want to logout from the system?" },
            { nameof(Components.Dialogs.Logout.ButtonText), "Logout" },
            { nameof(Components.Dialogs.Logout.Color), Color.Error }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        await DialogService.ShowAsync<Components.Dialogs.Logout>("Logout", parameters, options);
    }

    private void Profile()
    {
        Navigation.NavigateTo("/identity/account");
    }
}
