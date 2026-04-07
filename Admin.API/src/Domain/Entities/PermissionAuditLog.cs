/*
 * 文件名称: PermissionAuditLog.cs
 * 功能描述: 权限审计日志实体,记录权限变更历史
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-07
 */

using SqlSugar;
using Domain.Shared.Entities;

namespace Domain.Entities;

/// <summary>
/// 权限审计日志实体
/// <para>记录权限分配、回收、修改的历史</para>
/// </summary>
[SugarTable("PermissionAuditLogs", "权限审计日志表")]
[SugarIndex("IX_PermissionAuditLogs_UserId", nameof(UserId), OrderByType.Asc)]
[SugarIndex("IX_PermissionAuditLogs_OperatorId", nameof(OperatorId), OrderByType.Asc)]
[SugarIndex("IX_PermissionAuditLogs_CreatedAt", nameof(CreatedAt), OrderByType.Asc)]
public class PermissionAuditLog : DomainBase {
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "主键ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户ID", IsNullable = false)]
    public Guid UserId { get; set; }

    /// <summary>
    /// 操作人ID
    /// </summary>
    [SugarColumn(ColumnDescription = "操作人ID", IsNullable = false)]
    public Guid OperatorId { get; set; }

    /// <summary>
    /// 操作类型 (Assign, Revoke, Update)
    /// </summary>
    [SugarColumn(ColumnDescription = "操作类型", Length = 20, IsNullable = false)]
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// 目标类型 (UserRole, RolePermission, Permission)
    /// </summary>
    [SugarColumn(ColumnDescription = "目标类型", Length = 50, IsNullable = false)]
    public string TargetType { get; set; } = string.Empty;

    /// <summary>
    /// 目标ID
    /// </summary>
    [SugarColumn(ColumnDescription = "目标ID", IsNullable = false)]
    public Guid TargetId { get; set; }

    /// <summary>
    /// 旧值
    /// </summary>
    [SugarColumn(ColumnDescription = "旧值", Length = 2000, IsNullable = true)]
    public string? OldValue { get; set; }

    /// <summary>
    /// 新值
    /// </summary>
    [SugarColumn(ColumnDescription = "新值", Length = 2000, IsNullable = true)]
    public string? NewValue { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(ColumnDescription = "描述", Length = 500, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [SugarColumn(ColumnDescription = "IP地址", Length = 50, IsNullable = true)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// 用户代理
    /// </summary>
    [SugarColumn(ColumnDescription = "用户代理", Length = 500, IsNullable = true)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
