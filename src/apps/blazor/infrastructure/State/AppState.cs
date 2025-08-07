using imediatus.Blazor.Infrastructure.Api;
using imediatus.Blazor.Infrastructure.Auth;
using imediatus.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace imediatus.Blazor.Infrastructure.State;

public class AppState
{
    [Inject]
    protected IApiClient Client { get; set; } = default!;

    public ICollection<CostCenterResponse>? CostCenters { get; private set; } = [];
    public ICollection<UserDetail> Users { get; private set; } = [];
    public ICollection<PortfolioPriority> Priorities { get; private set; } = [.. PortfolioPriority.List.OrderBy(o => o.Value)];
    public ICollection<PortfolioClassification> Classifications { get; private set; } = [.. PortfolioClassification.List.OrderBy(o => o.Value)];
    public required UserInfo LoggedUser { get; set; }
    public bool IsLoaded { get; private set; }

    public async Task InitializeAsync()
    {
        if (IsLoaded) return; // Já carregado, evita repetir chamadas

        Users = await Client.GetUsersListEndpointAsync();
        CostCenters = (await Client.SearchCostCentersEndpointAsync("1", new SearchCostCentersCommand() { PageSize = 50 })).Items;

        IsLoaded = true;
    }
}
