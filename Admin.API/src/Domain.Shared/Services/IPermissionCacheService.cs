/*
 * 文件名称: IPermissionCacheService.cs
 * 功能描述: 权限缓存服务接口,定义权限缓存相关操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

namespace Domain.Shared.Services;

/// <summary>
/// 权限缓存服务接口
/// <para>定义权限缓存相关操作</para>
/// </summary>
/// <remarks>
/// <para>主要功能：</para>
/// <list type="bullet">
///   <item>用户权限缓存的增删改查</item>
///   <item>角色继承权限缓存的增删改查</item>
///   <item>批量缓存操作</item>
///   <item>缓存预热</item>
/// </list>
/// <para>缓存失效策略：</para>
/// <list type="bullet">
///   <item>用户角色变更 → 清除用户权限缓存</item>
///   <item>角色权限变更 → 清除所有拥有该角色的用户权限缓存</item>
///   <item>权限本身变更 → 清除所有相关缓存</item>
///   <item>角色继承关系变更 → 清除继承权限缓存及相关用户缓存</item>
/// </list>
/// </remarks>
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
    /// 批量移除用户权限缓存
    /// </summary>
    /// <param name="userIds">用户ID集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>成功移除的数量</returns>
    Task<int> RemoveUserPermissionsBatchAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除所有用户权限缓存
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> RemoveAllUserPermissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取角色继承权限缓存
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限编码集合，不存在则返回null</returns>
    Task<HashSet<string>?> GetRoleInheritedPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置角色继承权限缓存
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="permissions">权限编码集合</param>
    /// <param name="expiration">过期时间，默认30分钟</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> SetRoleInheritedPermissionsAsync(Guid roleId, HashSet<string> permissions, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除角色继承权限缓存
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> RemoveRoleInheritedPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量移除角色继承权限缓存
    /// </summary>
    /// <param name="roleIds">角色ID集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>成功移除的数量</returns>
    Task<int> RemoveRoleInheritedPermissionsBatchAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除所有角色继承权限缓存
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> RemoveAllRoleInheritedPermissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 清除角色相关的所有缓存
    /// <para>包括：角色继承权限缓存、拥有该角色的所有用户权限缓存</para>
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> ClearRoleRelatedCacheAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清除角色继承关系变更相关的所有缓存
    /// </summary>
    /// <param name="roleId">变更的角色ID</param>
    /// <param name="oldParentId">原父角色ID</param>
    /// <param name="newParentId">新父角色ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// <para>当角色的父角色变更时，需要清除以下缓存：</para>
    /// <list type="bullet">
    ///   <item>当前角色的继承权限缓存</item>
    ///   <item>原父角色链上所有角色的继承权限缓存（向上继承场景）</item>
    ///   <item>新父角色链上所有角色的继承权限缓存（向上继承场景）</item>
    ///   <item>所有子角色的继承权限缓存（向下继承场景）</item>
    ///   <item>所有受影响角色的用户权限缓存</item>
    /// </list>
    /// </remarks>
    Task<bool> ClearRoleInheritanceChangeCacheAsync(Guid roleId, Guid? oldParentId, Guid? newParentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清除角色继承类型变更相关的所有缓存
    /// </summary>
    /// <param name="roleId">变更的角色ID</param>
    /// <param name="oldInheritanceType">原继承类型</param>
    /// <param name="newInheritanceType">新继承类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// <para>当角色的继承类型变更时，需要清除以下缓存：</para>
    /// <list type="bullet">
    ///   <item>当前角色的继承权限缓存</item>
    ///   <item>父角色链上所有角色的继承权限缓存（向上继承场景）</item>
    ///   <item>所有子角色的继承权限缓存（向下继承场景）</item>
    ///   <item>所有受影响角色的用户权限缓存</item>
    /// </list>
    /// </remarks>
    Task<bool> ClearRoleInheritanceTypeChangeCacheAsync(Guid roleId, Enums.InheritanceType oldInheritanceType, Enums.InheritanceType newInheritanceType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量清除角色相关的所有缓存
    /// </summary>
    /// <param name="roleIds">角色ID集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> ClearRoleRelatedCacheBatchAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清除权限相关的所有缓存
    /// <para>包括：所有角色继承权限缓存、所有用户权限缓存</para>
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> ClearAllPermissionCacheAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 预热用户权限缓存
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> WarmupUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量预热用户权限缓存
    /// </summary>
    /// <param name="userIds">用户ID集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>成功预热的数量</returns>
    Task<int> WarmupUserPermissionsBatchAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 预热角色继承权限缓存
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> WarmupRoleInheritedPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量预热角色继承权限缓存
    /// </summary>
    /// <param name="roleIds">角色ID集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>成功预热的数量</returns>
    Task<int> WarmupRoleInheritedPermissionsBatchAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);
}
