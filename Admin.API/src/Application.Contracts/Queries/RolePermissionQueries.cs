/*
 * 文件名称: RolePermissionQueries.cs
 * 功能描述: 角色权限关联查询类，包含角色权限查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// 角色权限分页查询
/// <para>用于角色权限关联关系的分页查询</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class RolePermissionPagedQuery(RolePermissionQueryDto QueryDto) : PagedQuery<RolePermissionQueryDto, RolePermissionPagedDto>(QueryDto);

/// <summary>
/// 角色权限列表查询
/// <para>用于角色权限关联关系的列表查询</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class RolePermissionListQuery(RolePermissionQueryDto QueryDto) : ListQuery<RolePermissionQueryDto, RolePermissionListDto>(QueryDto);

/// <summary>
/// 角色权限列表查询
/// <para>获取指定角色的所有权限</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class RolePermissionsQuery(RolePermissionQueryDto QueryDto) : Query<RolePermissionQueryDto, List<PermissionListDto>>(QueryDto)
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }
}

/// <summary>
/// 角色权限ID列表查询
/// <para>获取指定角色的权限ID列表</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class RolePermissionIdsQuery(RolePermissionQueryDto QueryDto) : Query<RolePermissionQueryDto, List<Guid>>(QueryDto)
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }
}
