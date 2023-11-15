using System.Reflection;
using ApiAryanakala.Configurations;
using ApiAryanakala.Entities;
using ApiAryanakala.Entities.Security;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;

namespace ApiAryanakala.Data;

public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Info> Info => Set<Info>();
    public DbSet<User> Users => Set<User>();
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
        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRefreshTokenEntityConfiguration());
    }

}