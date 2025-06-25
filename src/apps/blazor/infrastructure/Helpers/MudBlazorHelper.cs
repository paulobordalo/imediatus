using imediatus.Shared.Enums;
using MudBlazor;

namespace imediatus.Blazor.Infrastructure.Helpers;

public static class MudBlazorHelper
{
    public static Color GetPortfolioStatusColor(PortfolioStatus status)
    {
        if (status == PortfolioStatus.ToDo)
        {
            return Color.Default;
        }

        if (status == PortfolioStatus.Development)
        {
            return Color.Info;
        }

        if (status == PortfolioStatus.InReview)
        {
            return Color.Info;
        }

        if (status == PortfolioStatus.PendingApproval)
        {
            return Color.Warning;
        }

        if (status == PortfolioStatus.Approved)
        {
            return Color.Primary;
        }

        if (status == PortfolioStatus.Rejected)
        {
            return Color.Error;
        }

        if (status == PortfolioStatus.InProgress)
        {
            return Color.Info;
        }

        if (status == PortfolioStatus.OnHold)
        {
            return Color.Warning;
        }

        if (status == PortfolioStatus.Deleted)
        {
            return Color.Error;
        }

        if (status == PortfolioStatus.Archive)
        {
            return Color.Error;
        }

        if (status == PortfolioStatus.Done)
        {
            return Color.Primary;
        }

        return Color.Default;
    }

    public static bool GetPortfolioStatusSelected(PortfolioStatus status)
    {
        if (status == PortfolioStatus.ToDo)
        {
            return false;
        }

        if (status == PortfolioStatus.Development)
        {
            return false;
        }

        if (status == PortfolioStatus.InReview)
        {
            return true;
        }

        if (status == PortfolioStatus.PendingApproval)
        {
            return true;
        }

        if (status == PortfolioStatus.Approved)
        {
            return false;
        }

        if (status == PortfolioStatus.Rejected)
        {
            return false;
        }

        if (status == PortfolioStatus.InProgress)
        {
            return true;
        }

        if (status == PortfolioStatus.OnHold)
        {
            return true;
        }

        if (status == PortfolioStatus.Deleted)
        {
            return true;
        }

        if (status == PortfolioStatus.Archive)
        {
            return true;
        }

        if (status == PortfolioStatus.Done)
        {
            return false;
        }

        return true;
    }

    public static string GetPortfolioPriorityIcon(PortfolioPriority priority)
    {
        return priority.Name switch
        {
            "Low" => Icons.Material.Filled.KeyboardDoubleArrowDown,
            "Medium" => Icons.Material.Filled.KeyboardDoubleArrowRight,
            "High" => Icons.Material.Filled.KeyboardDoubleArrowUp,
            _ => Icons.Material.Filled.HelpOutline
        };
    }

    public static Color GetPortfolioPriorityColor(PortfolioPriority priority)
    {
        return priority.Name switch
        {
            "Low" => Color.Info,
            "Medium" => Color.Warning,
            "High" => Color.Error,
            _ => Color.Default
        };
    }

    public static string GetPortfolioClassificationIcon(PortfolioClassification classification)
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

    public static Color GetPortfolioClassificationColor(PortfolioClassification classification)
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
}
