/*
 * 文件名称: RoleQueryHandlers.cs
 * 功能描述: 角色相关查询处理器，包含角色的所有查询处理逻辑
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
/// 角色根据ID查询处理器
/// <para>用于处理角色根据ID获取操作，支持缓存</para>
/// </summary>
public class RoleByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IRoleRepository repository,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<RoleByIdQueryHandler> logger) : AggregateByIdQueryHandler<Role, IRoleRepository, RoleByIdQuery, RoleQueryDto, RoleDetailDto>(unitOfWork, repository, mapper, cacheProvider, logger) {

}

/// <summary>
/// 角色列表查询处理器
/// <para>用于处理角色列表获取操作，支持缓存</para>
/// </summary>
public class RoleListQueryHandler(
    IUnitOfWork unitOfWork,
    IRoleRepository repository,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<RoleListQueryHandler> logger) : AggregateListQueryHandler<Role, IRoleRepository, RoleListQuery, RoleQueryDto, RoleListDto>(unitOfWork, repository, mapper, cacheProvider, logger) {

    /// <summary>
    /// 构建查询条件
    /// </summary>
    protected override List<Expression<Func<Role, bool>>> BuildPredicates(RoleListQuery query) {
        return RoleQueryBuilder.BuildPredicates(query.QueryDto, base.BuildPredicates(query));
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    protected override StringBuilder BuildCacheParams(RoleListQuery query) {
        return RoleQueryBuilder.BuildCacheParams(query.QueryDto, base.BuildCacheParams(query));
    }
}

/// <summary>
/// 角色分页查询处理器
/// <para>用于处理角色分页获取操作，支持缓存</para>
/// </summary>
public class RolePagedQueryHandler(
    IUnitOfWork unitOfWork,
    IRoleRepository repository,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<RolePagedQueryHandler> logger) : AggregatePagedQueryHandler<Role, IRoleRepository, RolePagedQuery, RoleQueryDto, RolePagedDto>(unitOfWork, repository, mapper, cacheProvider, logger) {

    /// <summary>
    /// 构建查询条件
    /// </summary>
    protected override List<Expression<Func<Role, bool>>> BuildPredicates(RolePagedQuery query) {
        return RoleQueryBuilder.BuildPredicates(query.QueryDto, base.BuildPredicates(query));
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    protected override StringBuilder BuildCacheParams(RolePagedQuery query) {
        return RoleQueryBuilder.BuildCacheParams(query.QueryDto, base.BuildCacheParams(query));
    }
}
