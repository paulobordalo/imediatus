using Ardalis.SmartEnum;

namespace imediatus.Shared.Enums;

public sealed class PortfolioClassification : SmartEnum<PortfolioClassification>
{
    public static readonly PortfolioClassification Public = new("Public", 1);
    public static readonly PortfolioClassification Internal = new("Internal", 2);
    public static readonly PortfolioClassification Confidential = new("Confidential", 3);
    public static readonly PortfolioClassification Restricted = new("Restricted", 4);
    public static readonly PortfolioClassification Secret = new("Secret", 5);
    public static readonly PortfolioClassification TopSecret = new("Top Secret", 6);

    private PortfolioClassification(string name, int value) : base(name, value)
    {
    }
}
