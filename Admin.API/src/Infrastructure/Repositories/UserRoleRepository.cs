/*
 * 文件名称: UserRoleRepository.cs
 * 功能描述: 用户角色关联仓储实现，用于处理用户和角色之间的关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Events;
using Infrastructure.Shared.Contexts;
using Infrastructure.Shared.Utils;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Infrastructure.Repositories;

/// <inheritdoc cref="IUserRoleRepository"/>
/// <remarks>
/// <para>用户角色关联仓储负责：</para>
/// <list type="bullet">
///   <item> 管理用户与角色的关联关系 </item>
///   <item> 支持批量分配角色给用户 </item>
///   <item> 支持批量分配用户给角色 </item>
///   <item> 支持移除关联关系 </item>
///   <item> 支持获取用户角色编码列表 </item>
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
public class UserRoleRepository(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IUserContextProvider userContextProvider,
    ILogger<UserRoleRepository> logger)
    : DomainRepository<UserRole>(client, eventBus, userContextProvider, nameof(UserRole), logger), IUserRoleRepository {
    /// <summary>
    /// 根据用户ID获取角色ID列表
    /// </summary>
    /// <param name="userId">用户ID，不能为 Guid.Empty</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色ID列表</returns>
    /// <exception cref="ArgumentException">当 userId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<List<Guid>> GetRoleIdsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) {
        if (userId == Guid.Empty) {
            throw new ArgumentException("用户ID不能为空", nameof(userId));
        }

        try {
            _logger.LogDebug("开始获取用户的角色ID列表: 用户ID: {UserId}", userId);
            var result = await _client.Queryable<UserRole>()
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);
            _logger.LogDebug("获取用户的角色ID列表成功: 用户ID: {UserId}, 数量: {Count}", userId, result.Count);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "获取用户的角色ID列表失败: 用户ID: {UserId}", userId);
            throw RepositoryExceptionHelper.HandleException(ex, "获取用户角色", _entityName);
        }
    }

    /// <summary>
    /// 根据角色ID获取用户ID列表
    /// </summary>
    /// <param name="roleId">角色ID，不能为 Guid.Empty</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户ID列表</returns>
    /// <exception cref="ArgumentException">当 roleId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<List<Guid>> GetUserIdsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default) {
        if (roleId == Guid.Empty) {
            throw new ArgumentException("角色ID不能为空", nameof(roleId));
        }

        try {
            _logger.LogDebug("开始获取角色的用户ID列表: 角色ID: {RoleId}", roleId);
            var result = await _client.Queryable<UserRole>()
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.UserId)
                .ToListAsync(cancellationToken);
            _logger.LogDebug("获取角色的用户ID列表成功: 角色ID: {RoleId}, 数量: {Count}", roleId, result.Count);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "获取角色的用户ID列表失败: 角色ID: {RoleId}", roleId);
            throw RepositoryExceptionHelper.HandleException(ex, "获取角色用户", _entityName);
        }
    }

    /// <summary>
    /// 为用户分配角色
    /// </summary>
    /// <param name="userId">用户ID，不能为 Guid.Empty</param>
    /// <param name="roleIds">角色ID列表，不能为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分配成功返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 userId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="ArgumentNullException">当 roleIds 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds, CancellationToken cancellationToken = default) {
        if (userId == Guid.Empty) {
            throw new ArgumentException("用户ID不能为空", nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(roleIds, nameof(roleIds));

        if (roleIds.Count == 0) {
            _logger.LogDebug("角色ID列表为空，跳过分配操作: 用户ID: {UserId}", userId);
            return true;
        }

        try {
            _logger.LogDebug("开始为用户分配角色: 用户ID: {UserId}, 角色数量: {Count}", userId, roleIds.Count);

            var validRoleIds = roleIds.Where(id => id != Guid.Empty).ToList();
            if (validRoleIds.Count == 0) {
                _logger.LogWarning("角色ID列表中无有效ID: 用户ID: {UserId}", userId);
                return true;
            }

            var userRoles = validRoleIds.Select(roleId => new UserRole {
                UserId = userId,
                RoleId = roleId
            }).ToList();

            var result = await InsertAsync(userRoles, cancellationToken);
            _logger.LogDebug("为用户分配角色完成: 用户ID: {UserId}, 结果: {Result}", userId, result);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "为用户分配角色失败: 用户ID: {UserId}", userId);
            throw RepositoryExceptionHelper.HandleException(ex, "分配角色", _entityName);
        }
    }

    /// <summary>
    /// 为角色分配用户
    /// </summary>
    /// <param name="roleId">角色ID，不能为 Guid.Empty</param>
    /// <param name="userIds">用户ID列表，不能为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分配成功返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 roleId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="ArgumentNullException">当 userIds 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> AssignUsersToRoleAsync(Guid roleId, List<Guid> userIds, CancellationToken cancellationToken = default) {
        if (roleId == Guid.Empty) {
            throw new ArgumentException("角色ID不能为空", nameof(roleId));
        }

        ArgumentNullException.ThrowIfNull(userIds, nameof(userIds));

        if (userIds.Count == 0) {
            _logger.LogDebug("用户ID列表为空，跳过分配操作: 角色ID: {RoleId}", roleId);
            return true;
        }

        try {
            _logger.LogDebug("开始为角色分配用户: 角色ID: {RoleId}, 用户数量: {Count}", roleId, userIds.Count);

            var validUserIds = userIds.Where(id => id != Guid.Empty).ToList();
            if (validUserIds.Count == 0) {
                _logger.LogWarning("用户ID列表中无有效ID: 角色ID: {RoleId}", roleId);
                return true;
            }

            var userRoles = validUserIds.Select(userId => new UserRole {
                UserId = userId,
                RoleId = roleId
            }).ToList();

            var result = await InsertAsync(userRoles, cancellationToken);
            _logger.LogDebug("为角色分配用户完成: 角色ID: {RoleId}, 结果: {Result}", roleId, result);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "为角色分配用户失败: 角色ID: {RoleId}", roleId);
            throw RepositoryExceptionHelper.HandleException(ex, "分配用户", _entityName);
        }
    }

    /// <summary>
    /// 移除用户的角色
    /// </summary>
    /// <param name="userId">用户ID，不能为 Guid.Empty</param>
    /// <param name="roleIds">角色ID列表，不能为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>移除成功返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 userId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="ArgumentNullException">当 roleIds 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> RemoveRolesFromUserAsync(Guid userId, List<Guid> roleIds, CancellationToken cancellationToken = default) {
        if (userId == Guid.Empty) {
            throw new ArgumentException("用户ID不能为空", nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(roleIds, nameof(roleIds));

        if (roleIds.Count == 0) {
            _logger.LogDebug("角色ID列表为空，跳过移除操作: 用户ID: {UserId}", userId);
            return true;
        }

        try {
            _logger.LogDebug("开始移除用户的角色: 用户ID: {UserId}, 角色数量: {Count}", userId, roleIds.Count);

            var validRoleIds = roleIds.Where(id => id != Guid.Empty).ToList();
            if (validRoleIds.Count == 0) {
                _logger.LogWarning("角色ID列表中无有效ID: 用户ID: {UserId}", userId);
                return true;
            }

            var result = await DeleteAsync(ur => ur.UserId == userId && validRoleIds.Contains(ur.RoleId), cancellationToken);
            _logger.LogDebug("移除用户的角色完成: 用户ID: {UserId}, 结果: {Result}", userId, result);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "移除用户的角色失败: 用户ID: {UserId}", userId);
            throw RepositoryExceptionHelper.HandleException(ex, "移除角色", _entityName);
        }
    }

    /// <summary>
    /// 移除角色的用户
    /// </summary>
    /// <param name="roleId">角色ID，不能为 Guid.Empty</param>
    /// <param name="userIds">用户ID列表，不能为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>移除成功返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 roleId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="ArgumentNullException">当 userIds 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> RemoveUsersFromRoleAsync(Guid roleId, List<Guid> userIds, CancellationToken cancellationToken = default) {
        if (roleId == Guid.Empty) {
            throw new ArgumentException("角色ID不能为空", nameof(roleId));
        }

        ArgumentNullException.ThrowIfNull(userIds, nameof(userIds));

        if (userIds.Count == 0) {
            _logger.LogDebug("用户ID列表为空，跳过移除操作: 角色ID: {RoleId}", roleId);
            return true;
        }

        try {
            _logger.LogDebug("开始移除角色的用户: 角色ID: {RoleId}, 用户数量: {Count}", roleId, userIds.Count);

            var validUserIds = userIds.Where(id => id != Guid.Empty).ToList();
            if (validUserIds.Count == 0) {
                _logger.LogWarning("用户ID列表中无有效ID: 角色ID: {RoleId}", roleId);
                return true;
            }

            var result = await DeleteAsync(ur => ur.RoleId == roleId && validUserIds.Contains(ur.UserId), cancellationToken);
            _logger.LogDebug("移除角色的用户完成: 角色ID: {RoleId}, 结果: {Result}", roleId, result);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "移除角色的用户失败: 角色ID: {RoleId}", roleId);
            throw RepositoryExceptionHelper.HandleException(ex, "移除用户", _entityName);
        }
    }

    /// <summary>
    /// 检查用户是否拥有指定角色
    /// </summary>
    /// <param name="userId">用户ID，不能为 Guid.Empty</param>
    /// <param name="roleId">角色ID，不能为 Guid.Empty</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>拥有角色返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 userId 或 roleId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default) {
        if (userId == Guid.Empty) {
            throw new ArgumentException("用户ID不能为空", nameof(userId));
        }

        if (roleId == Guid.Empty) {
            throw new ArgumentException("角色ID不能为空", nameof(roleId));
        }

        try {
            _logger.LogDebug("开始检查用户角色: 用户ID: {UserId}, 角色ID: {RoleId}", userId, roleId);
            var result = await ExistsAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
            _logger.LogDebug("检查用户角色完成: 用户ID: {UserId}, 角色ID: {RoleId}, 结果: {Result}", userId, roleId, result);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "检查用户角色失败: 用户ID: {UserId}, 角色ID: {RoleId}", userId, roleId);
            throw RepositoryExceptionHelper.HandleException(ex, "检查用户角色", _entityName);
        }
    }

    /// <summary>
    /// 获取用户的角色编码列表
    /// </summary>
    /// <param name="userId">用户ID，不能为 Guid.Empty</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色编码列表</returns>
    /// <exception cref="ArgumentException">当 userId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<List<string>> GetUserRoleCodesAsync(Guid userId, CancellationToken cancellationToken = default) {
        if (userId == Guid.Empty) {
            throw new ArgumentException("用户ID不能为空", nameof(userId));
        }

        try {
            _logger.LogDebug("开始获取用户角色编码列表: 用户ID: {UserId}", userId);
            var result = await _client.Queryable<UserRole>()
                .InnerJoin<Role>((ur, r) => ur.RoleId == r.Id)
                .Where((ur, r) => ur.UserId == userId)
                .Select((ur, r) => r.Code)
                .ToListAsync(cancellationToken);
            _logger.LogDebug("获取用户角色编码列表成功: 用户ID: {UserId}, 数量: {Count}", userId, result.Count);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "获取用户角色编码列表失败: 用户ID: {UserId}", userId);
            throw RepositoryExceptionHelper.HandleException(ex, "获取角色编码", _entityName);
        }
    }
}
