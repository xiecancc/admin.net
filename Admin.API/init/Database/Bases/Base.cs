/*
 * 文件名称: Base.cs
 * 功能描述: 数据库初始化基础类，提供 SqlSugar 客户端和日志功能
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;

namespace Database.Bases;

/// <summary>
/// 数据库初始化基础类
/// <para>提供 SqlSugar 客户端和日志功能</para>
/// </summary>
public abstract class Base {

    /// <summary>
    /// SqlSugar 客户端
    /// </summary>
    protected readonly ISqlSugarClient _client;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="client">SqlSugar 客户端，不能为空</param>
    /// <exception cref="ArgumentNullException">当 client 为 null 时抛出</exception>
    /// <remarks>
    /// <para>使用 ArgumentNullException.ThrowIfNull 进行参数校验</para>
    /// </remarks>
    protected Base(ISqlSugarClient client) {
        ArgumentNullException.ThrowIfNull(client);
        _client = client;
    }

    /// <summary>
    /// 写入信息日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="args">格式化参数</param>
    protected static void LogInfo(string message, params object[] args) {
        Console.WriteLine($"[INFO] {string.Format(message, args)}");
    }

    /// <summary>
    /// 写入异常日志
    /// </summary>
    /// <param name="ex">异常对象</param>
    /// <param name="message">日志消息</param>
    /// <param name="args">格式化参数</param>
    protected static void LogException(Exception ex, string message, params object[] args) {
        var formattedMessage = string.Format(message, args);
        Console.WriteLine($"[ERROR] {formattedMessage}");
        Console.WriteLine($"[ERROR] 异常类型：{ex.GetType().Name}");
        Console.WriteLine($"[ERROR] 详细信息：{ex.Message}");
        Console.WriteLine($"[ERROR] 堆栈跟踪：{ex.StackTrace}");
        if (ex.InnerException != null) {
            Console.WriteLine($"[ERROR] 内部异常：{ex.InnerException.Message}");
        }
    }
}
