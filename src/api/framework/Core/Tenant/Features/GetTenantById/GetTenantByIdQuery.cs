using imediatus.Framework.Core.Tenant.Dtos;
using MediatR;

namespace imediatus.Framework.Core.Tenant.Features.GetTenantById;
public record GetTenantByIdQuery(string TenantId) : IRequest<TenantDetail>;
