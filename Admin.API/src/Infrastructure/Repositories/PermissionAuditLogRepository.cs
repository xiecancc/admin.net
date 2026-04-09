/*
 * 文件名称: PermissionAuditLogRepository.cs
 * 功能描述: 权限审计日志仓储实现
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-07
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Events;
using Infrastructure.Shared.Contexts;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Infrastructure.Repositories;

/// <inheritdoc cref="IPermissionAuditLogRepository"/>
public class PermissionAuditLogRepository(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IUserContextProvider userContextProvider,
    ILogger<PermissionAuditLogRepository> logger)
    : DomainRepository<PermissionAuditLog>(client, eventBus, userContextProvider, "权限审计日志", logger as ILogger<DomainRepository<PermissionAuditLog>>), IPermissionAuditLogRepository {
    
    /// <inheritdoc/>
    public async Task<List<PermissionAuditLog>> GetByUserIdAsync(Guid userId, DateTime? startTime = null, DateTime? endTime = null, CancellationToken cancellationToken = default) {
        var query = _client.Queryable<PermissionAuditLog>()
            .Where(log => log.UserId == userId);

        if (startTime.HasValue) {
            query = query.Where(log => log.CreatedAt >= startTime.Value);
        }

        if (endTime.HasValue) {
            query = query.Where(log => log.CreatedAt <= endTime.Value);
        }

        return await query.OrderByDescending(log => log.CreatedAt).ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<PermissionAuditLog>> GetByOperatorIdAsync(Guid operatorId, DateTime? startTime = null, DateTime? endTime = null, CancellationToken cancellationToken = default) {
        var query = _client.Queryable<PermissionAuditLog>()
            .Where(log => log.OperatorId == operatorId);

        if (startTime.HasValue) {
            query = query.Where(log => log.CreatedAt >= startTime.Value);
        }

        if (endTime.HasValue) {
            query = query.Where(log => log.CreatedAt <= endTime.Value);
        }

        return await query.OrderByDescending(log => log.CreatedAt).ToListAsync(cancellationToken);
    }
}
