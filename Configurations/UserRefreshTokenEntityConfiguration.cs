using ApiAryanakala.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiAryanakala.Configurations
{
    public class UserRefreshTokenEntityConfiguration : IEntityTypeConfiguration<UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
        {
            builder.ToTable("UserRefreshTokens");
            builder.HasKey(s => s.Id);

            builder.Property(p => p.RefreshToken)
                  .HasMaxLength(128);
        }
    }

}