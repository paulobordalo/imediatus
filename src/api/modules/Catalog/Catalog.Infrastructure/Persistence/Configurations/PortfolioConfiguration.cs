using Finbuckle.MultiTenant;
using imediatus.WebApi.Catalog.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace imediatus.WebApi.Catalog.Infrastructure.Persistence.Configurations;

internal sealed class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
{
    public void Configure(EntityTypeBuilder<Portfolio> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.StatusId).IsRequired();
        builder.Property(x => x.ClassificationId).IsRequired();
        builder.Property(x => x.PriorityId).IsRequired();
        builder.Property(x => x.ReporterId).IsRequired();
    }
}
