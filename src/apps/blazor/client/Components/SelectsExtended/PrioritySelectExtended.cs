using Microsoft.AspNetCore.Components;
using System.Reflection.Emit;
using MudBlazor;
using MudExtensions;
using imediatus.Shared.Enums;
using System.Linq;

namespace imediatus.Blazor.Client.Components.SelectsExtended;

public class PrioritySelectExtended : MudSelectExtended<int>
{
    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        ItemCollection = [.. PortfolioPriority.List.Select(p => p.Value)];
        ValuePresenter = ValuePresenter.ItemContent;
        MultiSelection = false;
        SearchBox = true;
        SearchBoxAutoFocus = true;
        SearchBoxClearable = true;
        Label = "Piority";
        AnchorOrigin = Origin.BottomCenter;
        return base.SetParametersAsync(parameters);
    }
}
