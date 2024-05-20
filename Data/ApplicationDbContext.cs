using System.Reflection;
using ApiAryanakala.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;
using ApiAryanakala.Entities.User;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Entities.User.Security;

namespace ApiAryanakala.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Category>()
        .HasMany(p => p.Products)
        .WithOne(p => p.Category)
        .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Category>(category =>
        {
            category.HasKey(c => c.Id);
            category.HasIndex(c => c.ParentCategoryId);
            category.OwnsOne(p => p.Colors);

            category.HasOne(c => c.ParentCategory)
            .WithMany(c => c.ChildCategories)
            .HasForeignKey(c => c.ParentCategoryId);
        });

        modelBuilder.Entity<CartItem>()
       .HasKey(ci => new { ci.UserId, ci.ProductId, ci.CategoryId });

        modelBuilder.Entity<OrderItem>()
       .HasKey(oi => new { oi.OrderId, oi.ProductId, oi.CategoryId });

        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductBrandConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRefreshTokenEntityConfiguration());
        // modelBuilder.ApplyConfiguration(new DetailsConfiguration());

    }

    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<EntityImage<int, Category>> CategoryImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, Product>> ProductImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, Slider>> SliderImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, Banner>> BannerImages { get; set; } = default!;
    public DbSet<Slider> Sliders { get; set; } = default!;
    public DbSet<Banner> Banners { get; set; } = default!;
    // public DbSet<Details> Details { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Brand> Brands { get; set; } = default!;
    public DbSet<ProductInfo> ProductInformation { get; set; } = default!;
    public DbSet<ProductSpecification> ProductSpecification { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Address> Addresses { get; set; } = default!;
    public DbSet<Order> Orders { get; set; } = default!;
    public DbSet<CartItem> CartItems { get; set; } = default!;
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; } = default!;
    public DbSet<Permission> Permissions { get; set; } = default!;
    public DbSet<Role> Roles { get; set; } = default!;
    public DbSet<RolePermission> RolePermissions { get; set; } = default!;
    public DbSet<UserRole> UserRoles { get; set; } = default!;
}