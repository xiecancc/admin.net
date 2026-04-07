/*
 * 文件名称: IPermissionAuditLogRepository.cs
 * 功能描述: 权限审计日志仓储接口
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-07
 */

using Domain.Entities;
using Domain.Shared.Repositories;

namespace Domain.Repositories;

/// <summary>
/// 权限审计日志仓储接口
/// </summary>
public interface IPermissionAuditLogRepository : IDomainRepository<PermissionAuditLog> {
    /// <summary>
    /// 根据用户ID查询审计日志
    /// </summary>
    Task<List<PermissionAuditLog>> GetByUserIdAsync(Guid userId, DateTime? startTime = null, DateTime? endTime = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据操作人ID查询审计日志
    /// </summary>
    Task<List<PermissionAuditLog>> GetByOperatorIdAsync(Guid operatorId, DateTime? startTime = null, DateTime? endTime = null, CancellationToken cancellationToken = default);
}
