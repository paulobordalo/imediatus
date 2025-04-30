using Microsoft.AspNetCore.Identity;

namespace imediatus.Framework.Infrastructure.Identity.RoleClaims;
public class ImediatusRoleClaim : IdentityRoleClaim<string>
{
    public string? CreatedBy { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
}
