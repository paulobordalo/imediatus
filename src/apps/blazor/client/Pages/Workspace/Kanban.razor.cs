using imediatus.Blazor.Client.Components;
using imediatus.Blazor.Client.Components.Dialogs;
using imediatus.Blazor.Client.Components.EntityTable;
using imediatus.Blazor.Infrastructure.Api;
using imediatus.Blazor.Infrastructure.Helpers;
using imediatus.Shared.Enums;
using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace imediatus.Blazor.Client.Pages.Workspace;

public partial class Kanban
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    bool success;
    string[] errors = { };
    MudForm form;

    private MudDropContainer<PortfolioItem> _dropContainer;

    private List<PortfolioItem> _portfolios = new List<PortfolioItem>();

    protected override async Task OnInitializedAsync()
    {
        var portfolioFilter = new SearchPortfoliosCommand
        {
            PageSize = 50
        };
        if (await ApiHelper.ExecuteCallGuardedAsync(() => _client.SearchPortfoliosEndpointAsync("1", portfolioFilter), Toast, Navigation) is PortfolioResponsePagedList response)
        {
            var portfolios = response.Adapt<PaginationResponse<PortfolioResponse>>();

            foreach (var portfolio in portfolios.Items)
            {
                _portfolios.Add(new PortfolioItem() { Id = portfolio.Id.HasValue ? portfolio.Id.Value : Guid.NewGuid(), SecondaryKey = portfolio.SecondaryKey, Summary = portfolio.Summary, Status = PortfolioStatus.FromValue(portfolio.StatusId).Name, Priority = PortfolioPriority.FromValue(portfolio.PriorityId).Name, Selected = MudBlazorHelper.GetPortfolioStatusSelected(PortfolioStatus.FromValue(portfolio.StatusId)) , Color = MudBlazorHelper.GetPortfolioStatusColor(PortfolioStatus.FromValue(portfolio.StatusId)), Selector = PortfolioStatus.FromValue(portfolio.StatusId).Section });
            }
        }
    }

    private static void ItemUpdated(MudItemDropInfo<PortfolioItem> portfolioItem)
    {
        if (portfolioItem.Item != null) // Ensure Item is not null before dereferencing
        {
            portfolioItem.Item.Selector = portfolioItem.DropzoneIdentifier;
        }
    }

    public class PortfolioItem
    {
        public Guid Id { get; init; }
        public int SecondaryKey { get; init; }
        public string? Summary { get; init; }
        public string? Status { get; init; }
        public string? Priority { get; init; }
        public string? CostCenter { get; init; }
        public bool Selected { get; set; }
        public MudBlazor.Color? Color { get; init; }
        public string? Selector { get; set; }
    }

    private NewPortfolioItem _newPortfolioItem = new NewPortfolioItem();

    public class NewPortfolioItem
    {
        public bool NewPortfolioOpen { get; set; }
        public string? NewPortfolioName { get; set; }
    }

    private async Task AddPortfolioAsync()
    {
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<Components.Dialogs.Portfolio>("Portfolio Dialog", options);
        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            _dropContainer.Refresh();
        }
    }

    private async Task DeletePortfolioAsync(PortfolioItem portfolioItem)
    {
        string deleteContent = $"Are you sure you want to delete this Portfolio - {portfolioItem.SecondaryKey}?";
        var parameters = new DialogParameters
        {
            { nameof(DeleteConfirmation.ContentText), deleteContent }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>("Delete", parameters, options);
        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            if (await ApiHelper.ExecuteCallGuardedAsync(() => _client.DeletePortfolioEndpointAsync("1", portfolioItem.Id), Toast))
            {
                Toast.Add("Portfolio deleted successfully.", Severity.Error);
            }

            _portfolios.Remove(portfolioItem);
            _newPortfolioItem.NewPortfolioName = string.Empty;
            _newPortfolioItem.NewPortfolioOpen = false;
            _dropContainer.Refresh();
        }
    }
}
