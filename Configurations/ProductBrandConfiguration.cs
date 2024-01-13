using ApiAryanakala.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiAryanakala.Configurations;

public class ProductBrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Products)
           .WithOne(x => x.Brand)
           .HasForeignKey(x => x.BrandId);

    }
}