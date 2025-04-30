using imediatus.Blazor.Infrastructure.Themes;

namespace imediatus.Blazor.Infrastructure.Preferences;

public class ClientPreference : IPreference
{
    public bool IsDarkMode { get; set; } = true;
    public bool IsRTL { get; set; }
    public bool IsDrawerOpen { get; set; }
    public string PrimaryColor { get; set; } = CustomColors.Light.Primary;
    public string SecondaryColor { get; set; } = CustomColors.Light.Secondary;
    public double BorderRadius { get; set; } = 5;
    public ImediatusTablePreference TablePreference { get; set; } = new ImediatusTablePreference();
}
