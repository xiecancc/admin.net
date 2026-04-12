/*
 * 文件名称: AuditOperationType.cs
 * 功能描述: 审计操作类型枚举，定义权限审计日志的操作类型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

namespace Domain.Shared.Enums;

/// <summary>
/// 审计操作类型枚举
/// <para>定义权限审计日志的操作类型</para>
/// </summary>
/// <remarks>
/// <para>操作类型说明：</para>
/// <list type="bullet">
///   <item>Assign：分配操作，如分配角色、分配权限</item>
///   <item>Revoke：撤销操作，如撤销角色、撤销权限</item>
///   <item>Update：修改操作，如修改权限配置</item>
/// </list>
/// </remarks>
public enum AuditOperationType {
    /// <summary>
    /// 分配操作
    /// <para>用于记录权限或角色的分配行为</para>
    /// </summary>
    Assign = 0,

    /// <summary>
    /// 撤销操作
    /// <para>用于记录权限或角色的撤销行为</para>
    /// </summary>
    Revoke = 1,

    /// <summary>
    /// 修改操作
    /// <para>用于记录权限或角色配置的修改行为</para>
    /// </summary>
    Update = 2
}
