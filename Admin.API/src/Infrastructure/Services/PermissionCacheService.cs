/*
 * 文件名称: PermissionCacheService.cs
 * 功能描述: 权限缓存服务实现,提供权限缓存操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-07
 */

using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <inheritdoc cref="IPermissionCacheService"/>
/// <param name="cacheProvider">缓存提供者</param>
/// <param name="logger">日志记录器</param>
public class PermissionCacheService(ICacheProvider cacheProvider, ILogger<PermissionCacheService> logger) : IPermissionCacheService {
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private readonly ILogger<PermissionCacheService> _logger = logger;
    private const string CACHE_KEY_PREFIX = "user_permissions:";
    private static readonly TimeSpan DEFAULT_EXPIRATION = TimeSpan.FromMinutes(30);

    /// <inheritdoc/>
    public async Task<HashSet<string>?> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default) {
        var cacheKey = $"{CACHE_KEY_PREFIX}{userId}";
        var permissions = await _cacheProvider.GetAsync<HashSet<string>>(cacheKey);
        
        if (permissions != null) {
            _logger.LogDebug("从缓存获取用户权限成功 | UserId: {UserId} | PermissionCount: {Count}", userId, permissions.Count);
        }
        
        return permissions;
    }

    /// <inheritdoc/>
    public async Task<bool> SetUserPermissionsAsync(Guid userId, HashSet<string> permissions, TimeSpan? expiration = null, CancellationToken cancellationToken = default) {
        var cacheKey = $"{CACHE_KEY_PREFIX}{userId}";
        var expireTime = expiration ?? DEFAULT_EXPIRATION;
        
        try {
            await _cacheProvider.SetAsync(cacheKey, permissions, expireTime);
            _logger.LogInformation("设置用户权限缓存成功 | UserId: {UserId} | PermissionCount: {Count} | Expiration: {Expiration}", 
                userId, permissions.Count, expireTime);
            return true;
        } catch (Exception ex) {
            _logger.LogWarning(ex, "设置用户权限缓存失败 | UserId: {UserId}", userId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default) {
        var cacheKey = $"{CACHE_KEY_PREFIX}{userId}";
        
        try {
            await _cacheProvider.RemoveAsync(cacheKey);
            _logger.LogInformation("移除用户权限缓存成功 | UserId: {UserId}", userId);
            return true;
        } catch (Exception ex) {
            _logger.LogWarning(ex, "移除用户权限缓存失败 | UserId: {UserId}", userId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveAllUserPermissionsAsync(CancellationToken cancellationToken = default) {
        var pattern = $"{CACHE_KEY_PREFIX}*";
        
        try {
            var count = await _cacheProvider.RemoveByPatternAsync(pattern);
            _logger.LogInformation("移除所有用户权限缓存成功 | Count: {Count}", count);
            return true;
        } catch (Exception ex) {
            _logger.LogWarning(ex, "移除所有用户权限缓存失败");
            return false;
        }
    }
}
