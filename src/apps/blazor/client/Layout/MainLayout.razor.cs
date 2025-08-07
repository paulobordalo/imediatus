using imediatus.Blazor.Infrastructure.Preferences;
using imediatus.Blazor.Infrastructure.State;
using Microsoft.AspNetCore.Components;
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
    [Inject]
    protected AppState AppState { get; set; }

    private bool _drawerOpen;
    private bool _isDarkMode;

    protected override async Task OnInitializedAsync()
    {
        if (await ClientPreferences.GetPreference() is ClientPreference preferences)
        {
            _drawerOpen = preferences.IsDrawerOpen;
            _isDarkMode = preferences.IsDarkMode;
        }

        await AppState.InitializeAsync();
    }

    private async Task<MudBlazor.IDialogReference> OpenDialogAsync()
    {
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseOnEscapeKey = true };

        return await DialogService.ShowAsync<Components.Dialogs.CreatePortfolio>("Portfolio Dialog", options);
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
