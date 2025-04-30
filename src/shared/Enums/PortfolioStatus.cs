using Ardalis.SmartEnum;

namespace imediatus.Shared.Enums;

public sealed class PortfolioStatus : SmartEnum<PortfolioStatus>
{
    public static readonly PortfolioStatus ToDo = new("To Do", 1, PortfolioSection.Backlog.Name);
    public static readonly PortfolioStatus Development = new("Development", 2, PortfolioSection.Working.Name);
    public static readonly PortfolioStatus InReview = new("In Review", 3, PortfolioSection.Working.Name);
    public static readonly PortfolioStatus PendingApproval = new("Pending Approval", 4, PortfolioSection.Accepting.Name);
    public static readonly PortfolioStatus Approved = new("Approved", 5, PortfolioSection.Ready.Name);
    public static readonly PortfolioStatus Rejected = new("Rejected", 6, PortfolioSection.Blocked.Name);
    public static readonly PortfolioStatus InProgress = new("In Progress", 7, PortfolioSection.Accepting.Name);
    public static readonly PortfolioStatus OnHold = new("On Hold", 8, PortfolioSection.Accepting.Name);
    public static readonly PortfolioStatus Deleted = new("Deleted", 9, PortfolioSection.Blocked.Name);
    public static readonly PortfolioStatus Archive = new("Archive", 10, PortfolioSection.Blocked.Name);
    public static readonly PortfolioStatus Done = new("Done", 11, PortfolioSection.Ready.Name);

    public string Section { get; private set; }

    private PortfolioStatus(string name, int value, string section) : base(name, value)
    {
        Section = section;
    }
}
