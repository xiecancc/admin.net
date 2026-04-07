/*
 * 文件名称: UserRoleCommands.cs
 * 功能描述: 用户角色关联命令类，包含用户角色分配和移除操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using MediatR;

namespace Application.Contracts.Commands;

/// <summary>
/// 为用户分配角色命令
/// <para>将指定角色分配给用户</para>
/// </summary>
public class AssignRolesToUserCommand : IRequest<bool> {
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<Guid> RoleIds { get; set; } = [];
}

/// <summary>
/// 移除用户角色命令
/// <para>从用户移除指定角色</para>
/// </summary>
public class RemoveRolesFromUserCommand : IRequest<bool> {
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<Guid> RoleIds { get; set; } = [];
}

/// <summary>
/// 为角色分配用户命令
/// <para>将指定用户分配给角色</para>
/// </summary>
public class AssignUsersToRoleCommand : IRequest<bool> {
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 用户ID列表
    /// </summary>
    public List<Guid> UserIds { get; set; } = [];
}

/// <summary>
/// 移除角色用户命令
/// <para>从角色移除指定用户</para>
/// </summary>
public class RemoveUsersFromRoleCommand : IRequest<bool> {
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 用户ID列表
    /// </summary>
    public List<Guid> UserIds { get; set; } = [];
}
