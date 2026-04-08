/*
 * 文件名称: IDepartmentPermissionService.cs
 * 功能描述: 部门权限服务接口，定义部门权限相关的操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Domain.Entities;

namespace Domain.Services;

/// <summary>
/// 部门权限服务接口
/// <para>定义部门权限相关的操作</para>
/// </summary>
/// <remarks>
/// <para>主要功能：</para>
/// <list type="bullet">
///   <item>检查用户在部门中的角色</item>
///   <item>检查用户是否有管理部门的权限</item>
///   <item>检查用户是否有管理下级部门的权限</item>
///   <item>获取用户可管理的部门列表</item>
/// </list>
/// </remarks>
public interface IDepartmentPermissionService {
    /// <summary>
    /// 获取用户在部门中的角色
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="departmentId">部门 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色实体，如果用户在该部门没有角色则返回 null</returns>
    Task<Role?> GetUserRoleInDepartmentAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查用户是否是部门管理员（超级管理员或普通管理员）
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="departmentId">部门 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否是部门管理员</returns>
    Task<bool> IsDepartmentAdminAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查用户是否是部门超级管理员
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="departmentId">部门 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否是部门超级管理员</returns>
    Task<bool> IsDepartmentSuperAdminAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查用户是否有管理部门的权限
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="departmentId">部门 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否有管理权限</returns>
    Task<bool> HasDepartmentPermissionAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查用户是否有管理下级部门的权限
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="departmentId">部门 ID</param>
    /// <param name="targetDepartmentId">目标部门 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否有管理权限</returns>
    Task<bool> HasSubDepartmentPermissionAsync(Guid userId, Guid departmentId, Guid targetDepartmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户可管理的部门列表
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>可管理的部门列表</returns>
    Task<List<Department>> GetManageableDepartmentsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查用户是否是企业超级管理员
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否是企业超级管理员</returns>
    Task<bool> IsEnterpriseSuperAdminAsync(Guid userId, CancellationToken cancellationToken = default);
}