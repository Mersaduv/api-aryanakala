using ApiAryanakala.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiAryanakala.Configurations;

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.ProductsInfo)
           .WithMany(x => x.Info)
           .HasForeignKey(x => x.ProductId);

        builder.HasOne(x => x.ProductsSpecification)
           .WithMany(x => x.Specification)
           .HasForeignKey(x => x.ProductId);
    }

}