/*
 * 文件名称: RoleQueryHandlers.cs
 * 功能描述: 角色相关查询处理器，包含角色的所有查询处理逻辑
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Abstractions.Queries;
using Application.Contracts.Queries;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Constants;
using Domain.Shared.Caches;
using Domain.Shared.Units;
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
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class RoleByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<RoleByIdQueryHandler> logger) : AggregateByIdQueryHandler<Role, IRoleRepository, RoleByIdQuery, RoleQueryDto, RoleDetailDto>(unitOfWork, mapper, cacheProvider, logger) {

}

/// <summary>
/// 角色列表查询处理器
/// <para>用于处理角色列表获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class RoleListQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<RoleListQueryHandler> logger) : AggregateListQueryHandler<Role, IRoleRepository, RoleListQuery, RoleQueryDto, RoleListDto>(unitOfWork, mapper, cacheProvider, logger) {


    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<Role, bool>>> BuildPredicates(RoleListQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.QueryDto != null) {
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Code)) {
                predicates.Add(r => r.Code.Contains(query.QueryDto.Code!));
            }

            if (!string.IsNullOrWhiteSpace(query.QueryDto.Name)) {
                predicates.Add(r => r.Name.Contains(query.QueryDto.Name!));
            }

            if (query.QueryDto.ParentId.HasValue) {
                predicates.Add(r => r.ParentId == query.QueryDto.ParentId.Value);
            }
        }

        return predicates;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override StringBuilder BuildCacheParams(RoleListQuery query) {
        var builder = base.BuildCacheParams(query);

        if (query.QueryDto != null) {
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Code)) {
                builder.Append($"|Code={query.QueryDto.Code}");
            }
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Name)) {
                builder.Append($"|Name={query.QueryDto.Name}");
            }
            if (query.QueryDto.ParentId.HasValue) {
                builder.Append($"|ParentId={query.QueryDto.ParentId.Value}");
            }
        }

        return builder;
    }
}

/// <summary>
/// 角色分页查询处理器
/// <para>用于处理角色分页获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class RolePagedQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<RolePagedQueryHandler> logger) : AggregatePagedQueryHandler<Role, IRoleRepository, RolePagedQuery, RoleQueryDto, RolePagedDto>(unitOfWork, mapper, cacheProvider, logger) {


    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<Role, bool>>> BuildPredicates(RolePagedQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.QueryDto != null) {
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Code)) {
                predicates.Add(r => r.Code.Contains(query.QueryDto.Code!));
            }

            if (!string.IsNullOrWhiteSpace(query.QueryDto.Name)) {
                predicates.Add(r => r.Name.Contains(query.QueryDto.Name!));
            }

            if (query.QueryDto.ParentId.HasValue) {
                predicates.Add(r => r.ParentId == query.QueryDto.ParentId.Value);
            }
        }

        return predicates;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override StringBuilder BuildCacheParams(RolePagedQuery query) {
        var builder = base.BuildCacheParams(query);

        if (query.QueryDto != null) {
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Code)) {
                builder.Append($"|Code={query.QueryDto.Code}");
            }
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Name)) {
                builder.Append($"|Name={query.QueryDto.Name}");
            }
            if (query.QueryDto.ParentId.HasValue) {
                builder.Append($"|ParentId={query.QueryDto.ParentId.Value}");
            }
        }

        return builder;
    }
}
