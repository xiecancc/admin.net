/*
 * 文件名称: ButtonPermissionQueries.cs
 * 功能描述: 按钮权限相关查询类，包含按钮权限的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Contracts.Queries;

/// <summary>
/// 按钮权限根据ID查询
/// <para>用于根据ID获取按钮权限详情</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="id">按钮权限ID</param>
public class ButtonPermissionByIdQuery(Guid id) : AggregateByIdQuery<ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionDetailDto>(id) {
}

/// <summary>
/// 按钮权限列表查询
/// <para>用于获取按钮权限列表</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public class ButtonPermissionListQuery(ButtonPermissionQueryParameters queryParameters) : DomainListQuery<ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionListDto, ButtonPermissionQueryParameters>(queryParameters) {
}

/// <summary>
/// 按钮权限分页查询
/// <para>用于分页获取按钮权限列表</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public class ButtonPermissionPagedQuery(ButtonPermissionQueryParameters queryParameters) : DomainPagedQuery<ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionPagedDto, ButtonPermissionQueryParameters>(queryParameters) {
}


