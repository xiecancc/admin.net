/*
 * 文件名称: ButtonPermissionQueryHandlers.cs
 * 功能描述: 按钮权限相关查询处理器，包含按钮权限的所有查询处理逻辑
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
using System.Linq.Expressions;
using AutoMapper;
using System.Text;

namespace Application.Queries;

/// <summary>
/// 按钮权限根据ID查询处理器
/// <para>用于处理按钮权限根据ID获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class ButtonPermissionByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregateByIdQueryHandler<ButtonPermissionByIdQuery, ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionDetailDto>(unitOfWork, mapper, cacheProvider) {

}

/// <summary>
/// 按钮权限列表查询处理器
/// <para>用于处理按钮权限列表获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class ButtonPermissionListQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregateListQueryHandler<ButtonPermissionListQuery, ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionListDto>(unitOfWork, mapper, cacheProvider) {


    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<ButtonPermission, bool>>> BuildPredicates(ButtonPermissionListQuery query) {
        var predicates = base.BuildPredicates(query);

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            predicates.Add(p => p.Code.Contains(query.Code));
        }

        if (!string.IsNullOrWhiteSpace(query.Name)) {
            predicates.Add(p => p.Name.Contains(query.Name));
        }

        if (!string.IsNullOrWhiteSpace(query.ActionType)) {
            predicates.Add(p => p.ActionType.Contains(query.ActionType));
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
    protected override StringBuilder BuildCacheParams(ButtonPermissionListQuery query) {
        var builder = base.BuildCacheParams(query);

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            builder.Append($"|Code={query.Code}");
        }
        if (!string.IsNullOrWhiteSpace(query.Name)) {
            builder.Append($"|Name={query.Name}");
        }
        if (!string.IsNullOrWhiteSpace(query.ActionType)) {
            builder.Append($"|ActionType={query.ActionType}");
        }
        if (query.MenuId.HasValue) {
            builder.Append($"|MenuId={query.MenuId.Value}");
        }

        return builder;
    }
}

/// <summary>
/// 按钮权限分页查询处理器
/// <para>用于处理按钮权限分页获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class ButtonPermissionPagedQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregatePagedQueryHandler<ButtonPermissionPagedQuery, ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionPagedDto>(unitOfWork, mapper, cacheProvider) {


    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<ButtonPermission, bool>>> BuildPredicates(ButtonPermissionPagedQuery query) {
        var predicates = base.BuildPredicates(query);

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            predicates.Add(p => p.Code.Contains(query.Code));
        }

        if (!string.IsNullOrWhiteSpace(query.Name)) {
            predicates.Add(p => p.Name.Contains(query.Name));
        }

        if (!string.IsNullOrWhiteSpace(query.ActionType)) {
            predicates.Add(p => p.ActionType.Contains(query.ActionType));
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
    protected override StringBuilder BuildCacheParams(ButtonPermissionPagedQuery query) {
        var builder = base.BuildCacheParams(query);

        if (!string.IsNullOrWhiteSpace(query.Code)) {
            builder.Append($"|Code={query.Code}");
        }
        if (!string.IsNullOrWhiteSpace(query.Name)) {
            builder.Append($"|Name={query.Name}");
        }
        if (!string.IsNullOrWhiteSpace(query.ActionType)) {
            builder.Append($"|ActionType={query.ActionType}");
        }
        if (query.MenuId.HasValue) {
            builder.Append($"|MenuId={query.MenuId.Value}");
        }

        return builder;
    }
}
