using Finbuckle.MultiTenant.Abstractions;
using imediatus.Framework.Core.Exceptions;
using imediatus.Framework.Core.Identity.Roles;
using imediatus.Framework.Core.Identity.Roles.Features.CreateOrUpdateRole;
using imediatus.Framework.Core.Identity.Roles.Features.UpdatePermissions;
using imediatus.Framework.Core.Identity.Users.Abstractions;
using imediatus.Framework.Core.Tenant;
using imediatus.Framework.Infrastructure.Identity.Persistence;
using imediatus.Framework.Infrastructure.Identity.RoleClaims;
using imediatus.Framework.Infrastructure.Tenant;
using imediatus.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace imediatus.Framework.Infrastructure.Identity.Roles;

public class RoleService(RoleManager<ImediatusRole> roleManager,
    IdentityDbContext context,
    IMultiTenantContextAccessor<ImediatusTenantInfo> multiTenantContextAccessor,
    ICurrentUser currentUser) : IRoleService
{
    private readonly RoleManager<ImediatusRole> _roleManager = roleManager;

    public async Task<IEnumerable<RoleDto>> GetRolesAsync()
    {
        return await Task.Run(() => _roleManager.Roles
            .Select(role => new RoleDto { Id = role.Id, Name = role.Name!, Description = role.Description })
            .ToList());
    }

    public async Task<RoleDto?> GetRoleAsync(string id)
    {
        ImediatusRole? role = await _roleManager.FindByIdAsync(id);

        _ = role ?? throw new NotFoundException("role not found");

        return new RoleDto { Id = role.Id, Name = role.Name!, Description = role.Description };
    }

    public async Task<RoleDto> CreateOrUpdateRoleAsync(CreateOrUpdateRoleCommand command)
    {
        ImediatusRole? role = await _roleManager.FindByIdAsync(command.Id);

        if (role != null)
        {
            role.Name = command.Name;
            role.Description = command.Description;
            await _roleManager.UpdateAsync(role);
        }
        else
        {
            role = new ImediatusRole(command.Name, command.Description);
            await _roleManager.CreateAsync(role);
        }

        return new RoleDto { Id = role.Id, Name = role.Name!, Description = role.Description };
    }

    public async Task DeleteRoleAsync(string id)
    {
        ImediatusRole? role = await _roleManager.FindByIdAsync(id);

        _ = role ?? throw new NotFoundException("role not found");

        await _roleManager.DeleteAsync(role);
    }

    public async Task<RoleDto> GetWithPermissionsAsync(string id, CancellationToken cancellationToken)
    {
        var role = await GetRoleAsync(id);
        _ = role ?? throw new NotFoundException("role not found");

        role.Permissions = await context.RoleClaims
            .Where(c => c.RoleId == id && c.ClaimType == ImediatusClaims.Permission)
            .Select(c => c.ClaimValue!)
            .ToListAsync(cancellationToken);

        return role;
    }

    public async Task<string> UpdatePermissionsAsync(UpdatePermissionsCommand request)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId);
        _ = role ?? throw new NotFoundException("role not found");
        if (role.Name == ImediatusRoles.Admin)
        {
            throw new ImediatusException("operation not permitted");
        }

        if (multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Id != TenantConstants.Root.Id)
        {
            // Remove Root Permissions if the Role is not created for Root Tenant.
            request.Permissions.RemoveAll(u => u.StartsWith("Permissions.Root.", StringComparison.InvariantCultureIgnoreCase));
        }

        var currentClaims = await _roleManager.GetClaimsAsync(role);

        // Remove permissions that were previously selected
        foreach (var claim in currentClaims.Where(c => !request.Permissions.Exists(p => p == c.Value)))
        {
            var result = await _roleManager.RemoveClaimAsync(role, claim);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(error => error.Description).ToList();
                throw new ImediatusException("operation failed", errors);
            }
        }

        // Add all permissions that were not previously selected
        foreach (string permission in request.Permissions.Where(c => !currentClaims.Any(p => p.Value == c)))
        {
            if (!string.IsNullOrEmpty(permission))
            {
                context.RoleClaims.Add(new ImediatusRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = ImediatusClaims.Permission,
                    ClaimValue = permission,
                    CreatedBy = currentUser.GetUserId().ToString()
                });
                await context.SaveChangesAsync();
            }
        }

        return "permissions updated";
    }
}
