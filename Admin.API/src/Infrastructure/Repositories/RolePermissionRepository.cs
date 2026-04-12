/*
 * 文件名称: RolePermissionRepository.cs
 * 功能描述: 角色权限关联仓储实现，用于处理角色和权限之间的关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Events;
using Infrastructure.Shared.Contexts;
using Domain.Shared.Utils;
using Infrastructure.Shared.Utils;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Infrastructure.Repositories;

/// <inheritdoc cref="IRolePermissionRepository"/>
/// <remarks>
/// <para>角色权限关联仓储负责：</para>
/// <list type="bullet">
///   <item> 管理角色与权限的关联关系 </item>
///   <item> 支持批量分配权限给角色 </item>
///   <item> 支持批量分配角色给权限 </item>
///   <item> 支持移除关联关系 </item>
/// </list>
/// <para>异常处理策略：</para>
/// <list type="bullet">
///   <item> 参数验证异常：ArgumentException（ID为空）、ArgumentNullException（列表为null）</item>
///   <item> 数据库异常：通过 RepositoryExceptionHelper 转换为业务异常 </item>
/// </list>
/// </remarks>
/// <param name="client">SqlSugar 客户端，用于数据库操作</param>
/// <param name="eventBus">领域事件总线，用于发布领域事件</param>
/// <param name="userContextProvider">用户上下文提供者，用于获取当前用户信息</param>
/// <param name="logger">日志记录器，用于记录操作日志</param>
public class RolePermissionRepository(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IUserContextProvider userContextProvider,
    ILogger<RolePermissionRepository> logger)
    : DomainRepository<RolePermission>(client, eventBus, userContextProvider, nameof(RolePermission), logger), IRolePermissionRepository {
    /// <summary>
    /// 根据角色ID获取权限ID列表
    /// </summary>
    /// <param name="roleId">角色ID，不能为 Guid.Empty</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限ID列表</returns>
    /// <exception cref="ArgumentException">当 roleId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<List<Guid>> GetPermissionIdsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default) {
        if (roleId == Guid.Empty) {
            throw new ArgumentException("角色ID不能为空", nameof(roleId));
        }

        try {
            _logger.LogDebug("开始获取角色的权限ID列表: 角色ID: {RoleId}", roleId);
            var result = await _client.Queryable<RolePermission>()
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync(cancellationToken);
            _logger.LogDebug("获取角色的权限ID列表成功: 角色ID: {RoleId}, 数量: {Count}", roleId, result.Count);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "获取角色的权限ID列表失败: 角色ID: {RoleId}", roleId);
            throw RepositoryExceptionHelper.HandleException(ex, "获取角色权限", _entityName);
        }
    }

    /// <summary>
    /// 根据权限ID获取角色ID列表
    /// </summary>
    /// <param name="permissionId">权限ID，不能为 Guid.Empty</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色ID列表</returns>
    /// <exception cref="ArgumentException">当 permissionId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<List<Guid>> GetRoleIdsByPermissionIdAsync(Guid permissionId, CancellationToken cancellationToken = default) {
        if (permissionId == Guid.Empty) {
            throw new ArgumentException("权限ID不能为空", nameof(permissionId));
        }

        try {
            _logger.LogDebug("开始获取权限的角色ID列表: 权限ID: {PermissionId}", permissionId);
            var result = await _client.Queryable<RolePermission>()
                .Where(rp => rp.PermissionId == permissionId)
                .Select(rp => rp.RoleId)
                .ToListAsync(cancellationToken);
            _logger.LogDebug("获取权限的角色ID列表成功: 权限ID: {PermissionId}, 数量: {Count}", permissionId, result.Count);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "获取权限的角色ID列表失败: 权限ID: {PermissionId}", permissionId);
            throw RepositoryExceptionHelper.HandleException(ex, "获取权限角色", _entityName);
        }
    }

    /// <summary>
    /// 为角色分配权限
    /// </summary>
    /// <param name="roleId">角色ID，不能为 Guid.Empty</param>
    /// <param name="permissionIds">权限ID列表，不能为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分配成功返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 roleId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="ArgumentNullException">当 permissionIds 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken = default) {
        if (roleId == Guid.Empty) {
            throw new ArgumentException("角色ID不能为空", nameof(roleId));
        }

        ArgumentNullException.ThrowIfNull(permissionIds, nameof(permissionIds));

        if (permissionIds.Count == 0) {
            _logger.LogDebug("权限ID列表为空，跳过分配操作: 角色ID: {RoleId}", roleId);
            return true;
        }

        try {
            _logger.LogDebug("开始为角色分配权限: 角色ID: {RoleId}, 权限数量: {Count}", roleId, permissionIds.Count);

            var validPermissionIds = permissionIds.Where(id => id != Guid.Empty).ToList();
            if (validPermissionIds.Count == 0) {
                _logger.LogWarning("权限ID列表中无有效ID: 角色ID: {RoleId}", roleId);
                return true;
            }

            var rolePermissions = validPermissionIds.Select(permissionId => new RolePermission {
                RoleId = roleId,
                PermissionId = permissionId
            }).ToList();

            var result = await InsertAsync(rolePermissions, cancellationToken);
            _logger.LogDebug("为角色分配权限完成: 角色ID: {RoleId}, 结果: {Result}", roleId, result);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "为角色分配权限失败: 角色ID: {RoleId}", roleId);
            throw RepositoryExceptionHelper.HandleException(ex, "分配权限", _entityName);
        }
    }

    /// <summary>
    /// 为权限分配角色
    /// </summary>
    /// <param name="permissionId">权限ID，不能为 Guid.Empty</param>
    /// <param name="roleIds">角色ID列表，不能为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分配成功返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 permissionId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="ArgumentNullException">当 roleIds 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> AssignRolesToPermissionAsync(Guid permissionId, List<Guid> roleIds, CancellationToken cancellationToken = default) {
        if (permissionId == Guid.Empty) {
            throw new ArgumentException("权限ID不能为空", nameof(permissionId));
        }

        ArgumentNullException.ThrowIfNull(roleIds, nameof(roleIds));

        if (roleIds.Count == 0) {
            _logger.LogDebug("角色ID列表为空，跳过分配操作: 权限ID: {PermissionId}", permissionId);
            return true;
        }

        try {
            _logger.LogDebug("开始为权限分配角色: 权限ID: {PermissionId}, 角色数量: {Count}", permissionId, roleIds.Count);

            var validRoleIds = roleIds.Where(id => id != Guid.Empty).ToList();
            if (validRoleIds.Count == 0) {
                _logger.LogWarning("角色ID列表中无有效ID: 权限ID: {PermissionId}", permissionId);
                return true;
            }

            var rolePermissions = validRoleIds.Select(roleId => new RolePermission {
                RoleId = roleId,
                PermissionId = permissionId
            }).ToList();

            var result = await InsertAsync(rolePermissions, cancellationToken);
            _logger.LogDebug("为权限分配角色完成: 权限ID: {PermissionId}, 结果: {Result}", permissionId, result);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "为权限分配角色失败: 权限ID: {PermissionId}", permissionId);
            throw RepositoryExceptionHelper.HandleException(ex, "分配角色", _entityName);
        }
    }

    /// <summary>
    /// 移除角色的权限
    /// </summary>
    /// <param name="roleId">角色ID，不能为 Guid.Empty</param>
    /// <param name="permissionIds">权限ID列表，不能为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>移除成功返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 roleId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="ArgumentNullException">当 permissionIds 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> RemovePermissionsFromRoleAsync(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken = default) {
        if (roleId == Guid.Empty) {
            throw new ArgumentException("角色ID不能为空", nameof(roleId));
        }

        ArgumentNullException.ThrowIfNull(permissionIds, nameof(permissionIds));

        if (permissionIds.Count == 0) {
            _logger.LogDebug("权限ID列表为空，跳过移除操作: 角色ID: {RoleId}", roleId);
            return true;
        }

        try {
            _logger.LogDebug("开始移除角色的权限: 角色ID: {RoleId}, 权限数量: {Count}", roleId, permissionIds.Count);

            var validPermissionIds = permissionIds.Where(id => id != Guid.Empty).ToList();
            if (validPermissionIds.Count == 0) {
                _logger.LogWarning("权限ID列表中无有效ID: 角色ID: {RoleId}", roleId);
                return true;
            }

            var result = await DeleteAsync(rp => rp.RoleId == roleId && validPermissionIds.Contains(rp.PermissionId), cancellationToken);
            _logger.LogDebug("移除角色的权限完成: 角色ID: {RoleId}, 结果: {Result}", roleId, result);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "移除角色的权限失败: 角色ID: {RoleId}", roleId);
            throw RepositoryExceptionHelper.HandleException(ex, "移除权限", _entityName);
        }
    }

    /// <summary>
    /// 移除权限的角色
    /// </summary>
    /// <param name="permissionId">权限ID，不能为 Guid.Empty</param>
    /// <param name="roleIds">角色ID列表，不能为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>移除成功返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 permissionId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="ArgumentNullException">当 roleIds 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> RemoveRolesFromPermissionAsync(Guid permissionId, List<Guid> roleIds, CancellationToken cancellationToken = default) {
        if (permissionId == Guid.Empty) {
            throw new ArgumentException("权限ID不能为空", nameof(permissionId));
        }

        ArgumentNullException.ThrowIfNull(roleIds, nameof(roleIds));

        if (roleIds.Count == 0) {
            _logger.LogDebug("角色ID列表为空，跳过移除操作: 权限ID: {PermissionId}", permissionId);
            return true;
        }

        try {
            _logger.LogDebug("开始移除权限的角色: 权限ID: {PermissionId}, 角色数量: {Count}", permissionId, roleIds.Count);

            var validRoleIds = roleIds.Where(id => id != Guid.Empty).ToList();
            if (validRoleIds.Count == 0) {
                _logger.LogWarning("角色ID列表中无有效ID: 权限ID: {PermissionId}", permissionId);
                return true;
            }

            var result = await DeleteAsync(rp => rp.PermissionId == permissionId && validRoleIds.Contains(rp.RoleId), cancellationToken);
            _logger.LogDebug("移除权限的角色完成: 权限ID: {PermissionId}, 结果: {Result}", permissionId, result);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "移除权限的角色失败: 权限ID: {PermissionId}", permissionId);
            throw RepositoryExceptionHelper.HandleException(ex, "移除角色", _entityName);
        }
    }

    /// <summary>
    /// 检查角色是否拥有指定权限
    /// </summary>
    /// <param name="roleId">角色ID，不能为 Guid.Empty</param>
    /// <param name="permissionId">权限ID，不能为 Guid.Empty</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>拥有权限返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 roleId 或 permissionId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> RoleHasPermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default) {
        if (roleId == Guid.Empty) {
            throw new ArgumentException("角色ID不能为空", nameof(roleId));
        }

        if (permissionId == Guid.Empty) {
            throw new ArgumentException("权限ID不能为空", nameof(permissionId));
        }

        try {
            _logger.LogDebug("开始检查角色权限: 角色ID: {RoleId}, 权限ID: {PermissionId}", roleId, permissionId);
            var result = await ExistsAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken);
            _logger.LogDebug("检查角色权限完成: 角色ID: {RoleId}, 权限ID: {PermissionId}, 结果: {Result}", roleId, permissionId, result);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "检查角色权限失败: 角色ID: {RoleId}, 权限ID: {PermissionId}", roleId, permissionId);
            throw RepositoryExceptionHelper.HandleException(ex, "检查角色权限", _entityName);
        }
    }
}
