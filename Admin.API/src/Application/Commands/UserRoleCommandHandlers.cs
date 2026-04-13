/*
 * 文件名称: UserRoleCommandHandlers.cs
 * 功能描述: 用户角色关联命令处理器，处理用户角色的分配和移除操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using Application.Contracts.Commands;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Units;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// 为用户分配角色命令处理器
/// </summary>
public class AssignRolesToUserCommandHandler(
    IUserRoleRepository userRoleRepository,
    ILogger<AssignRolesToUserCommandHandler> logger) : IRequestHandler<AssignRolesToUserCommand, bool> {
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly ILogger<AssignRolesToUserCommandHandler> _logger = logger;

    /// <summary>
    /// 处理为用户分配角色命令
    /// </summary>
    public async Task<bool> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken) {
        var result = await _userRoleRepository.AssignRolesToUserAsync(request.UserId, request.RoleIds, cancellationToken);

        if (result) {
            _logger.LogInformation("为用户分配角色成功 | UserId: {UserId} | RoleIds: {RoleIds}", 
                request.UserId, string.Join(",", request.RoleIds));
        }

        return result;
    }
}

/// <summary>
/// 移除用户角色命令处理器
/// </summary>
public class RemoveRolesFromUserCommandHandler(
    IUserRoleRepository userRoleRepository,
    ILogger<RemoveRolesFromUserCommandHandler> logger) : IRequestHandler<RemoveRolesFromUserCommand, bool> {
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly ILogger<RemoveRolesFromUserCommandHandler> _logger = logger;

    /// <summary>
    /// 处理移除用户角色命令
    /// </summary>
    public async Task<bool> Handle(RemoveRolesFromUserCommand request, CancellationToken cancellationToken) {
        var result = await _userRoleRepository.RemoveRolesFromUserAsync(request.UserId, request.RoleIds, cancellationToken);

        if (result) {
            _logger.LogInformation("移除用户角色成功 | UserId: {UserId} | RoleIds: {RoleIds}", 
                request.UserId, string.Join(",", request.RoleIds));
        }

        return result;
    }
}

/// <summary>
/// 为角色分配用户命令处理器
/// </summary>
public class AssignUsersToRoleCommandHandler(
    IUserRoleRepository userRoleRepository,
    ILogger<AssignUsersToRoleCommandHandler> logger) : IRequestHandler<AssignUsersToRoleCommand, bool> {
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly ILogger<AssignUsersToRoleCommandHandler> _logger = logger;

    /// <summary>
    /// 处理为角色分配用户命令
    /// </summary>
    public async Task<bool> Handle(AssignUsersToRoleCommand request, CancellationToken cancellationToken) {
        var result = await _userRoleRepository.AssignUsersToRoleAsync(request.RoleId, request.UserIds, cancellationToken);

        if (result) {
            _logger.LogInformation("为角色分配用户成功 | RoleId: {RoleId} | UserIds: {UserIds}", 
                request.RoleId, string.Join(",", request.UserIds));
        }

        return result;
    }
}

/// <summary>
/// 移除角色用户命令处理器
/// </summary>
public class RemoveUsersFromRoleCommandHandler(
    IUserRoleRepository userRoleRepository,
    ILogger<RemoveUsersFromRoleCommandHandler> logger) : IRequestHandler<RemoveUsersFromRoleCommand, bool> {
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly ILogger<RemoveUsersFromRoleCommandHandler> _logger = logger;

    /// <summary>
    /// 处理移除角色用户命令
    /// </summary>
    public async Task<bool> Handle(RemoveUsersFromRoleCommand request, CancellationToken cancellationToken) {
        var result = await _userRoleRepository.RemoveUsersFromRoleAsync(request.RoleId, request.UserIds, cancellationToken);

        if (result) {
            _logger.LogInformation("移除角色用户成功 | RoleId: {RoleId} | UserIds: {UserIds}", 
                request.RoleId, string.Join(",", request.UserIds));
        }

        return result;
    }
}
