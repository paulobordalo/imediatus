using Finbuckle.MultiTenant.Abstractions;

namespace imediatus.Framework.Infrastructure.Tenant.Abstractions;
public interface IImediatusTenantInfo : ITenantInfo
{
    string? ConnectionString { get; set; }
}
