/*
 * 文件名称: IRolePermissionRepository.cs
 * 功能描述: 角色权限关联仓储接口，用于处理角色和权限之间的关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Entities;
using Domain.Shared.Repositories;

namespace Domain.Repositories;

/// <summary>
/// 角色权限关联仓储接口
/// <para>用于处理角色和权限之间的多对多关联关系</para>
/// </summary>
/// <remarks>
/// <para>职责：</para>
/// <list type="bullet">
///   <item>管理角色权限关联关系（分配、移除）</item>
///   <item>查询角色权限关联 ID 列表</item>
///   <item>检查角色是否拥有指定权限</item>
/// </list>
/// <para>不包含的职责（已迁移到领域服务）：</para>
/// <list type="bullet">
///   <item>获取角色的完整权限实体列表 → IPermissionDomainService.GetRolePermissionsAsync</item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // 在应用服务中使用
/// public class RolePermissionAppService
/// {
///     private readonly IRolePermissionRepository _rolePermissionRepository;
///     
///     public RolePermissionAppService(IRolePermissionRepository rolePermissionRepository)
///     {
///         _rolePermissionRepository = rolePermissionRepository;
///     }
///     
///     public async Task AssignPermissionsToRoleAsync(Guid roleId, List&lt;Guid&gt; permissionIds)
///     {
///         await _rolePermissionRepository.AssignPermissionsToRoleAsync(roleId, permissionIds);
///     }
///     
///     public async Task&lt;List&lt;Guid&gt;&gt; GetRolePermissionIdsAsync(Guid roleId)
///     {
///         return await _rolePermissionRepository.GetPermissionIdsByRoleIdAsync(roleId);
///     }
/// }
/// </code>
/// </example>
public interface IRolePermissionRepository : IDomainRepository<RolePermission> {
    /// <summary>
    /// 根据角色 ID 获取权限 ID 列表
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限 ID 列表</returns>
    Task<List<Guid>> GetPermissionIdsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据权限 ID 获取角色 ID 列表
    /// </summary>
    /// <param name="permissionId">权限 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色 ID 列表</returns>
    Task<List<Guid>> GetRoleIdsByPermissionIdAsync(Guid permissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 为角色分配权限
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="permissionIds">权限 ID 列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 为权限分配角色
    /// </summary>
    /// <param name="permissionId">权限 ID</param>
    /// <param name="roleIds">角色 ID 列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> AssignRolesToPermissionAsync(Guid permissionId, List<Guid> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除角色的权限
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="permissionIds">权限 ID 列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> RemovePermissionsFromRoleAsync(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除权限的角色
    /// </summary>
    /// <param name="permissionId">权限 ID</param>
    /// <param name="roleIds">角色 ID 列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> RemoveRolesFromPermissionAsync(Guid permissionId, List<Guid> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查角色是否拥有指定权限
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="permissionId">权限 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否拥有</returns>
    Task<bool> RoleHasPermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);
}
