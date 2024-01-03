using System.Reflection;
using ApiAryanakala.Configurations;
using ApiAryanakala.Entities;
using ApiAryanakala.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;

namespace ApiAryanakala.Data;

public class ApplicationDbContext : DbContext
{
    protected ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.LogTo(Console.WriteLine);
#endif

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brand => Set<Brand>();
    public DbSet<ProductAttribute> ProductAttribute => Set<ProductAttribute>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

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

}