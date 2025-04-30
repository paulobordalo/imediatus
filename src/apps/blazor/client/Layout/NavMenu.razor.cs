using imediatus.Blazor.Infrastructure.Auth;
using imediatus.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace imediatus.Blazor.Client.Layout;

public partial class NavMenu
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    private bool _canViewHangfire;
    private bool _canViewDashboard;
    private bool _canViewRoles;
    private bool _canViewUsers;
    private bool _canViewProducts;
    private bool _canViewBrands;
    private bool _canViewTodos;
    private bool _canViewTenants;
    private bool _canViewAuditTrails;
    private bool CanViewAdministrationGroup => _canViewUsers || _canViewRoles || _canViewTenants;

    protected override async Task OnParametersSetAsync()
    {
        var user = (await AuthState).User;
        _canViewHangfire = await AuthService.HasPermissionAsync(user, ImediatusActions.View, ImediatusResources.Hangfire);
        _canViewDashboard = await AuthService.HasPermissionAsync(user, ImediatusActions.View, ImediatusResources.Dashboard);
        _canViewRoles = await AuthService.HasPermissionAsync(user, ImediatusActions.View, ImediatusResources.Roles);
        _canViewUsers = await AuthService.HasPermissionAsync(user, ImediatusActions.View, ImediatusResources.Users);
        _canViewProducts = await AuthService.HasPermissionAsync(user, ImediatusActions.View, ImediatusResources.Products);
        _canViewBrands = await AuthService.HasPermissionAsync(user, ImediatusActions.View, ImediatusResources.Brands);
        _canViewTodos = await AuthService.HasPermissionAsync(user, ImediatusActions.View, ImediatusResources.Todos);
        _canViewTenants = await AuthService.HasPermissionAsync(user, ImediatusActions.View, ImediatusResources.Tenants);
        _canViewAuditTrails = await AuthService.HasPermissionAsync(user, ImediatusActions.View, ImediatusResources.AuditTrails);
    }
}
