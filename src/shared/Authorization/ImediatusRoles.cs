using System.Collections.ObjectModel;

namespace imediatus.Shared.Authorization;

public static class ImediatusRoles
{
    public const string Admin = nameof(Admin);
    public const string Basic = nameof(Basic);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(
    [
        Admin,
        Basic
    ]);

    public static bool IsDefault(string roleName)
    {
        return DefaultRoles.Any(r => r == roleName);
    }
}
