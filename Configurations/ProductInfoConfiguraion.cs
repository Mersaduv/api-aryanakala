using ApiAryanakala.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiAryanakala.Configurations;

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Products)
           .WithMany(x => x.ProductAttribute)
           .HasForeignKey(x => x.ProductId);
    }

}