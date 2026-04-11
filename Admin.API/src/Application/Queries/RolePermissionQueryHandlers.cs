/*
 * 文件名称: RolePermissionQueryHandlers.cs
 * 功能描述: 角色权限关联查询处理器，处理角色权限查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Abstractions.Queries;
using Application.Contracts.Dtos;
using Application.Contracts.Queries;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Linq.Expressions;

namespace Application.Queries;

/// <summary>
/// 角色权限分页查询处理器
/// <para>处理角色权限关联关系的分页查询</para>
/// </summary>
public class RolePermissionPagedQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<RolePermissionPagedQueryHandler> logger) : DomainPagedQueryHandler<RolePermissionPagedQuery, RolePermission, IRolePermissionRepository, RolePermissionPagedDto>(unitOfWork, mapper, cacheProvider) {
    private readonly ILogger<RolePermissionPagedQueryHandler> _logger = logger;

    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => "rolePermission";

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<RolePermission, bool>>> BuildPredicates(RolePermissionPagedQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.RoleId.HasValue) {
            predicates.Add(t => t.RoleId == query.RoleId.Value);
        }
        if (query.PermissionId.HasValue) {
            predicates.Add(t => t.PermissionId == query.PermissionId.Value);
        }

        _logger.LogDebug("构建角色权限分页查询条件 | RoleId: {RoleId} | PermissionId: {PermissionId} | PredicateCount: {Count}",
            query.RoleId, query.PermissionId, predicates.Count);

        return predicates;
    }

    /// <summary>
    /// 构建排序条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>排序条件字典</returns>
    protected override IDictionary<Expression<Func<RolePermission, object>>, OrderByType> BuildOrders(RolePermissionPagedQuery query) {
        var orders = base.BuildOrders(query);
        orders.Add(t => t.RoleId, OrderByType.Asc);
        return orders;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override string BuildCacheKey(RolePermissionPagedQuery query) {
        var parts = new List<string>();

        if (query.RoleId.HasValue) {
            parts.Add($"RoleId={query.RoleId.Value}");
        }
        if (query.PermissionId.HasValue) {
            parts.Add($"PermissionId={query.PermissionId.Value}");
        }

        return string.Join("|", parts);
    }
}

/// <summary>
/// 角色权限列表查询处理器
/// <para>处理角色权限关联关系的列表查询</para>
/// </summary>
public class RolePermissionListQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<RolePermissionListQueryHandler> logger) : DomainListQueryHandler<RolePermissionListQuery, RolePermission, IRolePermissionRepository, RolePermissionListDto>(unitOfWork, mapper, cacheProvider) {
    private readonly ILogger<RolePermissionListQueryHandler> _logger = logger;

    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => "rolePermission";

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<RolePermission, bool>>> BuildPredicates(RolePermissionListQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.RoleId.HasValue) {
            predicates.Add(t => t.RoleId == query.RoleId.Value);
        }
        if (query.PermissionId.HasValue) {
            predicates.Add(t => t.PermissionId == query.PermissionId.Value);
        }

        _logger.LogDebug("构建角色权限列表查询条件 | RoleId: {RoleId} | PermissionId: {PermissionId} | PredicateCount: {Count}",
            query.RoleId, query.PermissionId, predicates.Count);

        return predicates;
    }

    /// <summary>
    /// 构建排序条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>排序条件字典</returns>
    protected override IDictionary<Expression<Func<RolePermission, object>>, OrderByType> BuildOrders(RolePermissionListQuery query) {
        var orders = base.BuildOrders(query);
        orders.Add(t => t.RoleId, OrderByType.Asc);
        return orders;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override string BuildCacheKey(RolePermissionListQuery query) {
        var parts = new List<string>();

        if (query.RoleId.HasValue) {
            parts.Add($"RoleId={query.RoleId.Value}");
        }
        if (query.PermissionId.HasValue) {
            parts.Add($"PermissionId={query.PermissionId.Value}");
        }

        return string.Join("|", parts);
    }
}

/// <summary>
/// 角色权限列表查询处理器
/// </summary>
public class RolePermissionsQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<RolePermissionsQueryHandler> logger) : IRequestHandler<RolePermissionsQuery, List<Permission>> {
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<RolePermissionsQueryHandler> _logger = logger;

    /// <summary>
    /// 处理角色权限列表查询
    /// </summary>
    public async Task<List<Permission>> Handle(RolePermissionsQuery request, CancellationToken cancellationToken) {
        var rolePermissionRepository = _unitOfWork.GetRepository<IRolePermissionRepository, RolePermission>();
        var permissions = await rolePermissionRepository.GetPermissionsByRoleIdAsync(request.RoleId, cancellationToken);

        _logger.LogDebug("查询角色权限列表 | RoleId: {RoleId} | PermissionCount: {Count}", 
            request.RoleId, permissions.Count);

        return permissions;
    }
}

/// <summary>
/// 角色权限ID列表查询处理器
/// </summary>
public class RolePermissionIdsQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<RolePermissionIdsQueryHandler> logger) : IRequestHandler<RolePermissionIdsQuery, List<Guid>> {
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<RolePermissionIdsQueryHandler> _logger = logger;

    /// <summary>
    /// 处理角色权限ID列表查询
    /// </summary>
    public async Task<List<Guid>> Handle(RolePermissionIdsQuery request, CancellationToken cancellationToken) {
        var rolePermissionRepository = _unitOfWork.GetRepository<IRolePermissionRepository, RolePermission>();
        var permissionIds = await rolePermissionRepository.GetPermissionIdsByRoleIdAsync(request.RoleId, cancellationToken);

        _logger.LogDebug("查询角色权限ID列表 | RoleId: {RoleId} | PermissionCount: {Count}", 
            request.RoleId, permissionIds.Count);

        return permissionIds;
    }
}
