using imediatus.Blazor.Client.Components.EntityTable;
using imediatus.Blazor.Client.Pages.Identity.Roles;
using imediatus.Blazor.Infrastructure.Api;
using imediatus.Shared.Authorization;
using imediatus.Shared.Enums;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Syncfusion.Blazor.Diagrams;
using Syncfusion.Blazor.TreeMap.Internal;
using static imediatus.Blazor.Client.Pages.Workspace.Kanban;

namespace imediatus.Blazor.Client.Components.Dialogs;

public partial class Portfolio
{
    [Parameter]
    public Guid Id { get; set; }

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Inject]
    protected IApiClient _client { get; set; } = default!;

    private readonly List<CostCenterResponse> _costCenters = new List<CostCenterResponse>();

    private CostCenterResponse? _selectedCostCenter { get; set; }

    private PortfolioClassification _selectedClassification { get; set; }

    private PortfolioPriority _selectedPriority { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var costCenterFilter = new SearchCostCentersCommand
        {
            PageSize = 50
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(() => _client.SearchCostCentersEndpointAsync("1", costCenterFilter), Toast, Navigation) is CostCenterResponsePagedList response)
        {
            var costCenters = response.Adapt<PaginationResponse<CostCenterResponse>>();

            foreach (var costCenter in costCenters.Items)
            {
                _costCenters.Add(new CostCenterResponse() { Id = costCenter.Id.HasValue ? costCenter.Id.Value : Guid.NewGuid(), Name = costCenter.Name });
            }
        }
    }

    private void OnSelectedClassificationChanged(PortfolioClassification newValue)
    {
        _selectedClassification = newValue;
    }

    private void OnSelectedPriorityChanged(PortfolioPriority newValue)
    {
        _selectedPriority = newValue;
    }

    private void OnSelectedCostCenterChanged(CostCenterResponse newValue)
    {
        _selectedCostCenter = newValue;
    }

    private void Submit() => MudDialog.Close(DialogResult.Ok(true));

    private void Cancel() => MudDialog.Cancel();

    #nullable enable
    private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full";
    private string _dragClass = DefaultDragClass;
    private readonly List<string> _fileNames = new();
    private MudFileUpload<IReadOnlyList<IBrowserFile>>? _fileUpload;

    private async Task ClearAsync()
    {
        await (_fileUpload?.ClearAsync() ?? Task.CompletedTask);
        _fileNames.Clear();
        ClearDragClass();
    }

    private Task OpenFilePickerAsync()
    => _fileUpload?.OpenFilePickerAsync() ?? Task.CompletedTask;

    private void OnInputFileChanged(InputFileChangeEventArgs e)
    {
        ClearDragClass();
        var files = e.GetMultipleFiles();
        foreach (var file in files)
        {
            _fileNames.Add(file.Name);
        }
    }

    private void SetDragClass()
    => _dragClass = $"{DefaultDragClass} mud-border-primary";

    private void ClearDragClass()
    => _dragClass = DefaultDragClass;

    Tuple<string, string, bool>[] _users = new Tuple<string, string, bool>[]
    {
       new Tuple<string, string, bool>("Kareem Abdul-Jabbar", "Admin", true),
       new Tuple<string, string, bool>("LeBron James", "Admin", false),
       new Tuple<string, string, bool>("Karl Malone", "Basic", true),
       new Tuple<string, string, bool>("Kobe Bryant", "Admin", true),
       new Tuple<string, string, bool>("Michael Jordan", "Basic", true),
    };

    #region Prioridades

    private readonly List<PortfolioPriority> _priorities = [.. PortfolioPriority.List.OrderBy(o => o.Value)];

    private static string GetPriorityIcon(PortfolioPriority priority)
    {
        return priority.Name switch
        {
            "Low" => Icons.Material.Filled.KeyboardDoubleArrowDown,
            "Medium" => Icons.Material.Filled.KeyboardDoubleArrowRight,
            "High" => Icons.Material.Filled.KeyboardDoubleArrowUp,
            _ => Icons.Material.Filled.HelpOutline
        };
    }

    private static Color GetPriorityColor(PortfolioPriority priority)
    {
        return priority.Name switch
        {
            "Low" => Color.Info,
            "Medium" => Color.Warning,
            "High" => Color.Error,
            _ => Color.Default
        };
    }

    #endregion

    #region Classificações

    private readonly List<PortfolioClassification> _classifications = [.. PortfolioClassification.List.OrderBy(o => o.Value)];

    private static string GetClassificationIcon(PortfolioClassification classification)
    {
        return classification.Name switch
        {
            "Public" => Icons.Material.Filled.Public,
            "Internal" => Icons.Material.Filled.Work,
            "Confidential" => Icons.Material.Filled.Lock,
            "Restricted" => Icons.Material.Filled.Warning,
            "Secret" => Icons.Material.Filled.VisibilityOff,
            "Top Secret" => Icons.Material.Filled.Shield,
            _ => Icons.Material.Filled.HelpOutline
        };
    }

    private static Color GetClassificationColor(PortfolioClassification classification)
    {
        return classification.Name switch
        {
            "Public" => Color.Success,
            "Internal" => Color.Info,
            "Confidential" => Color.Warning,
            "Restricted" => Color.Error,
            "Secret" => Color.Dark,
            "Top Secret" => Color.Dark,
            _ => Color.Default
        };
    }

    #endregion
}
