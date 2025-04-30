using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using imediatus.Framework.Core.Identity.Users.Abstractions;
using imediatus.Framework.Infrastructure.Constants;
using imediatus.Framework.Infrastructure.Tenant;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using imediatus.Shared.Authorization;

namespace imediatus.Framework.Infrastructure.Jobs;

public class ImediatusJobActivator : JobActivator
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ImediatusJobActivator(IServiceScopeFactory scopeFactory) =>
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));

    public override JobActivatorScope BeginScope(PerformContext context) =>
        new Scope(context, _scopeFactory.CreateScope());

    private sealed class Scope : JobActivatorScope, IServiceProvider
    {
        private readonly PerformContext _context;
        private readonly IServiceScope _scope;

        public Scope(PerformContext context, IServiceScope scope)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));

            ReceiveParameters();
        }

        private void ReceiveParameters()
        {
            var tenantInfo = _context.GetJobParameter<ImediatusTenantInfo>(TenantConstants.Identifier);
            if (tenantInfo is not null)
            {
                _scope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>()
                    .MultiTenantContext = new MultiTenantContext<ImediatusTenantInfo>
                    {
                        TenantInfo = tenantInfo
                    };
            }

            string userId = _context.GetJobParameter<string>(QueryStringKeys.UserId);
            if (!string.IsNullOrEmpty(userId))
            {
                _scope.ServiceProvider.GetRequiredService<ICurrentUserInitializer>()
                    .SetCurrentUserId(userId);
            }
        }

        public override object Resolve(Type type) =>
            ActivatorUtilities.GetServiceOrCreateInstance(this, type);

        object? IServiceProvider.GetService(Type serviceType) =>
            serviceType == typeof(PerformContext)
                ? _context
                : _scope.ServiceProvider.GetService(serviceType);
    }
}
