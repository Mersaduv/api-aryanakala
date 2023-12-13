using ApiAryanakala.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiAryanakala.Configurations;

public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
       public void Configure(EntityTypeBuilder<Product> builder)
       {
              builder.HasKey(p => p.Id);

              builder.Property(p => p.Title)
                     .IsRequired()
                     .HasMaxLength(255);

              builder.Property(p => p.Price)
                     .IsRequired();

              builder.Property(p => p.Discount)
                .IsRequired(false);

              builder.HasMany(x => x.Info)
              .WithOne(x => x.Products)
              .HasForeignKey(x => x.ProductId);

              builder.HasOne(x => x.Category)
                  .WithMany(x => x.Products)
                 .HasForeignKey(x => x.CategoryId);

              builder.HasOne(x => x.Brand)
                  .WithMany(x => x.Products)
                 .HasForeignKey(x => x.BrandId);

              builder.ToTable("Products");
       }
}