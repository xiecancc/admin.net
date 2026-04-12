/*
 * 文件名称: MenuPermissionQueries.cs
 * 功能描述: 菜单权限相关查询类，包含菜单权限的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// 菜单权限根据ID查询
/// <para>用于根据ID获取菜单权限详情</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class MenuPermissionByIdQuery(MenuPermissionQueryDto QueryDto) : AggregateByIdQuery<MenuPermissionQueryDto, MenuPermissionDetailDto>(QueryDto);

/// <summary>
/// 菜单权限列表查询
/// <para>用于获取菜单权限列表</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class MenuPermissionListQuery(MenuPermissionQueryDto QueryDto) : AggregateListQuery<MenuPermissionQueryDto, MenuPermissionListDto>(QueryDto);

/// <summary>
/// 菜单权限分页查询
/// <para>用于分页获取菜单权限列表</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class MenuPermissionPagedQuery(MenuPermissionQueryDto QueryDto) : AggregatePagedQuery<MenuPermissionQueryDto, MenuPermissionPagedDto>(QueryDto);
