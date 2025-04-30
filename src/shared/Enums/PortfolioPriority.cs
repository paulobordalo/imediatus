using Ardalis.SmartEnum;

namespace imediatus.Shared.Enums;

public sealed class PortfolioPriority : SmartEnum<PortfolioPriority>
{
    public static readonly PortfolioPriority Low = new("Low", 1, "Low priority");
    public static readonly PortfolioPriority Medium = new("Medium", 2, "Medium priority");
    public static readonly PortfolioPriority High = new("High", 3, "High priority");

    public string Description { get; }

    private PortfolioPriority(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
