namespace imediatus.Shared.Extensions;
public static class GuidExtensions
{
    public static bool IsGuid(string value)
    {
        return Guid.TryParse(value, out _);
    }
}
