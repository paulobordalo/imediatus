using Ardalis.SmartEnum;

namespace imediatus.Shared.Enums;

public sealed class PortfolioSection : SmartEnum<PortfolioSection>
{
    public static readonly PortfolioSection Backlog = new(nameof(Backlog), 1);
    public static readonly PortfolioSection Working = new(nameof(Working), 2);
    public static readonly PortfolioSection Accepting = new(nameof(Accepting), 3);
    public static readonly PortfolioSection Blocked = new(nameof(Blocked), 4);
    public static readonly PortfolioSection Ready = new(nameof(Ready), 5);

    private PortfolioSection(string name, int value) : base(name, value)
    {
    }
}
