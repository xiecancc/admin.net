/*
 * 文件名称: RoleSeeder.cs
 * 功能描述: 角色数据初始化器，负责初始化系统默认角色
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;
using Domain.Entities;
using Domain.Shared.Constants;
using Database.Bases;

namespace Database.Seeders;

/// <summary>
/// 角色数据初始化器
/// <para>负责初始化系统默认角色</para>
/// </summary>
/// <param name="client">SqlSugar 客户端</param>
public class RoleSeeder(ISqlSugarClient client) : Seeder<Role>(client) {

    /// <summary>
    /// 初始化角色数据
    /// </summary>
    /// <returns>异步任务</returns>
    /// <exception cref="Exception">当初始化失败时抛出</exception>
    public override async Task SeedAsync() {
        try {
            if (!await IsTableEmptyAsync()) {
                LogInfo("角色数据已存在，跳过初始化");
                return;
            }

            LogInfo("正在初始化角色数据...");
            var roles = new List<Role>
            {
                new() {
                    Id = RoleConstants.Administrator.Id,
                    Code = RoleConstants.Administrator.Code,
                    Name = RoleConstants.Administrator.Name
                },
                new() {
                    Id = RoleConstants.Manager.Id,
                    Code = RoleConstants.Manager.Code,
                    Name = RoleConstants.Manager.Name
                },
                new() {
                    Id = RoleConstants.User.Id,
                    Code = RoleConstants.User.Code,
                    Name = RoleConstants.User.Name
                }
            };

            await _client.Insertable(roles.ToArray()).ExecuteCommandAsync();
            LogInfo("角色数据初始化完成，共 {0} 个角色", roles.Count);
        }
        catch (Exception ex) {
            LogException(ex, "初始化角色数据失败");
            throw;
        }
    }

    /// <summary>
    /// 根据角色代码获取角色
    /// </summary>
    /// <param name="code">角色代码</param>
    /// <returns>角色实体，如果不存在则返回 null</returns>
    public async Task<Role?> GetRoleByCodeAsync(string code) {
        return await _client.Queryable<Role>().FirstAsync(r => r.Code == code);
    }
}
