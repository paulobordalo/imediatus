using System.Collections.ObjectModel;

namespace imediatus.Shared.Authorization;


public static class ImediatusPermissions
{
    private static readonly ImediatusPermission[] AllPermissions =
    [     
        //tenants
        new("View Tenants", ImediatusActions.View, ImediatusResources.Tenants, IsRoot: true),
        new("Create Tenants", ImediatusActions.Create, ImediatusResources.Tenants, IsRoot: true),
        new("Update Tenants", ImediatusActions.Update, ImediatusResources.Tenants, IsRoot: true),
        new("Upgrade Tenant Subscription", ImediatusActions.UpgradeSubscription, ImediatusResources.Tenants, IsRoot: true),

        //identity
        new("View Users", ImediatusActions.View, ImediatusResources.Users),
        new("Search Users", ImediatusActions.Search, ImediatusResources.Users),
        new("Create Users", ImediatusActions.Create, ImediatusResources.Users),
        new("Update Users", ImediatusActions.Update, ImediatusResources.Users),
        new("Delete Users", ImediatusActions.Delete, ImediatusResources.Users),
        new("Export Users", ImediatusActions.Export, ImediatusResources.Users),
        new("View UserRoles", ImediatusActions.View, ImediatusResources.UserRoles),
        new("Update UserRoles", ImediatusActions.Update, ImediatusResources.UserRoles),
        new("View Roles", ImediatusActions.View, ImediatusResources.Roles),
        new("Create Roles", ImediatusActions.Create, ImediatusResources.Roles),
        new("Update Roles", ImediatusActions.Update, ImediatusResources.Roles),
        new("Delete Roles", ImediatusActions.Delete, ImediatusResources.Roles),
        new("View RoleClaims", ImediatusActions.View, ImediatusResources.RoleClaims),
        new("Update RoleClaims", ImediatusActions.Update, ImediatusResources.RoleClaims),
        
        //products
        new("View Products", ImediatusActions.View, ImediatusResources.Products, IsBasic: true),
        new("Search Products", ImediatusActions.Search, ImediatusResources.Products, IsBasic: true),
        new("Create Products", ImediatusActions.Create, ImediatusResources.Products),
        new("Update Products", ImediatusActions.Update, ImediatusResources.Products),
        new("Delete Products", ImediatusActions.Delete, ImediatusResources.Products),
        new("Export Products", ImediatusActions.Export, ImediatusResources.Products),

        //brands
        new("View Brands", ImediatusActions.View, ImediatusResources.Brands, IsBasic: true),
        new("Search Brands", ImediatusActions.Search, ImediatusResources.Brands, IsBasic: true),
        new("Create Brands", ImediatusActions.Create, ImediatusResources.Brands),
        new("Update Brands", ImediatusActions.Update, ImediatusResources.Brands),
        new("Delete Brands", ImediatusActions.Delete, ImediatusResources.Brands),
        new("Export Brands", ImediatusActions.Export, ImediatusResources.Brands),

        //brands
        new("View Portfolios", ImediatusActions.View, ImediatusResources.Portfolios, IsBasic: true),
        new("Search Portfolios", ImediatusActions.Search, ImediatusResources.Portfolios, IsBasic: true),
        new("Create Portfolios", ImediatusActions.Create, ImediatusResources.Portfolios),
        new("Update Portfolios", ImediatusActions.Update, ImediatusResources.Portfolios),
        new("Delete Portfolios", ImediatusActions.Delete, ImediatusResources.Portfolios),
        new("Export Portfolios", ImediatusActions.Export, ImediatusResources.Portfolios),

        //todos
        new("View Todos", ImediatusActions.View, ImediatusResources.Todos, IsBasic: true),
        new("Search Todos", ImediatusActions.Search, ImediatusResources.Todos, IsBasic: true),
        new("Create Todos", ImediatusActions.Create, ImediatusResources.Todos),
        new("Update Todos", ImediatusActions.Update, ImediatusResources.Todos),
        new("Delete Todos", ImediatusActions.Delete, ImediatusResources.Todos),
        new("Export Todos", ImediatusActions.Export, ImediatusResources.Todos),

         new("View Hangfire", ImediatusActions.View, ImediatusResources.Hangfire),
         new("View Dashboard", ImediatusActions.View, ImediatusResources.Dashboard),

        //audit
        new("View Audit Trails", ImediatusActions.View, ImediatusResources.AuditTrails),
    ];

    public static IReadOnlyList<ImediatusPermission> All { get; } = new ReadOnlyCollection<ImediatusPermission>(AllPermissions);
    public static IReadOnlyList<ImediatusPermission> Root { get; } = new ReadOnlyCollection<ImediatusPermission>(AllPermissions.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<ImediatusPermission> Admin { get; } = new ReadOnlyCollection<ImediatusPermission>(AllPermissions.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<ImediatusPermission> Basic { get; } = new ReadOnlyCollection<ImediatusPermission>(AllPermissions.Where(p => p.IsBasic).ToArray());
}

public record ImediatusPermission(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource)
    {
        return $"Permissions.{resource}.{action}";
    }
}


