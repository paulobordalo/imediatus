using imediatus.Framework.Core.Identity.Tokens.Features.Generate;
using imediatus.Framework.Core.Identity.Tokens.Features.Refresh;
using imediatus.Framework.Core.Identity.Tokens.Models;

namespace imediatus.Framework.Core.Identity.Tokens;
public interface ITokenService
{
    Task<TokenResponse> GenerateTokenAsync(TokenGenerationCommand request, string ipAddress, CancellationToken cancellationToken);
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenCommand request, string ipAddress, CancellationToken cancellationToken);

}
