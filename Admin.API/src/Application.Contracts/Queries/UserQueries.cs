/*
 * 文件名称: UserQueries.cs
 * 功能描述: 用户相关查询类，包含用户的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// 用户根据ID查询
/// <para>用于根据ID获取用户详情</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class UserByIdQuery(UserQueryDto QueryDto) : AggregateByIdQuery<UserQueryDto, UserDetailDto>(QueryDto);

/// <summary>
/// 用户列表查询
/// <para>用于获取用户列表</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class UserListQuery(UserQueryDto QueryDto) : AggregateListQuery<UserQueryDto, UserListDto>(QueryDto);

/// <summary>
/// 用户分页查询
/// <para>用于分页获取用户列表</para>
/// </summary>
/// <param name="QueryDto">查询参数 DTO</param>
public class UserPagedQuery(UserQueryDto QueryDto) : AggregatePagedQuery<UserQueryDto, UserPagedDto>(QueryDto);
