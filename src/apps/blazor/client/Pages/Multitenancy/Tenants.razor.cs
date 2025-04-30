using System.Linq;
using imediatus.Blazor.Client.Components;
using imediatus.Blazor.Client.Components.EntityTable;
using imediatus.Blazor.Infrastructure.Api;
using imediatus.Blazor.Infrastructure.Auth;
using imediatus.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace imediatus.Blazor.Client.Pages.Multitenancy;

public partial class Tenants
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;
    private string? _searchString;
    protected EntityClientTableContext<TenantViewModel, Guid, CreateTenantCommand> Context { get; set; } = default!;
    private List<TenantViewModel> _tenants = new();
    public EntityTable<TenantViewModel, Guid, CreateTenantCommand> EntityTable { get; set; } = default!;
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    private bool _canUpgrade;
    private bool _canModify;

    protected override async Task OnInitializedAsync()
    {
        Context = new(
            entityName: "Tenant",
            entityNamePlural: "Tenants",
            entityResource: ImediatusResources.Tenants,
            searchAction: ImediatusActions.View,
            deleteAction: string.Empty,
            updateAction: string.Empty,
            fields: new()
            {
                new(tenant => tenant.Id, "Id"),
                new(tenant => tenant.Name, "Name"),
                new(tenant => tenant.AdminEmail, "Admin Email"),
                new(tenant => tenant.ValidUpto.ToString("MMM dd, yyyy"), "Valid Upto"),
                new(tenant => tenant.IsActive, "Active", Type: typeof(bool))
            },
            loadDataFunc: async () => _tenants = (await ApiClient.GetTenantsEndpointAsync()).Adapt<List<TenantViewModel>>(),
            searchFunc: (searchString, tenantDto) =>
                string.IsNullOrWhiteSpace(searchString)
                    || tenantDto.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase),
            createFunc: tenant => ApiClient.CreateTenantEndpointAsync(tenant.Adapt<CreateTenantCommand>()),
            hasExtraActionsFunc: () => true,
            exportAction: string.Empty);

        var state = await AuthState;
        _canUpgrade = await AuthService.HasPermissionAsync(state.User, ImediatusActions.UpgradeSubscription, ImediatusResources.Tenants);
        _canModify = await AuthService.HasPermissionAsync(state.User, ImediatusActions.Update, ImediatusResources.Tenants);
    }

    private void ViewTenantDetails(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            _tenants.ForEach(f => f.ShowDetails = false);
        }
        else
        {
            var tenant = _tenants.First(f => f.Id == id);
            tenant.ShowDetails = !tenant.ShowDetails;
            _tenants.Except(new[] { tenant }).ToList().ForEach(f => f.ShowDetails = false);
        }
    }

    private async Task ViewUpgradeSubscriptionModalAsync(string id)
    {
        var tenant = _tenants.First(f => f.Id == id);
        var parameters = new DialogParameters
        {
            {
                nameof(UpgradeSubscriptionModal.Request),
                new UpgradeSubscriptionCommand
                {
                    Tenant = tenant.Id,
                    ExtendedExpiryDate = tenant.ValidUpto
                }
            }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };
        var dialog = await DialogService.ShowAsync<UpgradeSubscriptionModal>("Upgrade Subscription", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await EntityTable.ReloadDataAsync();
        }
    }

    private async Task DeactivateTenantAsync(string id)
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => ApiClient.DisableTenantEndpointAsync(id),
            Toast, Navigation,
            null,
            "Tenant Deactivated.") is not null)
        {
            await EntityTable.ReloadDataAsync();
        }
    }

    private async Task ActivateTenantAsync(string id)
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => ApiClient.ActivateTenantEndpointAsync(id),
            Toast, Navigation,
            null,
            "Tenant Activated.") is not null)
        {
            await EntityTable.ReloadDataAsync();
        }
    }

    public class TenantViewModel : TenantDetail
    {
        public bool ShowDetails { get; set; }
    }
}
