/*
 * 文件名称: RoleQueries.cs
 * 功能描述: 角色相关查询类，包含角色的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Contracts.Queries;

/// <summary>
/// 角色根据ID查询
/// <para>用于根据ID获取角色详情</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="id">角色ID</param>
public class RoleByIdQuery(Guid id) : AggregateByIdQuery<Role, IRoleRepository, RoleDetailDto>(id) {
}

/// <summary>
/// 角色列表查询
/// <para>用于获取角色列表</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public class RoleListQuery(RoleQueryParameters queryParameters) : DomainListQuery<Role, IRoleRepository, RoleListDto, RoleQueryParameters>(queryParameters) {
}

/// <summary>
/// 角色分页查询
/// <para>用于分页获取角色列表</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public class RolePagedQuery(RoleQueryParameters queryParameters) : DomainPagedQuery<Role, IRoleRepository, RolePagedDto, RoleQueryParameters>(queryParameters) {
}


