using ApiAryanakala.Entities.Product;
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
              .WithOne(x => x.Product)
              .HasForeignKey(x => x.ProductId);

              builder.HasMany(x => x.Specifications)
               .WithOne(x => x.Product)
              .HasForeignKey(x => x.ProductId);

              builder.HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

              builder.OwnsOne(p => p.CategoryLevels);

              builder.HasOne(x => x.Brand)
                  .WithMany(x => x.Products)
                 .HasForeignKey(x => x.BrandId);

              builder.HasMany(x =>x.Review)
                     .WithOne(x=>x.Product)
                     .HasForeignKey(x=>x.ProductId);

              builder.Property(p => p.OptionType)
              .HasConversion<string>();

              builder.Navigation(p => p.Category)
              .UsePropertyAccessMode(PropertyAccessMode.Property);

              builder.ToTable("Products");
       }
}