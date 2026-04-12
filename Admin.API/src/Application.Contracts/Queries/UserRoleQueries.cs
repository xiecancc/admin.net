/*
 * 文件名称: UserRoleQueries.cs
 * 功能描述: 用户角色关联查询类，包含用户角色查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// 用户角色列表查询
/// <para>获取指定用户的所有角色</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class UserRolesQuery(UserRoleQueryDto QueryDto) : Query<UserRoleQueryDto, List<RoleListDto>>(QueryDto)
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }
}

/// <summary>
/// 角色用户列表查询
/// <para>获取指定角色的所有用户</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class RoleUsersQuery(UserRoleQueryDto QueryDto) : Query<UserRoleQueryDto, List<UserListDto>>(QueryDto)
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }
}

/// <summary>
/// 用户角色ID列表查询
/// <para>获取指定用户的角色ID列表</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class UserRoleIdsQuery(UserRoleQueryDto QueryDto) : Query<UserRoleQueryDto, List<Guid>>(QueryDto)
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }
}

/// <summary>
/// 用户角色关联列表查询
/// <para>用于查询用户角色关联关系的列表数据</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class UserRoleListQuery(UserRoleQueryDto QueryDto) : ListQuery<UserRoleQueryDto, UserRoleListDto>(QueryDto);

/// <summary>
/// 用户角色关联分页查询
/// <para>用于查询用户角色关联关系的分页数据</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class UserRolePagedQuery(UserRoleQueryDto QueryDto) : PagedQuery<UserRoleQueryDto, UserRolePagedDto>(QueryDto);
