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

/*

public sealed class PortfolioClassification
{
    public static readonly PortfolioClassification Public = new PortfolioClassification("Public", 1);
    public static readonly PortfolioClassification Internal = new PortfolioClassification("Internal", 2);
    public static readonly PortfolioClassification Confidential = new PortfolioClassification("Confidential", 3);
    public static readonly PortfolioClassification Restricted = new PortfolioClassification("Restricted", 4);
    public static readonly PortfolioClassification Secret = new PortfolioClassification("Secret", 5);
    public static readonly PortfolioClassification TopSecret = new PortfolioClassification("Top Secret", 6);

    public string Name { get; private set; }
    public int Value { get; private set; }

    private PortfolioClassification(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static IEnumerable<PortfolioClassification> List() =>
        new[] { Public, Internal, Confidential, Restricted, Secret, TopSecret };

    public static PortfolioClassification FromName(string name)
    {
        return List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase))
               ?? throw new ArgumentException($"Invalid name: {name}");
    }

    public static PortfolioClassification FromValue(int value)
    {
        return List().SingleOrDefault(s => s.Value == value)
               ?? throw new ArgumentException($"Invalid value: {value}");
    }

    public override string ToString() => Name;
}
*/
