/*
 * 文件名称: MenuPermissionSeeder.cs
 * 功能描述: 菜单权限数据初始化器，负责初始化系统菜单权限数据
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;
using Domain.Entities;
using Database.Bases;

namespace Database.Seeders;

/// <summary>
/// 菜单权限数据初始化器
/// <para>负责初始化系统菜单权限数据</para>
/// </summary>
/// <param name="client">SqlSugar 客户端</param>
public class MenuPermissionSeeder(ISqlSugarClient client) : Seeder<MenuPermission>(client) {

    /// <summary>
    /// 初始化菜单权限数据
    /// </summary>
    /// <returns>异步任务</returns>
    /// <exception cref="Exception">当初始化失败时抛出</exception>
    public override async Task SeedAsync() {
        try {
            if (!await IsTableEmptyAsync()) {
                LogInfo("菜单权限数据已存在，跳过初始化");
                return;
            }

            LogInfo("正在初始化菜单权限...");
            var menuPermissions = new List<MenuPermission>
            {
                new() {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    Code = "menu:dashboard",
                    Name = "仪表盘",
                    Path = "/admin/dashboard",
                    Icon = "dashboard",
                    Component = "Dashboard",
                    Sort = 1
                },
                new() {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    Code = "menu:system",
                    Name = "系统管理",
                    Path = "",
                    Icon = "setting",
                    Component = "",
                    Sort = 2,
                    IsExternal = false,
                    KeepAlive = false,
                    IsVisible = true,
                    Redirect = "/admin/users"
                },
                new() {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    Code = "menu:users",
                    Name = "用户管理",
                    Path = "/admin/users",
                    Icon = "user",
                    Component = "@/admin/users/index",
                    ParentId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    Sort = 1
                },
                new() {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    Code = "menu:roles",
                    Name = "角色管理",
                    Path = "/admin/roles",
                    Icon = "peoples",
                    Component = "@/admin/roles/index",
                    ParentId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    Sort = 2
                },
                new() {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                    Code = "menu:permissions",
                    Name = "权限管理",
                    Path = "/admin/permissions",
                    Icon = "lock",
                    Component = "@/admin/menu-permissions/index",
                    ParentId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    Sort = 3
                }
            };

            await _client.Insertable(menuPermissions.ToArray()).ExecuteCommandAsync();
            LogInfo("菜单权限初始化完成，共 {0} 个菜单", menuPermissions.Count);
        }
        catch (Exception ex) {
            LogException(ex, "初始化菜单权限失败");
            throw;
        }
    }

    /// <summary>
    /// 获取所有菜单权限 ID
    /// </summary>
    /// <returns>权限 ID 列表</returns>
    public async Task<List<Guid>> GetAllPermissionIdsAsync() {
        return await _client.Queryable<MenuPermission>().Select(mp => mp.Id).ToListAsync();
    }
}
