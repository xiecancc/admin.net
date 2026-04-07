/*
 * 文件名称: UserQueries.cs
 * 功能描述: 用户相关查询类，包含用户的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Contracts.Queries;

/// <summary>
/// 用户根据ID查询
/// <para>用于根据ID获取用户详情</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="id">用户ID</param>
public class UserByIdQuery(Guid id) : AggregateByIdQuery<User, IUserRepository, UserDetailDto>(id) {
}

/// <summary>
/// 用户列表查询
/// <para>用于获取用户列表</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public class UserListQuery(UserQueryParameters queryParameters) : DomainListQuery<User, IUserRepository, UserListDto, UserQueryParameters>(queryParameters) {
}

/// <summary>
/// 用户分页查询
/// <para>用于分页获取用户列表</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public class UserPagedQuery(UserQueryParameters queryParameters) : DomainPagedQuery<User, IUserRepository, UserPagedDto, UserQueryParameters>(queryParameters) {
}


