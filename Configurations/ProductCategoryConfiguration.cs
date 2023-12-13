using ApiAryanakala.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiAryanakala.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Products)
           .WithOne(x => x.Category)
           .HasForeignKey(x => x.CategoryId);

    }
}