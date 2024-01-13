using System.Reflection;
using ApiAryanakala.Configurations;
using ApiAryanakala.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;
using ApiAryanakala.Entities.User;
using ApiAryanakala.Entities.Product;

namespace ApiAryanakala.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
    public virtual string GetPrimaryKey(Type entityModelType)
    {
        return this.Model
            .FindEntityType(entityModelType)
            .FindPrimaryKey().Properties
            .Select(x => x.Name).Single();
    }

    public static DbContext GetDbContext(IQueryable query)
    {
        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        var queryCompiler = typeof(EntityQueryProvider).GetField("_queryCompiler", bindingFlags).GetValue(query.Provider);
        var queryContextFactory = queryCompiler.GetType().GetField("_queryContextFactory", bindingFlags).GetValue(queryCompiler);

        var dependencies = typeof(RelationalQueryContextFactory).GetProperty("Dependencies", bindingFlags).GetValue(queryContextFactory);
        var queryContextDependencies = typeof(DbContext).Assembly.GetType(typeof(QueryContextDependencies).FullName);
        var stateManagerProperty = queryContextDependencies.GetProperty("StateManager", bindingFlags | BindingFlags.Public).GetValue(dependencies);
        var stateManager = (IStateManager)stateManagerProperty;

        return stateManager.Context;
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

            category.HasOne(c => c.ParentCategory)
            .WithMany(c => c.ChildCategories)
            .HasForeignKey(c => c.ParentCategoryId);
        });
        modelBuilder.Entity<CartItem>()
       .HasKey(ci => new { ci.UserId, ci.ProductId, ci.CategoryId });

        modelBuilder.Entity<OrderItem>()
       .HasKey(oi => new { oi.OrderId, oi.ProductId, oi.CategoryId });

        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAttributeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductBrandConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRefreshTokenEntityConfiguration());
    }

    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<ProductImage> ProductImages { get; set; } = default!;
    public DbSet<CategoryImage> CategoryImages { get; set; } = default!;
    public DbSet<Rating> Ratings { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Brand> Brands { get; set; } = default!;
    public DbSet<ProductAttribute> ProductAttributes { get; set; } = default!;
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