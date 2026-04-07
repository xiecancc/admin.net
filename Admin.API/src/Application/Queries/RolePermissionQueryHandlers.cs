/*
 * 文件名称: RolePermissionQueryHandlers.cs
 * 功能描述: 角色权限关联查询处理器，处理角色权限查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Application.Contracts.Queries;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Shared.Units;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Queries;

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
