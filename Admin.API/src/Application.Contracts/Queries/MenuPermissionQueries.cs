/*
 * 文件名称: MenuPermissionQueries.cs
 * 功能描述: 菜单权限相关查询类，包含菜单权限的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Contracts.Queries;

/// <summary>
/// 菜单权限根据ID查询
/// <para>用于根据ID获取菜单权限详情</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="id">菜单权限ID</param>
public class MenuPermissionByIdQuery(Guid id) : AggregateByIdQuery<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionDetailDto>(id) {
}

/// <summary>
/// 菜单权限列表查询
/// <para>用于获取菜单权限列表</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public class MenuPermissionListQuery(MenuPermissionQueryParameters queryParameters) : DomainListQuery<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionListDto, MenuPermissionQueryParameters>(queryParameters) {
}

/// <summary>
/// 菜单权限分页查询
/// <para>用于分页获取菜单权限列表</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public class MenuPermissionPagedQuery(MenuPermissionQueryParameters queryParameters) : DomainPagedQuery<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionPagedDto, MenuPermissionQueryParameters>(queryParameters) {
}


