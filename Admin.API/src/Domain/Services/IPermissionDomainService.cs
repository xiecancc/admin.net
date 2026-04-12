// ==================================================================================================
// FileName: IPermissionDomainService.cs
// 功能描述: 权限领域服务接口，处理跨聚合的权限相关业务逻辑
// 作    者: Admin.NET
// 最近修订: 2026-04-11
// ==================================================================================================

using Domain.Entities;
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
    /// 获取用户的所有权限编码（包含继承权限）
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限编码列表</returns>
    /// <remarks>
    /// <para>跨聚合查询：User → UserRole → RolePermission → Permission</para>
    /// <para>自动计算每个角色的继承权限，权限合并规则为并集</para>
    /// </remarks>
    Task<List<string>> GetUserAllPermissionCodesAsync(Guid userId, CancellationToken cancellationToken = default);

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
    /// 获取角色的所有权限实体
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限实体列表</returns>
    /// <remarks>
    /// <para>跨聚合查询：Role → RolePermission → Permission</para>
    /// <para>返回完整的 Permission 聚合对象，包含所有属性</para>
    /// </remarks>
    Task<List<Permission>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

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

    /// <summary>
    /// 获取角色的继承权限编码列表
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="inheritanceType">继承类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>继承的权限编码列表</returns>
    /// <remarks>
    /// <para>向上继承（Upward）：子角色继承父角色的权限</para>
    /// <para>向下继承（Downward）：父角色继承子角色的权限</para>
    /// <para>权限合并规则为并集</para>
    /// </remarks>
    Task<List<string>> GetRoleInheritedPermissionsAsync(Guid roleId, InheritanceType inheritanceType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取角色的所有权限编码列表（包括直接权限和继承权限）
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>所有权限编码列表</returns>
    /// <remarks>
    /// <para>根据角色的继承类型自动计算继承权限</para>
    /// <para>权限合并规则为并集</para>
    /// </remarks>
    Task<List<string>> GetRoleAllPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
}
