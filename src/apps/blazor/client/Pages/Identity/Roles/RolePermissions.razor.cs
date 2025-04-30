using imediatus.Blazor.Client.Components;
using imediatus.Blazor.Infrastructure.Api;
using imediatus.Blazor.Infrastructure.Auth;
using imediatus.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace imediatus.Blazor.Client.Pages.Identity.Roles;

public partial class RolePermissions
{
    [Parameter]
    public string Id { get; set; } = default!; // from route
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    protected IApiClient RolesClient { get; set; } = default!;

    private Dictionary<string, List<PermissionViewModel>> _groupedRoleClaims = default!;

    public string Title = string.Empty;
    public string Description = string.Empty;

    private string _searchString = string.Empty;

    private bool _canEditRoleClaims;
    private bool _canSearchRoleClaims;
    private bool _loaded;

    static RolePermissions() => TypeAdapterConfig<ImediatusPermission, PermissionViewModel>.NewConfig().MapToConstructor(true);

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;

        _canEditRoleClaims = await AuthService.HasPermissionAsync(state.User, ImediatusActions.Update, ImediatusResources.RoleClaims);
        _canSearchRoleClaims = await AuthService.HasPermissionAsync(state.User, ImediatusActions.View, ImediatusResources.RoleClaims);

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => RolesClient.GetRolePermissionsEndpointAsync(Id), Toast, Navigation)
            is RoleDto role && role.Permissions is not null)
        {
            Title = string.Format("{0} Permissions", role.Name);
            Description = string.Format("Manage {0} Role Permissions", role.Name);

            var permissions = state.User.GetTenant() == TenantConstants.Root.Id
                ? ImediatusPermissions.All
                : ImediatusPermissions.Admin;

            _groupedRoleClaims = permissions
                .GroupBy(p => p.Resource)
                .ToDictionary(g => g.Key, g => g.Select(p =>
                {
                    var permission = p.Adapt<PermissionViewModel>();
                    permission.Enabled = role.Permissions.Contains(permission.Name);
                    return permission;
                }).ToList());
        }

        _loaded = true;
    }

    private Color GetGroupBadgeColor(int selected, int all)
    {
        if (selected == 0)
            return Color.Error;

        if (selected == all)
            return Color.Success;

        return Color.Info;
    }

    private async Task SaveAsync()
    {
        var allPermissions = _groupedRoleClaims.Values.SelectMany(a => a);
        var selectedPermissions = allPermissions.Where(a => a.Enabled);
        var request = new UpdatePermissionsCommand()
        {
            RoleId = Id,
            Permissions = selectedPermissions.Where(x => x.Enabled).Select(x => x.Name).ToList(),
        };
        await ApiHelper.ExecuteCallGuardedAsync(
                () => RolesClient.UpdateRolePermissionsEndpointAsync(request.RoleId, request),
                Toast,
                successMessage: "Updated Permissions.");
        Navigation.NavigateTo("/identity/roles");
    }

    private bool Search(PermissionViewModel permission) =>
        string.IsNullOrWhiteSpace(_searchString)
            || permission.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true
            || permission.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true;
}

public record PermissionViewModel : ImediatusPermission
{
    public bool Enabled { get; set; }

    public PermissionViewModel(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
        : base(Description, Action, Resource, IsBasic, IsRoot)
    {
    }
}
