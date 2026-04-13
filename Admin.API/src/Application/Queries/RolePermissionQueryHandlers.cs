/*
 * 文件名称: RolePermissionQueryHandlers.cs
 * 功能描述: 角色权限关联查询处理器，处理角色权限查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using Application.Abstractions.Queries;
using Application.Contracts.Dtos;
using Application.Contracts.Queries;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Domain.Shared.Caches;
using Domain.Shared.Units;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text;

namespace Application.Queries;

/// <summary>
/// 角色权限分页查询处理器
/// <para>处理角色权限关联关系的分页查询</para>
/// </summary>
public class RolePermissionPagedQueryHandler(
    IUnitOfWork unitOfWork,
    IRolePermissionRepository repository,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<RolePermissionPagedQueryHandler> logger) : PagedQueryHandler<RolePermission, IRolePermissionRepository, RolePermissionPagedQuery, RolePermissionQueryDto, RolePermissionPagedDto>(unitOfWork, repository, mapper, cacheProvider, logger) {

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<RolePermission, bool>>> BuildPredicates(RolePermissionPagedQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.QueryDto != null) {
            if (query.QueryDto.RoleId.HasValue) {
                predicates.Add(t => t.RoleId == query.QueryDto.RoleId.Value);
            }
            if (query.QueryDto.PermissionId.HasValue) {
                predicates.Add(t => t.PermissionId == query.QueryDto.PermissionId.Value);
            }
        }

        return predicates;
    }

    /// <summary>
    /// 构建排序条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>排序条件字典</returns>
    protected override IDictionary<Expression<Func<RolePermission, object>>, bool> BuildOrders(RolePermissionPagedQuery query) {
        var orders = base.BuildOrders(query);
        orders.Add(t => t.RoleId, false);
        return orders;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override StringBuilder BuildCacheParams(RolePermissionPagedQuery query) {
        var builder = base.BuildCacheParams(query);

        if (query.QueryDto != null) {
            if (query.QueryDto.RoleId.HasValue) {
                builder.Append($"|RoleId={query.QueryDto.RoleId.Value}");
            }
            if (query.QueryDto.PermissionId.HasValue) {
                builder.Append($"|PermissionId={query.QueryDto.PermissionId.Value}");
            }
        }

        return builder;
    }
}

/// <summary>
/// 角色权限列表查询处理器
/// <para>处理角色权限关联关系的列表查询</para>
/// </summary>
public class RolePermissionListQueryHandler(
    IUnitOfWork unitOfWork,
    IRolePermissionRepository repository,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<RolePermissionListQueryHandler> logger) : ListQueryHandler<RolePermission, IRolePermissionRepository, RolePermissionListQuery, RolePermissionQueryDto, RolePermissionListDto>(unitOfWork, repository, mapper, cacheProvider, logger) {

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<RolePermission, bool>>> BuildPredicates(RolePermissionListQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.QueryDto != null) {
            if (query.QueryDto.RoleId.HasValue) {
                predicates.Add(t => t.RoleId == query.QueryDto.RoleId.Value);
            }
            if (query.QueryDto.PermissionId.HasValue) {
                predicates.Add(t => t.PermissionId == query.QueryDto.PermissionId.Value);
            }
        }

        return predicates;
    }

    /// <summary>
    /// 构建排序条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>排序条件字典</returns>
    protected override IDictionary<Expression<Func<RolePermission, object>>, bool> BuildOrders(RolePermissionListQuery query) {
        var orders = base.BuildOrders(query);
        orders.Add(t => t.RoleId, false);
        return orders;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override StringBuilder BuildCacheParams(RolePermissionListQuery query) {
        var builder = base.BuildCacheParams(query);

        if (query.QueryDto != null) {
            if (query.QueryDto.RoleId.HasValue) {
                builder.Append($"|RoleId={query.QueryDto.RoleId.Value}");
            }
            if (query.QueryDto.PermissionId.HasValue) {
                builder.Append($"|PermissionId={query.QueryDto.PermissionId.Value}");
            }
        }

        return builder;
    }
}

/// <summary>
/// 角色权限列表查询处理器
/// </summary>
public class RolePermissionsQueryHandler(
    IPermissionDomainService permissionDomainService,
    IMapper mapper,
    ILogger<RolePermissionsQueryHandler> logger) : IRequestHandler<RolePermissionsQuery, List<PermissionListDto>> {
    private readonly IPermissionDomainService _permissionDomainService = permissionDomainService;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<RolePermissionsQueryHandler> _logger = logger;

    /// <summary>
    /// 处理角色权限列表查询
    /// </summary>
    public async Task<List<PermissionListDto>> Handle(RolePermissionsQuery request, CancellationToken cancellationToken) {
        var permissions = await _permissionDomainService.GetRolePermissionsAsync(request.RoleId, cancellationToken);

        _logger.LogDebug("查询角色权限列表 | RoleId: {RoleId} | PermissionCount: {Count}", 
            request.RoleId, permissions.Count);

        return _mapper.Map<List<PermissionListDto>>(permissions);
    }
}

/// <summary>
/// 角色权限ID列表查询处理器
/// </summary>
public class RolePermissionIdsQueryHandler(
    IRolePermissionRepository rolePermissionRepository,
    ILogger<RolePermissionIdsQueryHandler> logger) : IRequestHandler<RolePermissionIdsQuery, List<Guid>> {
    private readonly IRolePermissionRepository _rolePermissionRepository = rolePermissionRepository;
    private readonly ILogger<RolePermissionIdsQueryHandler> _logger = logger;

    /// <summary>
    /// 处理角色权限ID列表查询
    /// </summary>
    public async Task<List<Guid>> Handle(RolePermissionIdsQuery request, CancellationToken cancellationToken) {
        var permissionIds = await _rolePermissionRepository.GetPermissionIdsByRoleIdAsync(request.RoleId, cancellationToken);

        _logger.LogDebug("查询角色权限ID列表 | RoleId: {RoleId} | PermissionCount: {Count}", 
            request.RoleId, permissionIds.Count);

        return permissionIds;
    }
}
