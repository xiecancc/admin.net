/*
 * 文件名称: ButtonPermissionSeeder.cs
 * 功能描述: 按钮权限数据初始化器，负责初始化系统按钮权限数据
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;
using Domain.Entities;
using Database.Bases;

namespace Database.Seeders;

/// <summary>
/// 按钮权限数据初始化器
/// <para>负责初始化系统按钮权限数据</para>
/// </summary>
/// <param name="client">SqlSugar 客户端</param>
public class ButtonPermissionSeeder(ISqlSugarClient client) : Seeder<ButtonPermission>(client) {

    /// <summary>
    /// 初始化按钮权限数据
    /// </summary>
    /// <returns>异步任务</returns>
    /// <exception cref="Exception">当初始化失败时抛出</exception>
    public override async Task SeedAsync() {
        try {
            if (!await IsTableEmptyAsync()) {
                LogInfo("按钮权限数据已存在，跳过初始化");
                return;
            }

            LogInfo("正在初始化按钮权限...");
            var buttonPermissions = new List<ButtonPermission>
            {
                new() {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                    Code = "button:user:add",
                    Name = "添加用户",
                    ActionType = "add",
                    Sort = 1
                },
                new() {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                    Code = "button:user:edit",
                    Name = "编辑用户",
                    ActionType = "edit",
                    Sort = 2
                },
                new() {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                    Code = "button:user:delete",
                    Name = "删除用户",
                    ActionType = "delete",
                    Sort = 3
                }
            };

            _ = await _client.Insertable(buttonPermissions.ToArray()).ExecuteCommandAsync();
            LogInfo("按钮权限初始化完成，共 {0} 个按钮权限", buttonPermissions.Count);
        }
        catch (Exception ex) {
            LogException(ex, "初始化按钮权限失败");
            throw;
        }
    }

    /// <summary>
    /// 获取所有按钮权限 ID
    /// </summary>
    /// <returns>权限 ID 列表</returns>
    public async Task<List<Guid>> GetAllPermissionIdsAsync() {
        return await _client.Queryable<ButtonPermission>().Select(bp => bp.Id).ToListAsync();
    }
}
