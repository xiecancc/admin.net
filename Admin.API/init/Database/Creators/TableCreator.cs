/*
 * 文件名称: TableCreator.cs
 * 功能描述: 数据表创建器，负责根据实体类型创建数据表
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;
using Domain.Entities;
using Database.Bases;

namespace Database.Creators;

/// <summary>
/// 数据表创建器
/// <para>负责根据实体类型创建数据表</para>
/// </summary>
/// <param name="client">SqlSugar 客户端</param>
public class TableCreator(ISqlSugarClient client) : Creator(client) {

    /// <summary>
    /// 创建数据表
    /// </summary>
    /// <exception cref="Exception">当创建失败时抛出</exception>
    public override void Create() {
        try {
            LogInfo("正在创建数据表...");
            
            // 配置 SqlSugar，为 MySQL 数据库设置 GUID 类型映射
            _client.CodeFirst
                .SetStringDefaultLength(200)
                .InitTables(
                    typeof(User),
                    typeof(Role),
                    typeof(Permission),
                    typeof(ApiPermission),
                    typeof(MenuPermission),
                    typeof(ButtonPermission),
                    typeof(UserRole),
                    typeof(RolePermission),
                    typeof(Department),
                    typeof(UserDepartmentRole)
                );
            LogInfo("数据表创建完成");
        }
        catch (Exception ex) {
            LogException(ex, "创建数据表失败");
            throw;
        }
    }
}
