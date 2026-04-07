/*
 * 文件名称: Seeder.cs
 * 功能描述: 数据初始化器抽象基类，定义种子数据初始化的标准接口
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;

namespace Database.Bases;

/// <summary>
/// 数据初始化器抽象基类
/// <para>定义种子数据初始化的标准接口</para>
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
/// <remarks>
/// <para>继承此类的子类需要实现 SeedAsync 方法</para>
/// </remarks>
/// <param name="client">SqlSugar 客户端</param>
public abstract class Seeder<T>(ISqlSugarClient client) : Base(client) where T : class, new() {

    /// <summary>
    /// 初始化种子数据
    /// </summary>
    /// <returns>异步任务</returns>
    public abstract Task SeedAsync();

    /// <summary>
    /// 检查表是否为空
    /// </summary>
    /// <returns>表是否为空</returns>
    protected virtual async Task<bool> IsTableEmptyAsync() {
        return !await _client.Queryable<T>().AnyAsync();
    }
}
