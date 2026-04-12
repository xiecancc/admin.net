/*
 * 文件名称: ApiPermissionQueries.cs
 * 功能描述: API权限相关查询类，包含API权限的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// API权限根据ID查询
/// <para>用于根据ID获取API权限详情</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class ApiPermissionByIdQuery(ApiPermissionQueryDto QueryDto) : AggregateByIdQuery<ApiPermissionQueryDto, ApiPermissionDetailDto>(QueryDto);

/// <summary>
/// API权限列表查询
/// <para>用于获取API权限列表</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class ApiPermissionListQuery(ApiPermissionQueryDto QueryDto) : AggregateListQuery<ApiPermissionQueryDto, ApiPermissionListDto>(QueryDto);

/// <summary>
/// API权限分页查询
/// <para>用于分页获取API权限列表</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class ApiPermissionPagedQuery(ApiPermissionQueryDto QueryDto) : AggregatePagedQuery<ApiPermissionQueryDto, ApiPermissionPagedDto>(QueryDto);
