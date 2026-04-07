/*
 * 文件名称: ApiPermissionQueries.cs
 * 功能描述: API权限相关查询类，包含API权限的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Contracts.Queries;

/// <summary>
/// API权限根据ID查询
/// <para>用于根据ID获取API权限详情</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="id">API权限ID</param>
public class ApiPermissionByIdQuery(Guid id) : AggregateByIdQuery<ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionDetailDto>(id) {
}

/// <summary>
/// API权限列表查询
/// <para>用于获取API权限列表</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public class ApiPermissionListQuery(ApiPermissionQueryParameters queryParameters) : DomainListQuery<ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionListDto, ApiPermissionQueryParameters>(queryParameters) {
}

/// <summary>
/// API权限分页查询
/// <para>用于分页获取API权限列表</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public class ApiPermissionPagedQuery(ApiPermissionQueryParameters queryParameters) : DomainPagedQuery<ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionPagedDto, ApiPermissionQueryParameters>(queryParameters) {
}


