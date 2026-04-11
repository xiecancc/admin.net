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
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using AutoMapper;
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
public class RoleByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregateByIdQueryHandler<RoleByIdQuery, Role, IRoleRepository, RoleDetailDto>(unitOfWork, mapper, cacheProvider) {

}

/// <summary>
/// 角色列表查询处理器
/// <para>用于处理角色列表获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class RoleListQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregateListQueryHandler<RoleListQuery, Role, IRoleRepository, RoleListDto>(unitOfWork, mapper, cacheProvider) {


    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<Role, bool>>> BuildPredicates(RoleListQuery query) {
        var predicates = base.BuildPredicates(query);

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            predicates.Add(r => r.Code.Contains(query.Code!));
        }

        if (!string.IsNullOrWhiteSpace(query.Name)) {
            predicates.Add(r => r.Name.Contains(query.Name!));
        }

        if (query.ParentId.HasValue) {
            predicates.Add(r => r.ParentId == query.ParentId.Value);
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

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            builder.Append($"|Code={query.Code}");
        }
        if (!string.IsNullOrWhiteSpace(query.Name)) {
            builder.Append($"|Name={query.Name}");
        }
        if (query.ParentId.HasValue) {
            builder.Append($"|ParentId={query.ParentId.Value}");
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
public class RolePagedQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregatePagedQueryHandler<RolePagedQuery, Role, IRoleRepository, RolePagedDto>(unitOfWork, mapper, cacheProvider) {


    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<Role, bool>>> BuildPredicates(RolePagedQuery query) {
        var predicates = base.BuildPredicates(query);

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            predicates.Add(r => r.Code.Contains(query.Code!));
        }

        if (!string.IsNullOrWhiteSpace(query.Name)) {
            predicates.Add(r => r.Name.Contains(query.Name!));
        }

        if (query.ParentId.HasValue) {
            predicates.Add(r => r.ParentId == query.ParentId.Value);
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

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            builder.Append($"|Code={query.Code}");
        }
        if (!string.IsNullOrWhiteSpace(query.Name)) {
            builder.Append($"|Name={query.Name}");
        }
        if (query.ParentId.HasValue) {
            builder.Append($"|ParentId={query.ParentId.Value}");
        }

        return builder;
    }
}
