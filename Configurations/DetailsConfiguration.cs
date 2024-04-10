using ApiAryanakala.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiAryanakala.Configurations;

public class DetailsConfiguration : IEntityTypeConfiguration<Details>
{
       public void Configure(EntityTypeBuilder<Details> builder)
       {
              builder.HasKey(d => d.Id);

              builder.HasOne(d => d.Category)
                     .WithMany()
                     .HasForeignKey(d => d.CategoryId)
                     .OnDelete(DeleteBehavior.Restrict);
       }
}
