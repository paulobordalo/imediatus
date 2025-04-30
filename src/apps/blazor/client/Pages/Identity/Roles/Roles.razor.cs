using imediatus.Blazor.Client.Components.EntityTable;
using imediatus.Blazor.Infrastructure.Api;
using imediatus.Blazor.Infrastructure.Auth;
using imediatus.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace imediatus.Blazor.Client.Pages.Identity.Roles;

public partial class Roles
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    private IApiClient RolesClient { get; set; } = default!;

    protected EntityClientTableContext<RoleDto, string?, CreateOrUpdateRoleCommand> Context { get; set; } = default!;

    private bool _canViewRoleClaims;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canViewRoleClaims = await AuthService.HasPermissionAsync(state.User, ImediatusActions.View, ImediatusResources.RoleClaims);

        Context = new(
            entityName: "Role",
            entityNamePlural: "Roles",
            entityResource: ImediatusResources.Roles,
            searchAction: ImediatusActions.View,
            fields: new()
            {
                new(role => role.Id, "Id"),
                new(role => role.Name,"Name"),
                new(role => role.Description, "Description")
            },
            idFunc: role => role.Id,
            loadDataFunc: async () => (await RolesClient.GetRolesEndpointAsync()).ToList(),
            searchFunc: (searchString, role) =>
                string.IsNullOrWhiteSpace(searchString)
                    || role.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || role.Description?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            createFunc: async role => await RolesClient.CreateOrUpdateRoleEndpointAsync(role),
            updateFunc: async (_, role) => await RolesClient.CreateOrUpdateRoleEndpointAsync(role),
            deleteFunc: async id => await RolesClient.DeleteRoleEndpointAsync(id!),
            hasExtraActionsFunc: () => _canViewRoleClaims,
            canUpdateEntityFunc: e => !ImediatusRoles.IsDefault(e.Name!),
            canDeleteEntityFunc: e => !ImediatusRoles.IsDefault(e.Name!),
            exportAction: string.Empty);
    }

    private void ManagePermissions(string? roleId)
    {
        ArgumentNullException.ThrowIfNull(roleId, nameof(roleId));
        Navigation.NavigateTo($"/identity/roles/{roleId}/permissions");
    }
}
