/*
 * 文件名称: UserRoleSeeder.cs
 * 功能描述: 用户角色关系数据初始化器，负责初始化用户与角色的关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;
using Domain.Entities;
using Database.Bases;

namespace Database.Seeders;

/// <summary>
/// 用户角色关系数据初始化器
/// <para>负责初始化用户与角色的关联关系</para>
/// </summary>
/// <param name="client">SqlSugar 客户端</param>
public class UserRoleSeeder(ISqlSugarClient client) : Seeder<UserRole>(client) {

    /// <summary>
    /// 初始化用户角色关系数据
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="roleCode">角色代码</param>
    /// <returns>异步任务</returns>
    /// <exception cref="Exception">当初始化失败时抛出</exception>
    public async Task SeedAsync(Guid userId, string roleCode) {
        try {
            LogInfo("正在初始化用户角色关系...");

            var role = await _client.Queryable<Role>().FirstAsync(r => r.Code == roleCode);

            if (role != null) {
                var exists = await _client.Queryable<UserRole>()
                    .AnyAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);

                if (!exists) {
                    var userRole = new UserRole {
                        UserId = userId,
                        RoleId = role.Id
                    };

                    _ = await _client.Insertable(userRole).ExecuteCommandAsync();
                    LogInfo("用户角色关系创建完成");
                }
                else {
                    LogInfo("用户角色关系已存在，跳过初始化");
                }
            }
            else {
                LogInfo("未找到角色：{0}", roleCode);
            }
        }
        catch (Exception ex) {
            LogException(ex, "初始化用户角色关系失败");
            throw;
        }
    }

    /// <summary>
    /// 初始化用户角色关系数据（重写基类方法）
    /// </summary>
    /// <returns>异步任务</returns>
    /// <exception cref="NotImplementedException">始终抛出</exception>
    public override Task SeedAsync() {
        throw new NotImplementedException();
    }
}
