/*
 * 文件名称: RolePermissionCommandHandlers.cs
 * 功能描述: 角色权限关联命令处理器，处理角色权限的分配和移除操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Application.Contracts.Commands;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Units;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// 为角色分配权限命令处理器
/// </summary>
public class AssignPermissionsToRoleCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<AssignPermissionsToRoleCommandHandler> logger) : IRequestHandler<AssignPermissionsToRoleCommand, bool> {
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<AssignPermissionsToRoleCommandHandler> _logger = logger;

    /// <summary>
    /// 处理为角色分配权限命令
    /// </summary>
    public async Task<bool> Handle(AssignPermissionsToRoleCommand request, CancellationToken cancellationToken) {
        var rolePermissionRepository = _unitOfWork.GetRepository<IRolePermissionRepository, RolePermission>();
        var result = await rolePermissionRepository.AssignPermissionsToRoleAsync(request.RoleId, request.PermissionIds, cancellationToken);

        if (result) {
            _logger.LogInformation("为角色分配权限成功 | RoleId: {RoleId} | PermissionIds: {PermissionIds}", 
                request.RoleId, string.Join(",", request.PermissionIds));
        }

        return result;
    }
}

/// <summary>
/// 移除角色权限命令处理器
/// </summary>
public class RemovePermissionsFromRoleCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<RemovePermissionsFromRoleCommandHandler> logger) : IRequestHandler<RemovePermissionsFromRoleCommand, bool> {
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<RemovePermissionsFromRoleCommandHandler> _logger = logger;

    /// <summary>
    /// 处理移除角色权限命令
    /// </summary>
    public async Task<bool> Handle(RemovePermissionsFromRoleCommand request, CancellationToken cancellationToken) {
        var rolePermissionRepository = _unitOfWork.GetRepository<IRolePermissionRepository, RolePermission>();
        var result = await rolePermissionRepository.RemovePermissionsFromRoleAsync(request.RoleId, request.PermissionIds, cancellationToken);

        if (result) {
            _logger.LogInformation("移除角色权限成功 | RoleId: {RoleId} | PermissionIds: {PermissionIds}", 
                request.RoleId, string.Join(",", request.PermissionIds));
        }

        return result;
    }
}
