using imediatus.Framework.Core.Tenant.Dtos;
using MediatR;

namespace imediatus.Framework.Core.Tenant.Features.GetTenants;
public sealed class GetTenantsQuery : IRequest<List<TenantDetail>>;
