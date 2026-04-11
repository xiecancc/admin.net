/*
 * 文件名称: ApiPermissionQueryHandlers.cs
 * 功能描述: API权限查询处理器，处理API权限相关的查询
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
using System.Linq.Expressions;

namespace Application.Queries;

/// <summary>
/// API权限根据ID查询处理器
/// <para>处理API权限的根据ID查询操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class ApiPermissionByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregateByIdQueryHandler<ApiPermissionByIdQuery, ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionDetailDto>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.Permission.Prefix;
}

/// <summary>
/// API权限列表查询处理器
/// <para>处理API权限的列表查询操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class ApiPermissionListQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregateListQueryHandler<ApiPermissionListQuery, ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionListDto>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.Permission.Prefix;

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<ApiPermission, bool>>> BuildPredicates(ApiPermissionListQuery query) {
        var predicates = base.BuildPredicates(query);

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            predicates.Add(p => p.Code.Contains(query.Code));
        }

        if (!string.IsNullOrWhiteSpace(query.Name)) {
            predicates.Add(p => p.Name.Contains(query.Name));
        }

        if (!string.IsNullOrWhiteSpace(query.ApiPath)) {
            predicates.Add(p => p.ApiPath.Contains(query.ApiPath));
        }

        if (!string.IsNullOrWhiteSpace(query.HttpMethod)) {
            predicates.Add(p => p.HttpMethod == query.HttpMethod);
        }

        if (!string.IsNullOrWhiteSpace(query.ModuleName)) {
            predicates.Add(p => p.ModuleName != null && p.ModuleName.Contains(query.ModuleName));
        }

        if (query.MenuId.HasValue) {
            predicates.Add(p => p.MenuId == query.MenuId.Value);
        }

        return predicates;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override string BuildCacheKey(ApiPermissionListQuery query) {
        var parts = new List<string>();
        var baseKey = base.BuildCacheKey(query);
        if (!string.IsNullOrEmpty(baseKey)) {
            parts.Add(baseKey);
        }

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            parts.Add($"Code={query.Code}");
        }
        if (!string.IsNullOrWhiteSpace(query.Name)) {
            parts.Add($"Name={query.Name}");
        }
        if (!string.IsNullOrWhiteSpace(query.ApiPath)) {
            parts.Add($"ApiPath={query.ApiPath}");
        }
        if (!string.IsNullOrWhiteSpace(query.HttpMethod)) {
            parts.Add($"HttpMethod={query.HttpMethod}");
        }
        if (!string.IsNullOrWhiteSpace(query.ModuleName)) {
            parts.Add($"ModuleName={query.ModuleName}");
        }
        if (query.MenuId.HasValue) {
            parts.Add($"MenuId={query.MenuId.Value}");
        }

        return string.Join("|", parts);
    }
}

/// <summary>
/// API权限分页查询处理器
/// <para>用于处理API权限分页获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class ApiPermissionPagedQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregatePagedQueryHandler<ApiPermissionPagedQuery, ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionPagedDto>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.Permission.Prefix;

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<ApiPermission, bool>>> BuildPredicates(ApiPermissionPagedQuery query) {
        var predicates = base.BuildPredicates(query);

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            predicates.Add(p => p.Code.Contains(query.Code));
        }

        if (!string.IsNullOrWhiteSpace(query.Name)) {
            predicates.Add(p => p.Name.Contains(query.Name));
        }

        if (!string.IsNullOrWhiteSpace(query.ApiPath)) {
            predicates.Add(p => p.ApiPath.Contains(query.ApiPath));
        }

        if (!string.IsNullOrWhiteSpace(query.HttpMethod)) {
            predicates.Add(p => p.HttpMethod == query.HttpMethod);
        }

        if (!string.IsNullOrWhiteSpace(query.ModuleName)) {
            predicates.Add(p => p.ModuleName != null && p.ModuleName.Contains(query.ModuleName));
        }

        if (query.MenuId.HasValue) {
            predicates.Add(p => p.MenuId == query.MenuId.Value);
        }

        return predicates;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override string BuildCacheKey(ApiPermissionPagedQuery query) {
        var parts = new List<string>();
        var baseKey = base.BuildCacheKey(query);
        if (!string.IsNullOrEmpty(baseKey)) {
            parts.Add(baseKey);
        }

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            parts.Add($"Code={query.Code}");
        }
        if (!string.IsNullOrWhiteSpace(query.Name)) {
            parts.Add($"Name={query.Name}");
        }
        if (!string.IsNullOrWhiteSpace(query.ApiPath)) {
            parts.Add($"ApiPath={query.ApiPath}");
        }
        if (!string.IsNullOrWhiteSpace(query.HttpMethod)) {
            parts.Add($"HttpMethod={query.HttpMethod}");
        }
        if (!string.IsNullOrWhiteSpace(query.ModuleName)) {
            parts.Add($"ModuleName={query.ModuleName}");
        }
        if (query.MenuId.HasValue) {
            parts.Add($"MenuId={query.MenuId.Value}");
        }

        return string.Join("|", parts);
    }
}
