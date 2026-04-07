/*
 * 文件名称: RoleDomainEvents.cs
 * 功能描述: 角色领域事件类，定义角色相关的领域事件
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using Domain.Shared.Events;

namespace Domain.Events;

/// <summary>
/// 角色权限分配事件
/// <para>当为角色分配权限时触发</para>
/// </summary>
public class RolePermissionsAssignedEvent : DomainEvent {
    /// <summary>
    /// 角色 ID
    /// </summary>
    /// <value>被分配权限的角色 ID</value>
    public Guid RoleId {
        get;
    }

    /// <summary>
    /// 权限 ID 集合
    /// </summary>
    /// <value>分配给角色的权限 ID 集合</value>
    public IEnumerable<Guid> PermissionIds {
        get;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="permissionIds">权限 ID 集合</param>
    public RolePermissionsAssignedEvent(Guid roleId, IEnumerable<Guid> permissionIds) {
        RoleId = roleId;
        PermissionIds = permissionIds;
        Description = $"为角色分配了 {permissionIds.Count()} 个权限";
    }
}

/// <summary>
/// 角色权限撤销事件
/// <para>当从角色撤销权限时触发</para>
/// </summary>
public class RolePermissionsRevokedEvent : DomainEvent {
    /// <summary>
    /// 角色 ID
    /// </summary>
    /// <value>被撤销权限的角色 ID</value>
    public Guid RoleId {
        get;
    }

    /// <summary>
    /// 权限 ID 集合
    /// </summary>
    /// <value>从角色撤销的权限 ID 集合</value>
    public IEnumerable<Guid> PermissionIds {
        get;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="permissionIds">权限 ID 集合</param>
    public RolePermissionsRevokedEvent(Guid roleId, IEnumerable<Guid> permissionIds) {
        RoleId = roleId;
        PermissionIds = permissionIds;
        Description = $"撤销了角色的 {permissionIds.Count()} 个权限";
    }
}
