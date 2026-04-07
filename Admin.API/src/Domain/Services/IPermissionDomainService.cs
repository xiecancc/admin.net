// ==================================================================================================
// FileName: IPermissionDomainService.cs
// 功能描述: 权限领域服务接口，处理跨聚合的权限相关业务逻辑
// 作    者: Admin.NET
// 最近修订: 2026-04-05
// ==================================================================================================

using Domain.Shared.Enums;

namespace Domain.Services;

/// <summary>
/// 权限领域服务接口
/// <para>处理跨聚合的权限相关业务逻辑，如用户权限检查、权限查询等</para>
/// </summary>
/// <remarks>
/// <para>职责：</para>
/// <list type="bullet">
///   <item>检查用户是否拥有指定权限（跨聚合：User → UserRole → RolePermission → Permission）</item>
///   <item>获取用户的所有权限（跨聚合查询）</item>
///   <item>获取角色的所有权限（跨聚合查询）</item>
/// </list>
/// <para>不包含的职责（由仓储负责）：</para>
/// <list type="bullet">
///   <item>简单的角色权限分配/移除 → IRolePermissionRepository</item>
///   <item>简单的用户角色分配/移除 → IUserRoleRepository</item>
///   <item>单表权限查询 → IPermissionRepository</item>
/// </list>
/// </remarks>
public interface IPermissionDomainService : IDomainService {
    /// <summary>
    /// 检查用户是否拥有指定权限
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="permissionCode">权限编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否拥有权限</returns>
    /// <remarks>
    /// 跨聚合查询：User → UserRole → RolePermission → Permission
    /// </remarks>
    Task<bool> UserHasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查用户是否拥有指定权限
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="permissionId">权限 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否拥有权限</returns>
    /// <remarks>
    /// 跨聚合查询：User → UserRole → RolePermission
    /// </remarks>
    Task<bool> UserHasPermissionAsync(Guid userId, Guid permissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户的所有权限编码
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限编码列表</returns>
    /// <remarks>
    /// 跨聚合查询：User → UserRole → RolePermission → Permission
    /// </remarks>
    Task<List<string>> GetUserPermissionCodesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户的所有权限 ID
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限 ID 列表</returns>
    /// <remarks>
    /// 跨聚合查询：User → UserRole → RolePermission
    /// </remarks>
    Task<List<Guid>> GetUserPermissionIdsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取角色的所有权限编码
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限编码列表</returns>
    /// <remarks>
    /// 跨聚合查询：Role → RolePermission → Permission
    /// </remarks>
    Task<List<string>> GetRolePermissionCodesAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定类型的权限编码列表
    /// </summary>
    /// <param name="permissionType">权限类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限编码列表</returns>
    /// <remarks>
    /// 单表查询，但属于权限相关的业务查询，放在领域服务中便于统一管理
    /// </remarks>
    Task<List<string>> GetPermissionCodesByTypeAsync(PermissionType permissionType, CancellationToken cancellationToken = default);
}
