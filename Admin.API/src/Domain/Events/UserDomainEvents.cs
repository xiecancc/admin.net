/*
 * 文件名称: UserDomainEvents.cs
 * 功能描述: 用户领域事件类，定义用户相关的领域事件
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

using Domain.Entities;
using Domain.Shared.Events;

namespace Domain.Events;

/// <summary>
/// 用户角色分配事件
/// <para>当为用户分配角色时触发</para>
/// </summary>
public class UserRolesAssignedEvent : DomainEvent {
    /// <summary>
    /// 用户 ID
    /// </summary>
    /// <value>被分配角色的用户 ID</value>
    public Guid UserId {
        get;
    }

    /// <summary>
    /// 角色 ID 集合
    /// </summary>
    /// <value>分配给用户的角色 ID 集合</value>
    public IEnumerable<Guid> RoleIds {
        get;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="roleIds">角色 ID 集合</param>
    public UserRolesAssignedEvent(Guid userId, IEnumerable<Guid> roleIds) {
        UserId = userId;
        RoleIds = roleIds;
        Description = $"为用户分配了 {roleIds.Count()} 个角色";
    }
}

/// <summary>
/// 用户角色撤销事件
/// <para>当从用户撤销角色时触发</para>
/// </summary>
public class UserRolesRevokedEvent : DomainEvent {
    /// <summary>
    /// 用户 ID
    /// </summary>
    /// <value>被撤销角色的用户 ID</value>
    public Guid UserId {
        get;
    }

    /// <summary>
    /// 角色 ID 集合
    /// </summary>
    /// <value>从用户撤销的角色 ID 集合</value>
    public IEnumerable<Guid> RoleIds {
        get;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="roleIds">角色 ID 集合</param>
    public UserRolesRevokedEvent(Guid userId, IEnumerable<Guid> roleIds) {
        UserId = userId;
        RoleIds = roleIds;
        Description = $"撤销了用户的 {roleIds.Count()} 个角色";
    }
}
