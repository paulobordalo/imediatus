using MediatR;

namespace imediatus.Framework.Core.Tenant.Features.DisableTenant;
public record DisableTenantCommand(string TenantId) : IRequest<DisableTenantResponse>;
