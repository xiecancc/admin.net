/*
 * 文件名称: ApiPermissionSeeder.cs
 * 功能描述: API 权限数据初始化器，负责初始化系统 API 权限数据
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;
using Domain.Entities;
using Database.Bases;

namespace Database.Seeders;

/// <summary>
/// API 权限数据初始化器
/// <para>负责初始化系统 API 权限数据</para>
/// </summary>
/// <param name="client">SqlSugar 客户端</param>
public class ApiPermissionSeeder(ISqlSugarClient client) : Seeder<ApiPermission>(client) {

    /// <summary>
    /// 初始化 API 权限数据
    /// </summary>
    /// <returns>异步任务</returns>
    /// <exception cref="Exception">当初始化失败时抛出</exception>
    public override async Task SeedAsync() {
        try {
            if (!await IsTableEmptyAsync()) {
                LogInfo("API 权限数据已存在，跳过初始化");
                return;
            }

            LogInfo("正在初始化 API 权限...");
            var apiPermissions = new List<ApiPermission>();

            apiPermissions.AddRange([
                CreateApiPermission(0, "Users", "GET", "/api/users", "获取用户列表"),
                CreateApiPermission(1, "Users", "POST", "/api/users", "创建用户"),
                CreateApiPermission(2, "Users", "PUT", "/api/users/{id}", "更新用户"),
                CreateApiPermission(3, "Users", "DELETE", "/api/users/{id}", "删除用户"),
                CreateApiPermission(4, "Users", "GET", "/api/users/{id}", "获取用户详情"),
            ]);

            apiPermissions.AddRange([
                CreateApiPermission(10, "Roles", "GET", "/api/roles", "获取角色列表"),
                CreateApiPermission(11, "Roles", "POST", "/api/roles", "创建角色"),
                CreateApiPermission(12, "Roles", "PUT", "/api/roles/{id}", "更新角色"),
                CreateApiPermission(13, "Roles", "DELETE", "/api/roles/{id}", "删除角色"),
                CreateApiPermission(14, "Roles", "GET", "/api/roles/tree", "获取角色树"),
                CreateApiPermission(15, "Roles", "GET", "/api/roles/{id}/children", "获取子角色"),
                CreateApiPermission(16, "Roles", "GET", "/api/roles/{id}/permissions", "获取角色权限"),
                CreateApiPermission(17, "Roles", "POST", "/api/roles/{id}/permissions", "分配角色权限"),
            ]);

            apiPermissions.AddRange([
                CreateApiPermission(20, "MenuPermissions", "GET", "/api/menu-permissions", "获取菜单权限列表"),
                CreateApiPermission(21, "MenuPermissions", "POST", "/api/menu-permissions", "创建菜单权限"),
                CreateApiPermission(22, "MenuPermissions", "PUT", "/api/menu-permissions/{id}", "更新菜单权限"),
                CreateApiPermission(23, "MenuPermissions", "DELETE", "/api/menu-permissions/{id}", "删除菜单权限"),
                CreateApiPermission(24, "MenuPermissions", "GET", "/api/menu-permissions/tree", "获取菜单树"),
                CreateApiPermission(25, "MenuPermissions", "GET", "/api/menu-permissions/{id}/children", "获取子菜单"),
            ]);

            apiPermissions.AddRange([
                CreateApiPermission(30, "ButtonPermissions", "GET", "/api/button-permissions", "获取按钮权限列表"),
                CreateApiPermission(31, "ButtonPermissions", "POST", "/api/button-permissions", "创建按钮权限"),
                CreateApiPermission(32, "ButtonPermissions", "PUT", "/api/button-permissions/{id}", "更新按钮权限"),
                CreateApiPermission(33, "ButtonPermissions", "DELETE", "/api/button-permissions/{id}", "删除按钮权限"),
                CreateApiPermission(34, "ButtonPermissions", "GET", "/api/button-permissions/tree", "获取按钮权限树"),
                CreateApiPermission(35, "ButtonPermissions", "GET", "/api/button-permissions/{id}/children", "获取子按钮权限"),
            ]);

            apiPermissions.AddRange([
                CreateApiPermission(40, "ApiPermissions", "GET", "/api/api-permissions", "获取 API 权限列表"),
                CreateApiPermission(41, "ApiPermissions", "POST", "/api/api-permissions", "创建 API 权限"),
                CreateApiPermission(42, "ApiPermissions", "PUT", "/api/api-permissions/{id}", "更新 API 权限"),
                CreateApiPermission(43, "ApiPermissions", "DELETE", "/api/api-permissions/{id}", "删除 API 权限"),
                CreateApiPermission(44, "ApiPermissions", "GET", "/api/api-permissions/tree", "获取 API 权限树"),
                CreateApiPermission(45, "ApiPermissions", "GET", "/api/api-permissions/{id}/children", "获取子 API 权限"),
            ]);

            apiPermissions.AddRange([
                CreateApiPermission(50, "UserPermissions", "GET", "/api/user-permissions", "获取用户权限列表"),
                CreateApiPermission(51, "UserPermissions", "POST", "/api/user-permissions", "创建用户权限"),
                CreateApiPermission(52, "UserPermissions", "PUT", "/api/user-permissions/{id}", "更新用户权限"),
                CreateApiPermission(53, "UserPermissions", "DELETE", "/api/user-permissions/{id}", "删除用户权限"),
            ]);

            if (apiPermissions.Count > 0) {
                _ = await _client.Insertable(apiPermissions.ToArray()).ExecuteCommandAsync();
                LogInfo("API 权限初始化完成，共 {0} 个 API", apiPermissions.Count);
            }
        }
        catch (Exception ex) {
            LogException(ex, "初始化 API 权限失败");
            throw;
        }
    }

    /// <summary>
    /// 获取所有 API 权限 ID
    /// </summary>
    /// <returns>权限 ID 列表</returns>
    public async Task<List<Guid>> GetAllPermissionIdsAsync() {
        return await _client.Queryable<ApiPermission>().Select(ap => ap.Id).ToListAsync();
    }

    /// <summary>
    /// 创建 API 权限
    /// </summary>
    /// <param name="offset">排序偏移量</param>
    /// <param name="module">模块名称</param>
    /// <param name="httpMethod">HTTP 方法</param>
    /// <param name="apiPath">API 路径</param>
    /// <param name="name">权限名称</param>
    /// <returns>API 权限实体</returns>
    private static ApiPermission CreateApiPermission(int offset, string module, string httpMethod, string apiPath, string name) {
        return new ApiPermission {
            Id = Guid.NewGuid(),
            Code = $"{httpMethod}:{apiPath}",
            Name = name,
            HttpMethod = httpMethod,
            ApiPath = apiPath,
            ModuleName = module,
            Sort = offset
        };
    }
}
