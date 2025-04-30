using Microsoft.AspNetCore.Identity;

namespace imediatus.Framework.Infrastructure.Identity.Roles;
public class ImediatusRole : IdentityRole
{
    public string? Description { get; set; }

    public ImediatusRole(string name, string? description = null)
        : base(name)
    {
        Description = description;
        NormalizedName = name.ToUpperInvariant();
    }
}
