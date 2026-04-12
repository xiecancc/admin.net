/*
 * 文件名称: PermissionCacheService.cs
 * 功能描述: 权限缓存服务实现,提供权限缓存操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Repositories;
using Domain.Services;
using Domain.Shared.Constants;
using Domain.Shared.Caches;
using Domain.Shared.Enums;
using Domain.Shared.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <inheritdoc cref="IPermissionCacheService"/>
/// <param name="cacheProvider">缓存提供者</param>
/// <param name="permissionDomainService">权限领域服务</param>
/// <param name="userRoleRepository">用户角色关联仓储</param>
/// <param name="roleRepository">角色仓储</param>
/// <param name="logger">日志记录器</param>
public class PermissionCacheService(
    ICacheProvider cacheProvider,
    IPermissionDomainService permissionDomainService,
    IUserRoleRepository userRoleRepository,
    IRoleRepository roleRepository,
    ILogger<PermissionCacheService> logger) : IPermissionCacheService {
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private readonly IPermissionDomainService _permissionDomainService = permissionDomainService;
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly ILogger<PermissionCacheService> _logger = logger;

    /// <inheritdoc/>
    public async Task<HashSet<string>?> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default) {
        var cacheKey = CacheKeyConstants.User.Permissions(userId);
        var permissions = await _cacheProvider.GetAsync<HashSet<string>>(cacheKey);

        if (permissions != null) {
            _logger.LogDebug("从缓存获取用户权限成功 | UserId: {UserId} | PermissionCount: {Count}", userId, permissions.Count);
        }

        return permissions;
    }

    /// <inheritdoc/>
#pragma warning disable CA1031
    public async Task<bool> SetUserPermissionsAsync(Guid userId, HashSet<string> permissions, TimeSpan? expiration = null, CancellationToken cancellationToken = default) {
        var cacheKey = CacheKeyConstants.User.Permissions(userId);
        var expireTime = expiration ?? CacheKeyConstants.Expiration.UserPermissions;

        try {
            await _cacheProvider.SetAsync(cacheKey, permissions, expireTime);
            _logger.LogInformation("设置用户权限缓存成功 | UserId: {UserId} | PermissionCount: {Count} | Expiration: {Expiration}",
                userId, permissions.Count, expireTime);
            return true;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "设置用户权限缓存失败 | UserId: {UserId}", userId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default) {
        var cacheKey = CacheKeyConstants.User.Permissions(userId);

        try {
            await _cacheProvider.RemoveAsync(cacheKey);
            _logger.LogInformation("移除用户权限缓存成功 | UserId: {UserId}", userId);
            return true;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "移除用户权限缓存失败 | UserId: {UserId}", userId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<int> RemoveUserPermissionsBatchAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(userIds);

        var userIdList = userIds.Distinct().ToList();
        var successCount = 0;

        foreach (var userId in userIdList) {
            if (await RemoveUserPermissionsAsync(userId, cancellationToken)) {
                successCount++;
            }
        }

        _logger.LogInformation("批量移除用户权限缓存完成 | TotalCount: {TotalCount} | SuccessCount: {SuccessCount}",
            userIdList.Count, successCount);

        return successCount;
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveAllUserPermissionsAsync(CancellationToken cancellationToken = default) {
        var pattern = CacheKeyConstants.Utils.UserModulePattern() + ":permissions:*";

        try {
            var count = await _cacheProvider.RemoveByPatternAsync(pattern);
            _logger.LogInformation("移除所有用户权限缓存成功 | Count: {Count}", count);
            return true;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "移除所有用户权限缓存失败");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<HashSet<string>?> GetRoleInheritedPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default) {
        var cacheKey = CacheKeyConstants.Role.InheritedPermissions(roleId);
        var permissions = await _cacheProvider.GetAsync<HashSet<string>>(cacheKey);

        if (permissions != null) {
            _logger.LogDebug("从缓存获取角色继承权限成功 | RoleId: {RoleId} | PermissionCount: {Count}", roleId, permissions.Count);
        }

        return permissions;
    }

    /// <inheritdoc/>
    public async Task<bool> SetRoleInheritedPermissionsAsync(Guid roleId, HashSet<string> permissions, TimeSpan? expiration = null, CancellationToken cancellationToken = default) {
        var cacheKey = CacheKeyConstants.Role.InheritedPermissions(roleId);
        var expireTime = expiration ?? CacheKeyConstants.Expiration.RoleInheritedPermissions;

        try {
            await _cacheProvider.SetAsync(cacheKey, permissions, expireTime);
            _logger.LogInformation("设置角色继承权限缓存成功 | RoleId: {RoleId} | PermissionCount: {Count} | Expiration: {Expiration}",
                roleId, permissions.Count, expireTime);
            return true;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "设置角色继承权限缓存失败 | RoleId: {RoleId}", roleId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveRoleInheritedPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default) {
        var cacheKey = CacheKeyConstants.Role.InheritedPermissions(roleId);

        try {
            await _cacheProvider.RemoveAsync(cacheKey);
            _logger.LogInformation("移除角色继承权限缓存成功 | RoleId: {RoleId}", roleId);
            return true;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "移除角色继承权限缓存失败 | RoleId: {RoleId}", roleId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<int> RemoveRoleInheritedPermissionsBatchAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(roleIds);

        var roleIdList = roleIds.Distinct().ToList();
        var successCount = 0;

        foreach (var roleId in roleIdList) {
            if (await RemoveRoleInheritedPermissionsAsync(roleId, cancellationToken)) {
                successCount++;
            }
        }

        _logger.LogInformation("批量移除角色继承权限缓存完成 | TotalCount: {TotalCount} | SuccessCount: {SuccessCount}",
            roleIdList.Count, successCount);

        return successCount;
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveAllRoleInheritedPermissionsAsync(CancellationToken cancellationToken = default) {
        var pattern = CacheKeyConstants.Utils.RoleModulePattern() + ":inherited_permissions:*";

        try {
            var count = await _cacheProvider.RemoveByPatternAsync(pattern);
            _logger.LogInformation("移除所有角色继承权限缓存成功 | Count: {Count}", count);
            return true;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "移除所有角色继承权限缓存失败");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ClearRoleRelatedCacheAsync(Guid roleId, CancellationToken cancellationToken = default) {
        try {
            await RemoveRoleInheritedPermissionsAsync(roleId, cancellationToken);

            var userIds = await _userRoleRepository.GetUserIdsByRoleIdAsync(roleId, cancellationToken);
            if (userIds.Count > 0) {
                await RemoveUserPermissionsBatchAsync(userIds, cancellationToken);
            }

            _logger.LogInformation("清除角色相关缓存成功 | RoleId: {RoleId} | AffectedUserCount: {UserCount}",
                roleId, userIds.Count);
            return true;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "清除角色相关缓存失败 | RoleId: {RoleId}", roleId);
            return false;
        }
    }

    /// <summary>
    /// 清除角色继承关系变更相关的所有缓存
    /// </summary>
    /// <param name="roleId">变更的角色ID</param>
    /// <param name="oldParentId">原父角色ID</param>
    /// <param name="newParentId">新父角色ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// <para>当角色的父角色变更时，需要清除以下缓存：</para>
    /// <list type="bullet">
    ///   <item>当前角色的继承权限缓存</item>
    ///   <item>原父角色链上所有角色的继承权限缓存（向上继承场景）</item>
    ///   <item>新父角色链上所有角色的继承权限缓存（向上继承场景）</item>
    ///   <item>所有子角色的继承权限缓存（向下继承场景）</item>
    ///   <item>所有受影响角色的用户权限缓存</item>
    /// </list>
    /// </remarks>
    public async Task<bool> ClearRoleInheritanceChangeCacheAsync(
        Guid roleId,
        Guid? oldParentId,
        Guid? newParentId,
        CancellationToken cancellationToken = default) {
        try {
            var affectedRoleIds = new HashSet<Guid> { roleId };

            if (oldParentId.HasValue) {
                var oldParentChain = await GetParentChainAsync(oldParentId.Value, cancellationToken);
                foreach (var parentId in oldParentChain) {
                    affectedRoleIds.Add(parentId);
                }
            }

            if (newParentId.HasValue) {
                var newParentChain = await GetParentChainAsync(newParentId.Value, cancellationToken);
                foreach (var parentId in newParentChain) {
                    affectedRoleIds.Add(parentId);
                }
            }

            var allChildren = await _roleRepository.GetAllChildrenAsync(roleId, cancellationToken);
            foreach (var child in allChildren) {
                affectedRoleIds.Add(child.Id);
            }

            var allUserIds = new HashSet<Guid>();
            foreach (var affectedRoleId in affectedRoleIds) {
                await RemoveRoleInheritedPermissionsAsync(affectedRoleId, cancellationToken);

                var userIds = await _userRoleRepository.GetUserIdsByRoleIdAsync(affectedRoleId, cancellationToken);
                foreach (var userId in userIds) {
                    allUserIds.Add(userId);
                }
            }

            if (allUserIds.Count > 0) {
                await RemoveUserPermissionsBatchAsync(allUserIds, cancellationToken);
            }

            _logger.LogInformation("清除角色继承关系变更缓存成功 | RoleId: {RoleId} | OldParentId: {OldParentId} | NewParentId: {NewParentId} | AffectedRoleCount: {RoleCount} | AffectedUserCount: {UserCount}",
                roleId, oldParentId, newParentId, affectedRoleIds.Count, allUserIds.Count);
            return true;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "清除角色继承关系变更缓存失败 | RoleId: {RoleId}", roleId);
            return false;
        }
    }

    /// <summary>
    /// 清除角色继承类型变更相关的所有缓存
    /// </summary>
    /// <param name="roleId">变更的角色ID</param>
    /// <param name="oldInheritanceType">原继承类型</param>
    /// <param name="newInheritanceType">新继承类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// <para>当角色的继承类型变更时，需要清除以下缓存：</para>
    /// <list type="bullet">
    ///   <item>当前角色的继承权限缓存</item>
    ///   <item>父角色链上所有角色的继承权限缓存（向上继承场景）</item>
    ///   <item>所有子角色的继承权限缓存（向下继承场景）</item>
    ///   <item>所有受影响角色的用户权限缓存</item>
    /// </list>
    /// </remarks>
    public async Task<bool> ClearRoleInheritanceTypeChangeCacheAsync(
        Guid roleId,
        InheritanceType oldInheritanceType,
        InheritanceType newInheritanceType,
        CancellationToken cancellationToken = default) {
        try {
            var affectedRoleIds = new HashSet<Guid> { roleId };

            if (oldInheritanceType == Domain.Shared.Enums.InheritanceType.Upward ||
                newInheritanceType == Domain.Shared.Enums.InheritanceType.Upward) {
                var inheritanceChain = await _roleRepository.GetInheritanceChainAsync(roleId, cancellationToken);
                foreach (var parentRole in inheritanceChain) {
                    if (parentRole.Id != roleId) {
                        affectedRoleIds.Add(parentRole.Id);
                    }
                }
            }

            if (oldInheritanceType == Domain.Shared.Enums.InheritanceType.Downward ||
                newInheritanceType == Domain.Shared.Enums.InheritanceType.Downward) {
                var allChildren = await _roleRepository.GetAllChildrenAsync(roleId, cancellationToken);
                foreach (var child in allChildren) {
                    affectedRoleIds.Add(child.Id);
                }
            }

            var allUserIds = new HashSet<Guid>();
            foreach (var affectedRoleId in affectedRoleIds) {
                await RemoveRoleInheritedPermissionsAsync(affectedRoleId, cancellationToken);

                var userIds = await _userRoleRepository.GetUserIdsByRoleIdAsync(affectedRoleId, cancellationToken);
                foreach (var userId in userIds) {
                    allUserIds.Add(userId);
                }
            }

            if (allUserIds.Count > 0) {
                await RemoveUserPermissionsBatchAsync(allUserIds, cancellationToken);
            }

            _logger.LogInformation("清除角色继承类型变更缓存成功 | RoleId: {RoleId} | OldType: {OldType} | NewType: {NewType} | AffectedRoleCount: {RoleCount} | AffectedUserCount: {UserCount}",
                roleId, oldInheritanceType, newInheritanceType, affectedRoleIds.Count, allUserIds.Count);
            return true;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "清除角色继承类型变更缓存失败 | RoleId: {RoleId}", roleId);
            return false;
        }
    }

    private async Task<List<Guid>> GetParentChainAsync(Guid roleId, CancellationToken cancellationToken) {
        var parentIds = new List<Guid>();
        var currentRole = await _roleRepository.GetAsync(roleId, cancellationToken);

        while (currentRole?.ParentId.HasValue == true) {
            parentIds.Add(currentRole.ParentId.Value);
            currentRole = await _roleRepository.GetAsync(currentRole.ParentId.Value, cancellationToken);
        }

        return parentIds;
    }

    /// <inheritdoc/>
    public async Task<bool> ClearRoleRelatedCacheBatchAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(roleIds);

        var roleIdList = roleIds.Distinct().ToList();
        var allUserIds = new HashSet<Guid>();

        try {
            await RemoveRoleInheritedPermissionsBatchAsync(roleIdList, cancellationToken);

            foreach (var roleId in roleIdList) {
                var userIds = await _userRoleRepository.GetUserIdsByRoleIdAsync(roleId, cancellationToken);
                foreach (var userId in userIds) {
                    allUserIds.Add(userId);
                }
            }

            if (allUserIds.Count > 0) {
                await RemoveUserPermissionsBatchAsync(allUserIds, cancellationToken);
            }

            _logger.LogInformation("批量清除角色相关缓存成功 | RoleCount: {RoleCount} | AffectedUserCount: {UserCount}",
                roleIdList.Count, allUserIds.Count);
            return true;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "批量清除角色相关缓存失败 | RoleCount: {Count}", roleIdList.Count);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ClearAllPermissionCacheAsync(CancellationToken cancellationToken = default) {
        try {
            await RemoveAllUserPermissionsAsync(cancellationToken);
            await RemoveAllRoleInheritedPermissionsAsync(cancellationToken);

            _logger.LogInformation("清除所有权限缓存成功");
            return true;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "清除所有权限缓存失败");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> WarmupUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default) {
        try {
            var permissions = await _permissionDomainService.GetUserPermissionCodesAsync(userId, cancellationToken);
            if (permissions.Count == 0) {
                _logger.LogDebug("用户无权限，跳过缓存预热 | UserId: {UserId}", userId);
                return true;
            }

            var permissionSet = new HashSet<string>(permissions);
            return await SetUserPermissionsAsync(userId, permissionSet, cancellationToken: cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "预热用户权限缓存失败 | UserId: {UserId}", userId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<int> WarmupUserPermissionsBatchAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(userIds);

        var userIdList = userIds.Distinct().ToList();
        var successCount = 0;

        foreach (var userId in userIdList) {
            if (await WarmupUserPermissionsAsync(userId, cancellationToken)) {
                successCount++;
            }
        }

        _logger.LogInformation("批量预热用户权限缓存完成 | TotalCount: {TotalCount} | SuccessCount: {SuccessCount}",
            userIdList.Count, successCount);

        return successCount;
    }

    /// <inheritdoc/>
    public async Task<bool> WarmupRoleInheritedPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default) {
        try {
            var role = await _roleRepository.GetAsync(roleId, cancellationToken);
            if (role == null) {
                _logger.LogWarning("角色不存在，跳过缓存预热 | RoleId: {RoleId}", roleId);
                return false;
            }

            if (role.InheritanceType == Domain.Shared.Enums.InheritanceType.None) {
                _logger.LogDebug("角色无继承关系，跳过缓存预热 | RoleId: {RoleId}", roleId);
                return true;
            }

            var inheritedPermissions = await _permissionDomainService.GetRoleInheritedPermissionsAsync(roleId, role.InheritanceType, cancellationToken);
            if (inheritedPermissions.Count == 0) {
                _logger.LogDebug("角色无继承权限，跳过缓存预热 | RoleId: {RoleId}", roleId);
                return true;
            }

            var permissionSet = new HashSet<string>(inheritedPermissions);
            return await SetRoleInheritedPermissionsAsync(roleId, permissionSet, cancellationToken: cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "预热角色继承权限缓存失败 | RoleId: {RoleId}", roleId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<int> WarmupRoleInheritedPermissionsBatchAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(roleIds);

        var roleIdList = roleIds.Distinct().ToList();
        var successCount = 0;

        foreach (var roleId in roleIdList) {
            if (await WarmupRoleInheritedPermissionsAsync(roleId, cancellationToken)) {
                successCount++;
            }
        }

        _logger.LogInformation("批量预热角色继承权限缓存完成 | TotalCount: {TotalCount} | SuccessCount: {SuccessCount}",
            roleIdList.Count, successCount);

        return successCount;
    }
#pragma warning restore CA1031
}
