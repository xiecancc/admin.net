/*
 * 文件名称: RolePermissionCommands.cs
 * 功能描述: 角色权限关联命令类，包含角色权限分配和移除操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Application.Contracts.Abstractions.Commands;

namespace Application.Contracts.Commands;

/// <summary>
/// 为角色分配权限命令
/// <para>将指定权限分配给角色</para>
/// </summary>
public class AssignPermissionsToRoleCommand : Command<bool> {
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 权限ID列表
    /// </summary>
    public List<Guid> PermissionIds { get; set; } = [];
}

/// <summary>
/// 移除角色权限命令
/// <para>从角色移除指定权限</para>
/// </summary>
public class RemovePermissionsFromRoleCommand : Command<bool> {
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 权限ID列表
    /// </summary>
    public List<Guid> PermissionIds { get; set; } = [];
}
