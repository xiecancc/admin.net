/*
 * 文件名称: RolePermissionSeeder.cs
 * 功能描述: 角色权限关系数据初始化器，负责初始化角色与权限的关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;
using Domain.Entities;
using Database.Bases;

namespace Database.Seeders;

/// <summary>
/// 角色权限关系数据初始化器
/// <para>负责初始化角色与权限的关联关系</para>
/// </summary>
/// <param name="client">SqlSugar 客户端</param>
public class RolePermissionSeeder(ISqlSugarClient client) : Seeder<RolePermission>(client) {

    /// <summary>
    /// 初始化角色权限关系数据
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="permissionIds">权限 ID 列表</param>
    /// <returns>异步任务</returns>
    /// <exception cref="Exception">当初始化失败时抛出</exception>
    public async Task SeedAsync(Guid roleId, List<Guid> permissionIds) {
        try {
            LogInfo("正在初始化角色权限关系...");

            if (permissionIds.Count == 0) {
                LogInfo("权限列表为空，跳过初始化");
                return;
            }

            // 先删除已存在的角色权限关系，避免重复键错误
            var deleteCount = _client.Deleteable<RolePermission>()
                .Where(rp => rp.RoleId == roleId)
                .ExecuteCommand();
            LogInfo("删除了 {0} 条角色权限关系", deleteCount);

            // 对权限 ID 列表进行去重，避免重复键错误
            var newPermissionIds = permissionIds.Distinct().ToList();

            if (newPermissionIds.Count > 0) {
                var rolePermissions = newPermissionIds.Select(permissionId => new RolePermission {
                    RoleId = roleId,
                    PermissionId = permissionId
                }).ToArray();

                // 使用 InsertOrUpdate 方法避免重复键错误
                _ = await _client.Insertable(rolePermissions)
                    .ExecuteCommandAsync();
                LogInfo("角色权限关系创建完成，共 {0} 个权限", newPermissionIds.Count);
            }
            else {
                LogInfo("角色权限关系已存在，跳过初始化");
            }
        }
        catch (Exception ex) {
            LogException(ex, "初始化角色权限关系失败");
            throw;
        }
    }

    /// <summary>
    /// 初始化角色权限关系数据（重写基类方法）
    /// </summary>
    /// <returns>异步任务</returns>
    /// <exception cref="NotImplementedException">始终抛出</exception>
    public override Task SeedAsync() {
        throw new NotImplementedException();
    }
}
