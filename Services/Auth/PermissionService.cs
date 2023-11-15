using ApiAryanakala.Data;
using ApiAryanakala.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ApiAryanakala.Services.Auth;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext db;
    private readonly IDistributedCache distributedCache;

    public PermissionService(ApplicationDbContext onlineShopDbContext, IDistributedCache distributedCache)
    {
        this.db = onlineShopDbContext;
        this.distributedCache = distributedCache;
    }

    public async Task<bool> CheckPermission(Guid userId, string permissionFlag)
    {
        string permissionCacheKey = $"permissions-{userId.ToString()}";
        var permissionFlags = new List<string>();

        // Try to get data from cache
        var cachedData = await distributedCache.GetStringAsync(permissionCacheKey);
        if (cachedData != null)
        {
            // Data found in cache, deserialize it.
            permissionFlags = JsonConvert.DeserializeObject<List<string>>(cachedData);
        }
        else
        {
            // Key not in cache, so get data from the database.
            var roles = await db.UserRoles.Where(q => q.UserId == userId)
                .Select(q => q.RoleId).ToListAsync();

            permissionFlags = await db.RolePermissions
                .Where(q => roles.Contains(q.RoleId))
                .Select(q => q.Permission.PermissionFlag)
                .ToListAsync();

            // Serialize data and save it in the cache.
            var serializedData = JsonConvert.SerializeObject(permissionFlags);
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            };
            await distributedCache.SetStringAsync(permissionCacheKey, serializedData, cacheEntryOptions);
        }

        return permissionFlags.Contains(permissionFlag);
    }
}
