/*
 * 文件名称: MenuPermissionQueryHandlers.cs
 * 功能描述: 菜单权限相关查询处理器，包含菜单权限的所有查询处理逻辑
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
using System.Text;

namespace Application.Queries;

/// <summary>
/// 菜单权限根据ID查询处理器
/// <para>用于处理菜单权限根据ID获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class MenuPermissionByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregateByIdQueryHandler<MenuPermissionByIdQuery, MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionDetailDto>(unitOfWork, mapper, cacheProvider) {

}

/// <summary>
/// 菜单权限列表查询处理器
/// <para>用于处理菜单权限列表获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class MenuPermissionListQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregateListQueryHandler<MenuPermissionListQuery, MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionListDto>(unitOfWork, mapper, cacheProvider) {


    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<MenuPermission, bool>>> BuildPredicates(MenuPermissionListQuery query) {
        var predicates = base.BuildPredicates(query);

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            predicates.Add(p => p.Code.Contains(query.Code));
        }

        if (!string.IsNullOrWhiteSpace(query.Name)) {
            predicates.Add(p => p.Name.Contains(query.Name));
        }

        if (!string.IsNullOrWhiteSpace(query.Path)) {
            predicates.Add(p => p.Path != null && p.Path.Contains(query.Path));
        }

        if (query.IsVisible.HasValue) {
            predicates.Add(p => p.IsVisible == query.IsVisible.Value);
        }

        if (query.ParentId.HasValue) {
            predicates.Add(p => p.ParentId == query.ParentId.Value);
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

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            builder.Append($"|Code={query.Code}");
        }
        if (!string.IsNullOrWhiteSpace(query.Name)) {
            builder.Append($"|Name={query.Name}");
        }
        if (!string.IsNullOrWhiteSpace(query.Path)) {
            builder.Append($"|Path={query.Path}");
        }
        if (query.IsVisible.HasValue) {
            builder.Append($"|IsVisible={query.IsVisible.Value}");
        }
        if (query.ParentId.HasValue) {
            builder.Append($"|ParentId={query.ParentId.Value}");
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
public class MenuPermissionPagedQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregatePagedQueryHandler<MenuPermissionPagedQuery, MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionPagedDto>(unitOfWork, mapper, cacheProvider) {


    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<MenuPermission, bool>>> BuildPredicates(MenuPermissionPagedQuery query) {
        var predicates = base.BuildPredicates(query);

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            predicates.Add(p => p.Code.Contains(query.Code));
        }

        if (!string.IsNullOrWhiteSpace(query.Name)) {
            predicates.Add(p => p.Name.Contains(query.Name));
        }

        if (!string.IsNullOrWhiteSpace(query.Path)) {
            predicates.Add(p => p.Path != null && p.Path.Contains(query.Path));
        }

        if (query.IsVisible.HasValue) {
            predicates.Add(p => p.IsVisible == query.IsVisible.Value);
        }

        if (query.ParentId.HasValue) {
            predicates.Add(p => p.ParentId == query.ParentId.Value);
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

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            builder.Append($"|Code={query.Code}");
        }
        if (!string.IsNullOrWhiteSpace(query.Name)) {
            builder.Append($"|Name={query.Name}");
        }
        if (!string.IsNullOrWhiteSpace(query.Path)) {
            builder.Append($"|Path={query.Path}");
        }
        if (query.IsVisible.HasValue) {
            builder.Append($"|IsVisible={query.IsVisible.Value}");
        }
        if (query.ParentId.HasValue) {
            builder.Append($"|ParentId={query.ParentId.Value}");
        }

        return builder;
    }
}
