/*
 * 文件名称: RoleQueries.cs
 * 功能描述: 角色相关查询类，包含角色的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// 角色根据ID查询
/// <para>用于根据ID获取角色详情</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class RoleByIdQuery(RoleQueryDto QueryDto) : AggregateByIdQuery<RoleQueryDto, RoleDetailDto>(QueryDto);

/// <summary>
/// 角色列表查询
/// <para>用于获取角色列表</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class RoleListQuery(RoleQueryDto QueryDto) : AggregateListQuery<RoleQueryDto, RoleListDto>(QueryDto);

/// <summary>
/// 角色分页查询
/// <para>用于分页获取角色列表</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class RolePagedQuery(RoleQueryDto QueryDto) : AggregatePagedQuery<RoleQueryDto, RolePagedDto>(QueryDto);
