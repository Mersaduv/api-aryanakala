using ApiAryanakala.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiAryanakala.Configurations;

public class ProductInfoConfiguraion : IEntityTypeConfiguration<Info>
{
    public void Configure(EntityTypeBuilder<Info> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Products)
           .WithMany(x => x.Info)
           .HasForeignKey(x => x.ProductId);
    }

}