/*
 * 文件名称: PermissionCacheWarmupService.cs
 * 功能描述: 权限缓存预热服务，在应用启动时预热权限缓存
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Enums;
using Infrastructure.Shared.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// 权限缓存预热服务
/// <para>在应用启动时预热权限缓存，提高首次访问性能</para>
/// </summary>
/// <param name="permissionCacheService">权限缓存服务</param>
/// <param name="userRepository">用户仓储</param>
/// <param name="roleRepository">角色仓储</param>
/// <param name="logger">日志记录器</param>
public class PermissionCacheWarmupService(
    IPermissionCacheService permissionCacheService,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    ILogger<PermissionCacheWarmupService> logger) : IHostedService {
    private readonly IPermissionCacheService _permissionCacheService = permissionCacheService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly ILogger<PermissionCacheWarmupService> _logger = logger;

    /// <summary>
    /// 启动时执行缓存预热
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
#pragma warning disable CA1031
    public async Task StartAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("开始权限缓存预热...");

        try {
            await WarmupRoleInheritedPermissionsAsync(cancellationToken);

            await WarmupActiveUserPermissionsAsync(cancellationToken);

            _logger.LogInformation("权限缓存预热完成");
        }
        catch (OperationCanceledException) {
            _logger.LogInformation("权限缓存预热被取消");
            throw;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "权限缓存预热失败，将在首次访问时按需加载");
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// 停止时无需处理
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    private async Task WarmupRoleInheritedPermissionsAsync(CancellationToken cancellationToken) {
        var roles = await _roleRepository.GetListAsync(predicate: null, cancellationToken: cancellationToken);
        if (roles.Count == 0) {
            _logger.LogDebug("没有角色需要预热");
            return;
        }

        var inheritedRoles = roles.Where(r => r.InheritanceType != InheritanceType.None).ToList();
        if (inheritedRoles.Count == 0) {
            _logger.LogDebug("没有需要继承权限的角色");
            return;
        }

        var successCount = await _permissionCacheService.WarmupRoleInheritedPermissionsBatchAsync(
            inheritedRoles.Select(r => r.Id), cancellationToken);

        _logger.LogInformation("角色继承权限缓存预热完成 | TotalCount: {TotalCount} | SuccessCount: {SuccessCount}",
            inheritedRoles.Count, successCount);
    }

    private async Task WarmupActiveUserPermissionsAsync(CancellationToken cancellationToken) {
        var activeUsers = await _userRepository.GetListAsync(
            predicate: u => !u.IsDeleted,
            cancellationToken: cancellationToken);

        if (activeUsers.Count == 0) {
            _logger.LogDebug("没有活跃用户需要预热");
            return;
        }

        const int batchSize = 50;
        var userIds = activeUsers.Select(u => u.Id).ToList();
        var totalBatches = (int)Math.Ceiling(userIds.Count / (double)batchSize);
        var totalSuccess = 0;

        for (var i = 0; i < totalBatches; i++) {
            var batchUserIds = userIds.Skip(i * batchSize).Take(batchSize);
            var successCount = await _permissionCacheService.WarmupUserPermissionsBatchAsync(batchUserIds, cancellationToken);
            totalSuccess += successCount;

            _logger.LogDebug("用户权限缓存预热批次 {CurrentBatch}/{TotalBatches} 完成 | SuccessCount: {SuccessCount}",
                i + 1, totalBatches, successCount);
        }

        _logger.LogInformation("用户权限缓存预热完成 | TotalCount: {TotalCount} | SuccessCount: {SuccessCount}",
            userIds.Count, totalSuccess);
    }
}
