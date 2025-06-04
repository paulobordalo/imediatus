using Finbuckle.MultiTenant;
using imediatus.WebApi.Workspace.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace imediatus.WebApi.Workspace.Infrastructure.Persistence.Configurations;

internal sealed class CostCenterConfiguration : IEntityTypeConfiguration<CostCenter>
{
    public void Configure(EntityTypeBuilder<CostCenter> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired();
    }
}
