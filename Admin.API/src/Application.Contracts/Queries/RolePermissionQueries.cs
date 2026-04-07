/*
 * 文件名称: RolePermissionQueries.cs
 * 功能描述: 角色权限关联关系查询类，包含角色权限关系的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Contracts.Queries;

/// <summary>
/// 角色权限关联列表查询
/// <para>用于获取角色权限关联关系的列表</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public class RolePermissionListQuery(RolePermissionQueryParameters queryParameters) : DomainListQuery<RolePermission, IRolePermissionRepository, RolePermissionListDto, RolePermissionQueryParameters>(queryParameters);

/// <summary>
/// 角色权限关联分页查询
/// <para>用于获取角色权限关联关系的分页数据</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public class RolePermissionPagedQuery(RolePermissionQueryParameters queryParameters) : DomainPagedQuery<RolePermission, IRolePermissionRepository, RolePermissionPagedDto, RolePermissionQueryParameters>(queryParameters);
