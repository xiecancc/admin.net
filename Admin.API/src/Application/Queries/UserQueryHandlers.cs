/*
 * 文件名称: UserQueryHandlers.cs
 * 功能描述: 用户相关查询处理器，包含用户的所有查询处理逻辑
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using Application.Abstractions.Queries;
using Application.Contracts.Queries;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Constants;
using Domain.Shared.Caches;
using Domain.Shared.Units;
using Application.Caching;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Linq.Expressions;
using System.Text;

namespace Application.Queries;

/// <summary>
/// 用户根据ID查询处理器
/// <para>用于处理用户根据ID获取操作，支持缓存</para>
/// </summary>
public class UserByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IUserRepository repository,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<UserByIdQueryHandler> logger) : AggregateByIdQueryHandler<User, IUserRepository, UserByIdQuery, UserQueryDto, UserDetailDto>(unitOfWork, repository, mapper, cacheProvider, logger) {

    /// <summary>
    /// 获取缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.User.Prefix;
}

/// <summary>
/// 用户列表查询处理器
/// <para>用于处理用户列表获取操作，支持缓存和条件过滤</para>
/// </summary>
public class UserListQueryHandler(
    IUnitOfWork unitOfWork,
    IUserRepository repository,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<UserListQueryHandler> logger) : AggregateListQueryHandler<User, IUserRepository, UserListQuery, UserQueryDto, UserListDto>(unitOfWork, repository, mapper, cacheProvider, logger) {

    /// <summary>
    /// 获取缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.User.Prefix;

    /// <summary>
    /// 构建查询条件
    /// </summary>
    protected override List<Expression<Func<User, bool>>> BuildPredicates(UserListQuery query) {
        return UserQueryBuilder.BuildPredicates(query.QueryDto, base.BuildPredicates(query));
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    protected override StringBuilder BuildCacheParams(UserListQuery query) {
        return UserQueryBuilder.BuildCacheParams(query.QueryDto, base.BuildCacheParams(query));
    }
}

/// <summary>
/// 用户分页查询处理器
/// <para>用于处理用户分页获取操作，支持缓存和条件过滤</para>
/// </summary>
public class UserPagedQueryHandler(
    IUnitOfWork unitOfWork,
    IUserRepository repository,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<UserPagedQueryHandler> logger) : AggregatePagedQueryHandler<User, IUserRepository, UserPagedQuery, UserQueryDto, UserPagedDto>(unitOfWork, repository, mapper, cacheProvider, logger) {

    /// <summary>
    /// 获取缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.User.Prefix;

    /// <summary>
    /// 构建查询条件
    /// </summary>
    protected override List<Expression<Func<User, bool>>> BuildPredicates(UserPagedQuery query) {
        return UserQueryBuilder.BuildPredicates(query.QueryDto, base.BuildPredicates(query));
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    protected override StringBuilder BuildCacheParams(UserPagedQuery query) {
        return UserQueryBuilder.BuildCacheParams(query.QueryDto, base.BuildCacheParams(query));
    }
}
