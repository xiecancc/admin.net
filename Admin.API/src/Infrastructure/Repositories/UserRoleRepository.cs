/*
 * 文件名称: UserRoleRepository.cs
 * 功能描述: 用户角色关联仓储实现，用于处理用户和角色之间的关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-31
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Events;
using Infrastructure.Shared.Contexts;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Infrastructure.Repositories;

/// <inheritdoc cref="IUserRoleRepository"/>
/// <param name="client">SqlSugar 客户端</param>
/// <param name="eventBus">领域事件总线</param>
/// <param name="httpContextProvider">HTTP 上下文提供者</param>
/// <param name="logger">日志记录器</param>
public class UserRoleRepository(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IHttpContextProvider httpContextProvider,
    ILogger<UserRoleRepository> logger)
    : DomainRepository<UserRole>(client, eventBus, httpContextProvider, "UserRole", logger), IUserRoleRepository {
    /// <inheritdoc/>
    public async Task<List<Guid>> GetRoleIdsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) {
        return await _client.Queryable<UserRole>()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<Guid>> GetUserIdsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default) {
        return await _client.Queryable<UserRole>()
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.UserId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds, CancellationToken cancellationToken = default) {
        if (roleIds.Count == 0) {
            return true;
        }

        var userRoles = roleIds.Select(roleId => new UserRole {
            UserId = userId,
            RoleId = roleId
        }).ToList();

        return await InsertAsync(userRoles, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> AssignUsersToRoleAsync(Guid roleId, List<Guid> userIds, CancellationToken cancellationToken = default) {
        if (userIds.Count == 0) {
            return true;
        }

        var userRoles = userIds.Select(userId => new UserRole {
            UserId = userId,
            RoleId = roleId
        }).ToList();

        return await InsertAsync(userRoles, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveRolesFromUserAsync(Guid userId, List<Guid> roleIds, CancellationToken cancellationToken = default) {
        return roleIds.Count == 0 ? true : await DeleteAsync(ur => ur.UserId == userId && roleIds.Contains(ur.RoleId), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveUsersFromRoleAsync(Guid roleId, List<Guid> userIds, CancellationToken cancellationToken = default) {
        return userIds.Count == 0 ? true : await DeleteAsync(ur => ur.RoleId == roleId && userIds.Contains(ur.UserId), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default) {
        return await ExistsAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetUserRoleCodesAsync(Guid userId, CancellationToken cancellationToken = default) {
        return await _client.Queryable<UserRole>()
            .InnerJoin<Role>((ur, r) => ur.RoleId == r.Id)
            .Where((ur, r) => ur.UserId == userId)
            .Select((ur, r) => r.Code)
            .ToListAsync(cancellationToken);
    }
}
