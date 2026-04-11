/*
 * 文件名称: UserRoleQueries.cs
 * 功能描述: 用户角色关联查询类，包含用户角色查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// 用户角色列表查询
/// <para>获取指定用户的所有角色</para>
/// </summary>
public class UserRolesQuery : Query<List<RoleListDto>> {
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userId">用户ID</param>
    public UserRolesQuery(Guid userId) {
        UserId = userId;
    }
}

/// <summary>
/// 角色用户列表查询
/// <para>获取指定角色的所有用户</para>
/// </summary>
public class RoleUsersQuery : Query<List<UserListDto>> {
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleId">角色ID</param>
    public RoleUsersQuery(Guid roleId) {
        RoleId = roleId;
    }
}

/// <summary>
/// 用户角色ID列表查询
/// <para>获取指定用户的角色ID列表</para>
/// </summary>
public class UserRoleIdsQuery : Query<List<Guid>> {
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userId">用户ID</param>
    public UserRoleIdsQuery(Guid userId) {
        UserId = userId;
    }
}

/// <summary>
/// 用户角色关联列表查询
/// <para>用于查询用户角色关联关系的列表数据</para>
/// </summary>
public class UserRoleListQuery : ListQuery<UserRoleListDto> {
    /// <summary>
    /// 用户 ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 角色 ID
    /// </summary>
    public Guid? RoleId { get; set; }
}

/// <summary>
/// 用户角色关联分页查询
/// <para>用于查询用户角色关联关系的分页数据</para>
/// </summary>
public class UserRolePagedQuery : PagedQuery<UserRolePagedDto> {
    /// <summary>
    /// 用户 ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 角色 ID
    /// </summary>
    public Guid? RoleId { get; set; }
}
