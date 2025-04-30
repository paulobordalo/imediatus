using Finbuckle.MultiTenant.Abstractions;
using imediatus.Framework.Core.Origin;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Infrastructure.Identity.RoleClaims;
using imediatus.Framework.Infrastructure.Identity.Roles;
using imediatus.Framework.Infrastructure.Identity.Users;
using imediatus.Framework.Infrastructure.Tenant;
using imediatus.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IdentityConstants = imediatus.Shared.Authorization.IdentityConstants;

namespace imediatus.Framework.Infrastructure.Identity.Persistence;

internal sealed class IdentityDbInitializer(
    ILogger<IdentityDbInitializer> logger,
    IdentityDbContext context,
    RoleManager<ImediatusRole> roleManager,
    UserManager<ImediatusUser> userManager,
    TimeProvider timeProvider,
    IMultiTenantContextAccessor<ImediatusTenantInfo> multiTenantContextAccessor,
    IOptions<OriginOptions> originSettings) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{Tenant}] applied database migrations for identity module", context.TenantInfo?.Identifier);
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();
    }

    private async Task SeedRolesAsync()
    {
        foreach (string roleName in ImediatusRoles.DefaultRoles)
        {
            if (await roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
                is not ImediatusRole role)
            {
                // create role
                role = new ImediatusRole(roleName, $"{roleName} Role for {multiTenantContextAccessor.MultiTenantContext.TenantInfo?.Id} Tenant");
                await roleManager.CreateAsync(role);
            }

            // Assign permissions
            if (roleName == ImediatusRoles.Basic)
            {
                await AssignPermissionsToRoleAsync(context, ImediatusPermissions.Basic, role);
            }
            else if (roleName == ImediatusRoles.Admin)
            {
                await AssignPermissionsToRoleAsync(context, ImediatusPermissions.Admin, role);

                if (multiTenantContextAccessor.MultiTenantContext.TenantInfo?.Id == TenantConstants.Root.Id)
                {
                    await AssignPermissionsToRoleAsync(context, ImediatusPermissions.Root, role);
                }
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(IdentityDbContext dbContext, IReadOnlyList<ImediatusPermission> permissions, ImediatusRole role)
    {
        var currentClaims = await roleManager.GetClaimsAsync(role);
        var newClaims = permissions
            .Where(permission => !currentClaims.Any(c => c.Type == ImediatusClaims.Permission && c.Value == permission.Name))
            .Select(permission => new ImediatusRoleClaim
            {
                RoleId = role.Id,
                ClaimType = ImediatusClaims.Permission,
                ClaimValue = permission.Name,
                CreatedBy = "application",
                CreatedOn = timeProvider.GetUtcNow()
            })
            .ToList();

        foreach (var claim in newClaims)
        {
            logger.LogInformation("Seeding {Role} Permission '{Permission}' for '{TenantId}' Tenant.", role.Name, claim.ClaimValue, multiTenantContextAccessor.MultiTenantContext.TenantInfo?.Id);
            await dbContext.RoleClaims.AddAsync(claim);
        }

        // Save changes to the database context
        if (newClaims.Count != 0)
        {
            await dbContext.SaveChangesAsync();
        }

    }

    private async Task SeedAdminUserAsync()
    {
        if (string.IsNullOrWhiteSpace(multiTenantContextAccessor.MultiTenantContext.TenantInfo?.Id) || string.IsNullOrWhiteSpace(multiTenantContextAccessor.MultiTenantContext.TenantInfo?.AdminEmail))
        {
            return;
        }

        if (await userManager.Users.FirstOrDefaultAsync(u => u.Email == multiTenantContextAccessor.MultiTenantContext.TenantInfo!.AdminEmail)
            is not ImediatusUser adminUser)
        {
            string adminUserName = $"{multiTenantContextAccessor.MultiTenantContext.TenantInfo?.Id.Trim()}.{ImediatusRoles.Admin}".ToUpperInvariant();
            adminUser = new ImediatusUser
            {
                FirstName = multiTenantContextAccessor.MultiTenantContext.TenantInfo?.Id.Trim().ToUpperInvariant(),
                LastName = ImediatusRoles.Admin,
                Email = multiTenantContextAccessor.MultiTenantContext.TenantInfo?.AdminEmail,
                UserName = adminUserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = multiTenantContextAccessor.MultiTenantContext.TenantInfo?.AdminEmail!.ToUpperInvariant(),
                NormalizedUserName = adminUserName.ToUpperInvariant(),
                ImageUrl = new Uri(originSettings.Value.OriginUrl! + TenantConstants.Root.DefaultProfilePicture),
                IsActive = true
            };

            logger.LogInformation("Seeding Default Admin User for '{TenantId}' Tenant.", multiTenantContextAccessor.MultiTenantContext.TenantInfo?.Id);
            var password = new PasswordHasher<ImediatusUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, TenantConstants.DefaultPassword);
            await userManager.CreateAsync(adminUser);
        }

        // Assign role to user
        if (!await userManager.IsInRoleAsync(adminUser, ImediatusRoles.Admin))
        {
            logger.LogInformation("Assigning Admin Role to Admin User for '{TenantId}' Tenant.", multiTenantContextAccessor.MultiTenantContext.TenantInfo?.Id);
            await userManager.AddToRoleAsync(adminUser, ImediatusRoles.Admin);
        }
    }
}
