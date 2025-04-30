using System.Security.Claims;

namespace imediatus.Framework.Core.Identity.Users.Abstractions;
public interface ICurrentUserInitializer
{
    void SetCurrentUser(ClaimsPrincipal user);

    void SetCurrentUserId(string userId);
}
