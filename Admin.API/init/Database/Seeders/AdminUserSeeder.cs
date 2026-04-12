/*
 * 文件名称: AdminUserSeeder.cs
 * 功能描述: 管理员用户数据初始化器，负责创建系统管理员账号
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;
using Domain.Entities;
using Domain.Shared.Constants;
using Database.Bases;
using Domain.Shared.Utils;

namespace Database.Seeders;

/// <summary>
/// 管理员用户数据初始化器
/// <para>负责创建系统管理员账号</para>
/// </summary>
/// <param name="client">SqlSugar 客户端</param>
public class AdminUserSeeder(ISqlSugarClient client) : Seeder<User>(client) {

    /// <summary>
    /// 初始化管理员用户数据
    /// </summary>
    /// <returns>异步任务</returns>
    /// <exception cref="Exception">当初始化失败时抛出</exception>
    public override async Task SeedAsync() {
        try {
            if (!await IsTableEmptyAsync()) {
                LogInfo("用户数据已存在，跳过初始化");
                return;
            }

            LogInfo("正在初始化管理员用户...");
            var adminUser = new User {
                Id = UserConstants.Administrator.Id,
                Email = UserConstants.Administrator.Email,
                PasswordHash = PasswordUtil.HashPassword(UserConstants.Administrator.Password),
                NickName = UserConstants.Administrator.NickName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _client.Insertable(adminUser).ExecuteCommandAsync();

            LogInfo("管理员用户创建完成，账号：{0}，密码：{1}", UserConstants.Administrator.Email, UserConstants.Administrator.Password);
        }
        catch (Exception ex) {
            LogException(ex, "初始化管理员用户失败");
            throw;
        }
    }

    /// <summary>
    /// 获取管理员用户 ID
    /// </summary>
    /// <returns>管理员用户 ID</returns>
    public Guid? GetAdminUserId() {
        return UserConstants.Administrator.Id;
    }
}
