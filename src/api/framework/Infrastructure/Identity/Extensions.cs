using imediatus.Framework.Core.Audit;
using imediatus.Framework.Core.Identity.Roles;
using imediatus.Framework.Core.Identity.Tokens;
using imediatus.Framework.Core.Identity.Users.Abstractions;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Infrastructure.Auth;
using imediatus.Framework.Infrastructure.Identity.Audit;
using imediatus.Framework.Infrastructure.Identity.Persistence;
using imediatus.Framework.Infrastructure.Identity.Roles;
using imediatus.Framework.Infrastructure.Identity.Roles.Endpoints;
using imediatus.Framework.Infrastructure.Identity.Tokens;
using imediatus.Framework.Infrastructure.Identity.Tokens.Endpoints;
using imediatus.Framework.Infrastructure.Identity.Users;
using imediatus.Framework.Infrastructure.Identity.Users.Endpoints;
using imediatus.Framework.Infrastructure.Identity.Users.Services;
using imediatus.Framework.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using IdentityConstants = imediatus.Shared.Authorization.IdentityConstants;


namespace imediatus.Framework.Infrastructure.Identity;
internal static class Extensions
{
    internal static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped<CurrentUserMiddleware>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IAuditService, AuditService>();
        services.BindDbContext<IdentityDbContext>();
        services.AddScoped<IDbInitializer, IdentityDbInitializer>();
        services.AddIdentity<ImediatusUser, ImediatusRole>(options =>
           {
               options.Password.RequiredLength = IdentityConstants.PasswordLength;
               options.Password.RequireDigit = false;
               options.Password.RequireLowercase = false;
               options.Password.RequireNonAlphanumeric = false;
               options.Password.RequireUppercase = false;
               options.User.RequireUniqueEmail = true;
           })
           .AddEntityFrameworkStores<IdentityDbContext>()
           .AddDefaultTokenProviders();
        return services;
    }

    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("api/users").WithTags("users");
        users.MapUserEndpoints();

        var tokens = app.MapGroup("api/token").WithTags("token");
        tokens.MapTokenEndpoints();

        var roles = app.MapGroup("api/roles").WithTags("roles");
        roles.MapRoleEndpoints();

        return app;
    }
}
