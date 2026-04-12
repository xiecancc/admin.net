/*
 * 文件名称: AuditTargetType.cs
 * 功能描述: 审计目标类型枚举，定义权限审计日志的目标类型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

namespace Domain.Shared.Enums;

/// <summary>
/// 审计目标类型枚举
/// <para>定义权限审计日志的目标类型</para>
/// </summary>
/// <remarks>
/// <para>目标类型说明：</para>
/// <list type="bullet">
///   <item>UserRole：用户角色关系，记录用户与角色的关联变更</item>
///   <item>RolePermission：角色权限关系，记录角色与权限的关联变更</item>
///   <item>UserPermission：用户权限关系，记录用户与权限的直接关联变更</item>
/// </list>
/// </remarks>
public enum AuditTargetType {
    /// <summary>
    /// 用户角色关系
    /// <para>记录用户与角色之间的关联变更</para>
    /// </summary>
    UserRole = 0,

    /// <summary>
    /// 角色权限关系
    /// <para>记录角色与权限之间的关联变更</para>
    /// </summary>
    RolePermission = 1,

    /// <summary>
    /// 用户权限关系
    /// <para>记录用户与权限之间的直接关联变更</para>
    /// </summary>
    UserPermission = 2
}
