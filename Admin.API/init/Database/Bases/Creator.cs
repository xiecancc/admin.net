/*
 * 文件名称: Creator.cs
 * 功能描述: 创建器抽象基类，定义数据库创建操作的标准接口
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;

namespace Database.Bases;

/// <summary>
/// 创建器抽象基类
/// <para>定义数据库创建操作的标准接口</para>
/// </summary>
/// <remarks>
/// <para>继承此类的子类需要实现 Create 方法</para>
/// </remarks>
/// <param name="client">SqlSugar 客户端</param>
public abstract class Creator(ISqlSugarClient client) : Base(client) {

    /// <summary>
    /// 执行创建操作
    /// </summary>
    public abstract void Create();
}
