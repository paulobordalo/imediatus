using MediatR;

namespace imediatus.Framework.Core.Tenant.Features.ActivateTenant;
public record ActivateTenantCommand(string TenantId) : IRequest<ActivateTenantResponse>;
