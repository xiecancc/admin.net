/*
 * 文件名称: MenuPermissionQueryHandlers.cs
 * 功能描述: 菜单权限相关查询处理器，包含菜单权限的所有查询处理逻辑
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
using System.Linq.Expressions;
using System.Text;

namespace Application.Queries;

/// <summary>
/// 菜单权限根据ID查询处理器
/// <para>用于处理菜单权限根据ID获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class MenuPermissionByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<MenuPermissionByIdQueryHandler> logger) : AggregateByIdQueryHandler<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionByIdQuery, MenuPermissionQueryDto, MenuPermissionDetailDto>(unitOfWork, mapper, cacheProvider, logger) {

}

/// <summary>
/// 菜单权限列表查询处理器
/// <para>用于处理菜单权限列表获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class MenuPermissionListQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<MenuPermissionListQueryHandler> logger) : AggregateListQueryHandler<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionListQuery, MenuPermissionQueryDto, MenuPermissionListDto>(unitOfWork, mapper, cacheProvider, logger) {

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<MenuPermission, bool>>> BuildPredicates(MenuPermissionListQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.QueryDto != null) {
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Code)) {
                predicates.Add(p => p.Code.Contains(query.QueryDto.Code));
            }

            if (!string.IsNullOrWhiteSpace(query.QueryDto.Name)) {
                predicates.Add(p => p.Name.Contains(query.QueryDto.Name));
            }

            if (!string.IsNullOrWhiteSpace(query.QueryDto.Path)) {
                predicates.Add(p => p.Path != null && p.Path.Contains(query.QueryDto.Path));
            }

            if (query.QueryDto.IsVisible.HasValue) {
                predicates.Add(p => p.IsVisible == query.QueryDto.IsVisible.Value);
            }

            if (query.QueryDto.ParentId.HasValue) {
                predicates.Add(p => p.ParentId == query.QueryDto.ParentId.Value);
            }
        }

        return predicates;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override StringBuilder BuildCacheParams(MenuPermissionListQuery query) {
        var builder = base.BuildCacheParams(query);

        if (query.QueryDto != null) {
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Code)) {
                builder.Append($"|Code={query.QueryDto.Code}");
            }
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Name)) {
                builder.Append($"|Name={query.QueryDto.Name}");
            }
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Path)) {
                builder.Append($"|Path={query.QueryDto.Path}");
            }
            if (query.QueryDto.IsVisible.HasValue) {
                builder.Append($"|IsVisible={query.QueryDto.IsVisible.Value}");
            }
            if (query.QueryDto.ParentId.HasValue) {
                builder.Append($"|ParentId={query.QueryDto.ParentId.Value}");
            }
        }

        return builder;
    }
}

/// <summary>
/// 菜单权限分页查询处理器
/// <para>用于处理菜单权限分页获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class MenuPermissionPagedQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<MenuPermissionPagedQueryHandler> logger) : AggregatePagedQueryHandler<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionPagedQuery, MenuPermissionQueryDto, MenuPermissionPagedDto>(unitOfWork, mapper, cacheProvider, logger) {

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<MenuPermission, bool>>> BuildPredicates(MenuPermissionPagedQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.QueryDto != null) {
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Code)) {
                predicates.Add(p => p.Code.Contains(query.QueryDto.Code));
            }

            if (!string.IsNullOrWhiteSpace(query.QueryDto.Name)) {
                predicates.Add(p => p.Name.Contains(query.QueryDto.Name));
            }

            if (!string.IsNullOrWhiteSpace(query.QueryDto.Path)) {
                predicates.Add(p => p.Path != null && p.Path.Contains(query.QueryDto.Path));
            }

            if (query.QueryDto.IsVisible.HasValue) {
                predicates.Add(p => p.IsVisible == query.QueryDto.IsVisible.Value);
            }

            if (query.QueryDto.ParentId.HasValue) {
                predicates.Add(p => p.ParentId == query.QueryDto.ParentId.Value);
            }
        }

        return predicates;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override StringBuilder BuildCacheParams(MenuPermissionPagedQuery query) {
        var builder = base.BuildCacheParams(query);

        if (query.QueryDto != null) {
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Code)) {
                builder.Append($"|Code={query.QueryDto.Code}");
            }
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Name)) {
                builder.Append($"|Name={query.QueryDto.Name}");
            }
            if (!string.IsNullOrWhiteSpace(query.QueryDto.Path)) {
                builder.Append($"|Path={query.QueryDto.Path}");
            }
            if (query.QueryDto.IsVisible.HasValue) {
                builder.Append($"|IsVisible={query.QueryDto.IsVisible.Value}");
            }
            if (query.QueryDto.ParentId.HasValue) {
                builder.Append($"|ParentId={query.QueryDto.ParentId.Value}");
            }
        }

        return builder;
    }
}
