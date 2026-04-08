/*
 * 文件名称: DepartmentPermissionService.cs
 * 功能描述: 部门权限服务实现，提供部门权限相关的操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Infrastructure.Services;

/// <inheritdoc cref="IDepartmentPermissionService"/>
/// <remarks>
/// <para>部门权限服务负责：</para>
/// <list type="bullet">
///   <item> 检查用户在部门中的角色 </item>
///   <item> 检查用户是否有管理部门的权限 </item>
///   <item> 检查用户是否有管理下级部门的权限 </item>
///   <item> 获取用户可管理的部门列表 </item>
///   <item> 检查用户是否是企业超级管理员 </item>
/// </list>
/// <para>实现策略：</para>
/// <list type="bullet">
///   <item> 企业超级管理员拥有所有部门的管理权限 </item>
///   <item> 部门超级管理员拥有本部门及所有下级部门的管理权限 </item>
///   <item> 部门普通管理员拥有本部门及直接下级部门的管理权限 </item>
///   <item> 普通用户只能访问本部门信息 </item>
/// </list>
/// </remarks>
/// <param name="db">SqlSugar 客户端，用于数据库操作</param>
/// <param name="departmentRepository">部门仓储</param>
/// <param name="roleRepository">角色仓储</param>
/// <param name="logger">日志记录器</param>
public class DepartmentPermissionService(ISqlSugarClient db, IDepartmentRepository departmentRepository, IRoleRepository roleRepository, ILogger<DepartmentPermissionService> logger) : IDepartmentPermissionService {
    private readonly ISqlSugarClient _db = db;
    private readonly IDepartmentRepository _departmentRepository = departmentRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly ILogger<DepartmentPermissionService> _logger = logger;

    /// <inheritdoc/>
    public async Task<Role?> GetUserRoleInDepartmentAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default) {
        try {
            var userDepartmentRole = await _db.Queryable<UserDepartmentRole>()
                .Where(udr => udr.UserId == userId && udr.DepartmentId == departmentId)
                .FirstAsync(cancellationToken);

            if (userDepartmentRole == null) {
                return null;
            }

            return await _roleRepository.GetAsync(userDepartmentRole.RoleId, cancellationToken);
        } catch (Exception ex) {
            _logger.LogError(ex, "获取用户在部门中的角色失败：UserId: {UserId}, DepartmentId: {DepartmentId}", userId, departmentId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsDepartmentAdminAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default) {
        try {
            var role = await GetUserRoleInDepartmentAsync(userId, departmentId, cancellationToken);
            if (role == null) {
                return false;
            }

            // 假设角色编码以 "admin" 或 "super" 开头的为管理员角色
            return role.Code.StartsWith("admin") || role.Code.StartsWith("super");
        } catch (Exception ex) {
            _logger.LogError(ex, "检查用户是否是部门管理员失败：UserId: {UserId}, DepartmentId: {DepartmentId}", userId, departmentId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsDepartmentSuperAdminAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default) {
        try {
            var role = await GetUserRoleInDepartmentAsync(userId, departmentId, cancellationToken);
            if (role == null) {
                return false;
            }

            // 假设角色编码以 "super" 开头的为超级管理员角色
            return role.Code.StartsWith("super");
        } catch (Exception ex) {
            _logger.LogError(ex, "检查用户是否是部门超级管理员失败：UserId: {UserId}, DepartmentId: {DepartmentId}", userId, departmentId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> HasDepartmentPermissionAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default) {
        try {
            // 检查是否是企业超级管理员
            if (await IsEnterpriseSuperAdminAsync(userId, cancellationToken)) {
                return true;
            }

            // 检查是否是部门管理员
            return await IsDepartmentAdminAsync(userId, departmentId, cancellationToken);
        } catch (Exception ex) {
            _logger.LogError(ex, "检查用户是否有管理部门的权限失败：UserId: {UserId}, DepartmentId: {DepartmentId}", userId, departmentId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> HasSubDepartmentPermissionAsync(Guid userId, Guid departmentId, Guid targetDepartmentId, CancellationToken cancellationToken = default) {
        try {
            // 检查是否是企业超级管理员
            if (await IsEnterpriseSuperAdminAsync(userId, cancellationToken)) {
                return true;
            }

            // 检查是否是部门超级管理员
            if (await IsDepartmentSuperAdminAsync(userId, departmentId, cancellationToken)) {
                // 检查目标部门是否是当前部门或其子部门
                var targetDepartment = await _departmentRepository.GetSubTreeAsync(targetDepartmentId, cancellationToken);
                if (targetDepartment == null) {
                    return false;
                }

                // 检查目标部门的继承链是否包含当前部门
                var inheritanceChain = await _departmentRepository.GetInheritanceChainAsync(targetDepartmentId, cancellationToken);
                return inheritanceChain.Any(d => d.Id == departmentId);
            }

            // 检查是否是部门普通管理员
            if (await IsDepartmentAdminAsync(userId, departmentId, cancellationToken)) {
                // 检查目标部门是否是当前部门或其直接下级部门
                var directChildren = await _departmentRepository.GetDirectChildrenAsync(departmentId, cancellationToken);
                var directChildIds = directChildren.Select(d => d.Id).ToList();
                return targetDepartmentId == departmentId || directChildIds.Contains(targetDepartmentId);
            }

            return false;
        } catch (Exception ex) {
            _logger.LogError(ex, "检查用户是否有管理下级部门的权限失败：UserId: {UserId}, DepartmentId: {DepartmentId}, TargetDepartmentId: {TargetDepartmentId}", userId, departmentId, targetDepartmentId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<List<Department>> GetManageableDepartmentsAsync(Guid userId, CancellationToken cancellationToken = default) {
        try {
            var manageableDepartments = new List<Department>();

            // 检查是否是企业超级管理员
            if (await IsEnterpriseSuperAdminAsync(userId, cancellationToken)) {
                // 企业超级管理员可以管理所有部门
                var allDepartments = await _departmentRepository.GetTreeAsync(cancellationToken);
                return allDepartments;
            }

            // 获取用户在所有部门中的角色
            var userDepartmentRoles = await _db.Queryable<UserDepartmentRole>()
                .Where(udr => udr.UserId == userId)
                .ToListAsync(cancellationToken);

            foreach (var udr in userDepartmentRoles) {
                var department = await _departmentRepository.GetAsync(udr.DepartmentId, cancellationToken);
                if (department == null) {
                    continue;
                }

                var role = await _roleRepository.GetAsync(udr.RoleId, cancellationToken);
                if (role == null) {
                    continue;
                }

                if (role.Code.StartsWith("super")) {
                    // 超级管理员可以管理本部门及所有下级部门
                    var subTree = await _departmentRepository.GetSubTreeAsync(department.Id, cancellationToken);
                    if (subTree != null) {
                        manageableDepartments.Add(subTree);
                    }
                } else if (role.Code.StartsWith("admin")) {
                    // 普通管理员可以管理本部门及直接下级部门
                    manageableDepartments.Add(department);
                    var directChildren = await _departmentRepository.GetDirectChildrenAsync(department.Id, cancellationToken);
                    manageableDepartments.AddRange(directChildren);
                }
            }

            return manageableDepartments.DistinctBy(d => d.Id).ToList();
        } catch (Exception ex) {
            _logger.LogError(ex, "获取用户可管理的部门列表失败：UserId: {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsEnterpriseSuperAdminAsync(Guid userId, CancellationToken cancellationToken = default) {
        try {
            // 检查用户是否拥有系统超级管理员角色
            var userRoles = await _db.Queryable<UserRole>()
                .Where(ur => ur.UserId == userId)
                .ToListAsync(cancellationToken);

            foreach (var userRole in userRoles) {
                var role = await _roleRepository.GetAsync(userRole.RoleId, cancellationToken);
                if (role != null && role.Code == "super_admin") {
                    return true;
                }
            }

            return false;
        } catch (Exception ex) {
            _logger.LogError(ex, "检查用户是否是企业超级管理员失败：UserId: {UserId}", userId);
            throw;
        }
    }
}