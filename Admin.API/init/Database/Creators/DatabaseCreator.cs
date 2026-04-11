/*
 * 文件名称: DatabaseCreator.cs
 * 功能描述: 数据库创建器，负责创建数据库实例
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using Database.Bases;
using SqlSugar;

namespace Database.Creators;

/// <summary>
/// 数据库创建器
/// <para>负责创建数据库实例</para>
/// </summary>
/// <param name="client">SqlSugar 客户端</param>
public class DatabaseCreator(ISqlSugarClient client) : Creator(client) {

    /// <summary>
    /// 创建数据库
    /// </summary>
    /// <exception cref="Exception">当创建失败时抛出</exception>
    public override void Create() {
        try {
            LogInfo("正在创建数据库...");
            _client.DbMaintenance.CreateDatabase();
            LogInfo("数据库创建完成");
        }
        catch (Exception ex) {
            LogException(ex, "创建数据库失败");
            throw;
        }
    }
}
