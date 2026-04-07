/*
 * 文件名称: UserQueryHandlers.cs
 * 功能描述: 用户相关查询处理器，包含用户的所有查询处理逻辑
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

using Application.Abstractions.Queries;
using Application.Contracts.Queries;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Constants;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using AutoMapper;

namespace Application.Queries;

/// <summary>
/// 用户根据ID查询处理器
/// <para>用于处理用户根据ID获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class UserByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregateByIdQueryHandler<UserByIdQuery, User, IUserRepository, UserDetailDto>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.User.PREFIX;
}

/// <summary>
/// 用户列表查询处理器
/// <para>用于处理用户列表获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class UserListQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : DomainListQueryHandler<UserListQuery, User, IUserRepository, UserListDto, UserQueryParameters>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.User.PREFIX;
}

/// <summary>
/// 用户分页查询处理器
/// <para>用于处理用户分页获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class UserPagedQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : DomainPagedQueryHandler<UserPagedQuery, User, IUserRepository, UserPagedDto, UserQueryParameters>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.User.PREFIX;
}
