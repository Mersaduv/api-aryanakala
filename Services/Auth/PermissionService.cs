using ApiAryanakala.Data;
using ApiAryanakala.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ApiAryanakala.Services.Auth;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _db;
    private readonly IMemoryCache _cache;

    public PermissionService(ApplicationDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<bool> CheckPermission(Guid userId, string permissionFlag)
    {
        string permissionCacheKey = $"permissions-{userId.ToString()}";
        var permissionFlags = new List<string>();

        // Try to get data from cache
        if (_cache.TryGetValue(permissionCacheKey, out List<string>? cachedPermissionFlags))
        {
            // Data found in cache
            permissionFlags = cachedPermissionFlags;
        }
        else
        {
            // Key not in cache, so get data from the database.
            var roles = await _db.UserRoles.Where(q => q.UserId == userId)
                .Select(q => q.RoleId).ToListAsync();

            permissionFlags = await _db.RolePermissions
                .Where(q => roles.Contains(q.RoleId))
                .Select(q => q.Permission!.PermissionFlag)
                .ToListAsync();

            // Save data in the cache.
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            };
            _cache.Set(permissionCacheKey, permissionFlags, cacheEntryOptions);
        }

        return permissionFlags!.Contains(permissionFlag);
    }
}
