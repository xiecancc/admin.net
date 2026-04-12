/*
 * 文件名称: UserQueryHandlers.cs
 * 功能描述: 用户相关查询处理器，包含用户的所有查询处理逻辑
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
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
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Linq.Expressions;
using System.Text;

namespace Application.Queries;

/// <summary>
/// 用户根据ID查询处理器
/// <para>用于处理用户根据ID获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
/// <exception cref="ArgumentNullException">当参数为 null 时抛出</exception>
public class UserByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<UserByIdQueryHandler> logger) : AggregateByIdQueryHandler<User, IUserRepository, UserByIdQuery, UserQueryDto, UserDetailDto>(unitOfWork, mapper, cacheProvider, logger) {

    /// <summary>
    /// 获取缓存键前缀
    /// </summary>
    /// <value>用户模块缓存键前缀</value>
    protected override string CacheKeyPrefix => CacheKeyConstants.User.Prefix;
}

/// <summary>
/// 用户列表查询处理器
/// <para>用于处理用户列表获取操作，支持缓存和条件过滤</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
/// <exception cref="ArgumentNullException">当参数为 null 时抛出</exception>
public class UserListQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<UserListQueryHandler> logger) : AggregateListQueryHandler<User, IUserRepository, UserListQuery, UserQueryDto, UserListDto>(unitOfWork, mapper, cacheProvider, logger) {

    /// <summary>
    /// 获取缓存键前缀
    /// </summary>
    /// <value>用户模块缓存键前缀</value>
    protected override string CacheKeyPrefix => CacheKeyConstants.User.Prefix;

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    /// <remarks>
    /// <para>支持的过滤条件：</para>
    /// <list type="bullet">
    ///   <item>Email：邮箱模糊匹配</item>
    ///   <item>NickName：昵称模糊匹配</item>
    ///   <item>Phone：手机号模糊匹配</item>
    /// </list>
    /// </remarks>
    protected override List<Expression<Func<User, bool>>> BuildPredicates(UserListQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.QueryDto != null) {
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Email)) {
                predicates.Add(u => u.Email.Contains(query.QueryDto.Email));
            }

            if (!string.IsNullOrWhiteSpace(query.QueryDto.NickName)) {
                predicates.Add(u => u.NickName != null && u.NickName.Contains(query.QueryDto.NickName));
            }

            if (!string.IsNullOrWhiteSpace(query.QueryDto.Phone)) {
                predicates.Add(u => u.Phone != null && u.Phone.Contains(query.QueryDto.Phone));
            }
        }

        return predicates;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override StringBuilder BuildCacheParams(UserListQuery query) {
        var builder = base.BuildCacheParams(query);

        if (query.QueryDto != null) {
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Email)) {
                builder.Append($"|Email={query.QueryDto.Email}");
            }
            if (!string.IsNullOrWhiteSpace(query.QueryDto.NickName)) {
                builder.Append($"|NickName={query.QueryDto.NickName}");
            }
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Phone)) {
                builder.Append($"|Phone={query.QueryDto.Phone}");
            }
        }

        return builder;
    }
}

/// <summary>
/// 用户分页查询处理器
/// <para>用于处理用户分页获取操作，支持缓存和条件过滤</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
/// <exception cref="ArgumentNullException">当参数为 null 时抛出</exception>
public class UserPagedQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<UserPagedQueryHandler> logger) : AggregatePagedQueryHandler<User, IUserRepository, UserPagedQuery, UserQueryDto, UserPagedDto>(unitOfWork, mapper, cacheProvider, logger) {

    /// <summary>
    /// 获取缓存键前缀
    /// </summary>
    /// <value>用户模块缓存键前缀</value>
    protected override string CacheKeyPrefix => CacheKeyConstants.User.Prefix;

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    /// <remarks>
    /// <para>支持的过滤条件：</para>
    /// <list type="bullet">
    ///   <item>Email：邮箱模糊匹配</item>
    ///   <item>NickName：昵称模糊匹配</item>
    ///   <item>Phone：手机号模糊匹配</item>
    /// </list>
    /// </remarks>
    protected override List<Expression<Func<User, bool>>> BuildPredicates(UserPagedQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.QueryDto != null) {
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Email)) {
                predicates.Add(u => u.Email.Contains(query.QueryDto.Email));
            }

            if (!string.IsNullOrWhiteSpace(query.QueryDto.NickName)) {
                predicates.Add(u => u.NickName != null && u.NickName.Contains(query.QueryDto.NickName));
            }

            if (!string.IsNullOrWhiteSpace(query.QueryDto.Phone)) {
                predicates.Add(u => u.Phone != null && u.Phone.Contains(query.QueryDto.Phone));
            }
        }

        return predicates;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override StringBuilder BuildCacheParams(UserPagedQuery query) {
        var builder = base.BuildCacheParams(query);

        if (query.QueryDto != null) {
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Email)) {
                builder.Append($"|Email={query.QueryDto.Email}");
            }
            if (!string.IsNullOrWhiteSpace(query.QueryDto.NickName)) {
                builder.Append($"|NickName={query.QueryDto.NickName}");
            }
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Phone)) {
                builder.Append($"|Phone={query.QueryDto.Phone}");
            }
        }

        return builder;
    }
}
