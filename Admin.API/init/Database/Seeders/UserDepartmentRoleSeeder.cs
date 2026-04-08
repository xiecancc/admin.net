/*
 * 文件名称: UserDepartmentRoleSeeder.cs
 * 功能描述: 用户部门角色关联数据初始化器，负责初始化系统默认的用户部门角色关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using SqlSugar;
using Domain.Entities;
using Domain.Shared.Constants;
using Database.Bases;

namespace Database.Seeders;

/// <summary>
/// 用户部门角色关联数据初始化器
/// <para>负责初始化系统默认的用户部门角色关联关系</para>
/// </summary>
/// <param name="client">SqlSugar 客户端</param>
/// <param name="userSeeder">用户种子数据</param>
/// <param name="departmentSeeder">部门种子数据</param>
/// <param name="roleSeeder">角色种子数据</param>
public class UserDepartmentRoleSeeder(ISqlSugarClient client, AdminUserSeeder userSeeder, DepartmentSeeder departmentSeeder, RoleSeeder roleSeeder) : Seeder<UserDepartmentRole>(client) {
    private readonly AdminUserSeeder _userSeeder = userSeeder;
    private readonly DepartmentSeeder _departmentSeeder = departmentSeeder;
    private readonly RoleSeeder _roleSeeder = roleSeeder;

    /// <summary>
    /// 初始化用户部门角色关联数据
    /// </summary>
    /// <returns>异步任务</returns>
    /// <exception cref="Exception">当初始化失败时抛出</exception>
    public override async Task SeedAsync() {
        try {
            if (!await IsTableEmptyAsync()) {
                LogInfo("用户部门角色关联数据已存在，跳过初始化");
                return;
            }

            LogInfo("正在初始化用户部门角色关联数据...");

            // 获取默认用户
            var adminUserId = _userSeeder.GetAdminUserId();
            if (!adminUserId.HasValue) {
                LogInfo("未找到管理员用户，跳过用户部门角色关联数据初始化");
                return;
            }
            
            var adminUser = await _client.Queryable<User>().FirstAsync(u => u.Id == adminUserId.Value);
            if (adminUser == null) {
                LogInfo("未找到管理员用户，跳过用户部门角色关联数据初始化");
                return;
            }

            // 获取默认部门
            var companyDept = await _departmentSeeder.GetDepartmentByCodeAsync("COMPANY");
            var hrDept = await _departmentSeeder.GetDepartmentByCodeAsync("HR");
            var itDept = await _departmentSeeder.GetDepartmentByCodeAsync("IT");
            var financeDept = await _departmentSeeder.GetDepartmentByCodeAsync("FINANCE");
            var marketingDept = await _departmentSeeder.GetDepartmentByCodeAsync("MARKETING");

            // 获取默认角色
            var adminRole = await _roleSeeder.GetRoleByCodeAsync(RoleConstants.Administrator.CODE);
            var managerRole = await _roleSeeder.GetRoleByCodeAsync(RoleConstants.Manager.CODE);
            var userRole = await _roleSeeder.GetRoleByCodeAsync(RoleConstants.User.CODE);

            if (companyDept == null || adminRole == null) {
                LogInfo("缺少必要的部门或角色数据，跳过用户部门角色关联数据初始化");
                return;
            }

            var userDepartmentRoles = new List<UserDepartmentRole>();

            // 为管理员用户分配总公司超级管理员角色
            userDepartmentRoles.Add(new() {
                UserId = adminUser.Id,
                DepartmentId = companyDept.Id,
                RoleId = adminRole.Id
            });

            // 为其他部门分配默认角色（如果存在）
            if (hrDept != null && managerRole != null) {
                userDepartmentRoles.Add(new() {
                    UserId = adminUser.Id,
                    DepartmentId = hrDept.Id,
                    RoleId = managerRole.Id
                });
            }

            if (itDept != null && managerRole != null) {
                userDepartmentRoles.Add(new() {
                    UserId = adminUser.Id,
                    DepartmentId = itDept.Id,
                    RoleId = managerRole.Id
                });
            }

            if (financeDept != null && managerRole != null) {
                userDepartmentRoles.Add(new() {
                    UserId = adminUser.Id,
                    DepartmentId = financeDept.Id,
                    RoleId = managerRole.Id
                });
            }

            if (marketingDept != null && managerRole != null) {
                userDepartmentRoles.Add(new() {
                    UserId = adminUser.Id,
                    DepartmentId = marketingDept.Id,
                    RoleId = managerRole.Id
                });
            }

            if (userDepartmentRoles.Count > 0) {
                _ = await _client.Insertable(userDepartmentRoles.ToArray()).ExecuteCommandAsync();
                LogInfo("用户部门角色关联数据初始化完成，共 {0} 条关联", userDepartmentRoles.Count);
            } else {
                LogInfo("未创建用户部门角色关联数据");
            }
        }
        catch (Exception ex) {
            LogException(ex, "初始化用户部门角色关联数据失败");
            throw;
        }
    }
}