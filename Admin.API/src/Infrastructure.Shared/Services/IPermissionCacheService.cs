/*
 * 文件名称: IPermissionCacheService.cs
 * 功能描述: 权限缓存服务接口,定义权限缓存相关操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-07
 */

namespace Infrastructure.Shared.Services;

/// <summary>
/// 权限缓存服务接口
/// <para>定义权限缓存相关操作</para>
/// </summary>
public interface IPermissionCacheService {
    /// <summary>
    /// 获取用户权限缓存
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限编码集合,不存在则返回null</returns>
    Task<HashSet<string>?> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置用户权限缓存
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permissions">权限编码集合</param>
    /// <param name="expiration">过期时间,默认30分钟</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> SetUserPermissionsAsync(Guid userId, HashSet<string> permissions, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除用户权限缓存
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> RemoveUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除所有用户权限缓存
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> RemoveAllUserPermissionsAsync(CancellationToken cancellationToken = default);
}
