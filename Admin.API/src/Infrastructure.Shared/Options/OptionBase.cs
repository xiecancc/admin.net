/*
 * 文件名称：OptionBase.cs
 * 功能描述：配置选项基类，提供配置验证的抽象方法和通用验证辅助方法
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

using Infrastructure.Shared.Utils;

namespace Infrastructure.Shared.Options;

/// <summary>
/// 配置选项基类
/// <para>所有配置选项类的基类，提供配置验证的抽象方法和通用验证辅助方法</para>
/// </summary>
/// <remarks>
/// <para>子类需要实现 Validate() 方法来验证配置的有效性</para>
/// <para>验证失败时应抛出 InvalidOperationException 异常</para>
/// <para>提供了多种验证辅助方法，简化子类的验证逻辑</para>
/// <para>验证方法内部调用 ValidateUtil 工具类实现</para>
/// </remarks>
/// <example>
/// <code>
/// public class JwtOption : OptionBase
/// {
///     public string SecretKey { get; set; }
///     public int ExpirationMinutes { get; set; }
///     
///     public override void Validate()
///     {
///         ValidateNotNull(SecretKey, nameof(SecretKey), "JWT 密钥不能为空");
///         ValidateMin(ExpirationMinutes, 1, nameof(ExpirationMinutes), "过期时间必须大于 0");
///     }
/// }
/// </code>
/// </example>
public abstract class OptionBase {
    /// <summary>
    /// 验证配置有效性
    /// </summary>
    /// <exception cref="InvalidOperationException">配置无效时抛出异常</exception>
    public abstract void Validate();

    /// <summary>
    /// 验证值不为 null
    /// </summary>
    /// <typeparam name="T">值的类型</typeparam>
    /// <param name="value">要验证的值</param>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">自定义错误信息，为 null 时使用默认错误信息</param>
    /// <exception cref="InvalidOperationException">当值为 null 时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateNotNull(SecretKey, nameof(SecretKey));
    /// ValidateNotNull(SecretKey, nameof(SecretKey), "JWT 密钥不能为空");
    /// </code>
    /// </example>
    protected void ValidateNotNull<T>(T? value, string paramName, string? message = null) {
        ValidateUtil.ValidateNotNull(value, paramName, message);
    }

    /// <summary>
    /// 验证字符串不为空
    /// </summary>
    /// <param name="value">要验证的字符串值</param>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">自定义错误信息，为 null 时使用默认错误信息</param>
    /// <exception cref="InvalidOperationException">当字符串为空时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateNotEmpty(ConnectionString, nameof(ConnectionString));
    /// ValidateNotEmpty(ConnectionString, nameof(ConnectionString), "数据库连接字符串不能为空");
    /// </code>
    /// </example>
    protected void ValidateNotEmpty(string value, string paramName, string? message = null) {
        ValidateUtil.ValidateNotEmpty(value, paramName, message);
    }

    /// <summary>
    /// 验证值在指定范围内
    /// </summary>
    /// <typeparam name="T">值的类型，必须实现 IComparable{T}</typeparam>
    /// <param name="value">要验证的值</param>
    /// <param name="min">最小值（包含）</param>
    /// <param name="max">最大值（包含）</param>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">自定义错误信息，为 null 时使用默认错误信息</param>
    /// <exception cref="InvalidOperationException">当值不在指定范围内时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateRange(Port, 1, 65535, nameof(Port));
    /// ValidateRange(Port, 1, 65535, nameof(Port), "端口号必须在 1-65535 之间");
    /// </code>
    /// </example>
    protected void ValidateRange<T>(T value, T min, T max, string paramName, string? message = null)
        where T : IComparable<T> {
        ValidateUtil.ValidateRange(value, min, max, paramName, message);
    }

    /// <summary>
    /// 验证整数值不小于最小值
    /// </summary>
    /// <param name="value">要验证的整数值</param>
    /// <param name="min">最小值（包含）</param>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">自定义错误信息，为 null 时使用默认错误信息</param>
    /// <exception cref="InvalidOperationException">当值小于最小值时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateMin(ExpirationMinutes, 1, nameof(ExpirationMinutes));
    /// ValidateMin(ExpirationMinutes, 1, nameof(ExpirationMinutes), "过期时间必须大于 0");
    /// </code>
    /// </example>
    protected void ValidateMin(int value, int min, string paramName, string? message = null) {
        ValidateUtil.ValidateMin(value, min, paramName, message);
    }

    /// <summary>
    /// 验证整数值不大于最大值
    /// </summary>
    /// <param name="value">要验证的整数值</param>
    /// <param name="max">最大值（包含）</param>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">自定义错误信息，为 null 时使用默认错误信息</param>
    /// <exception cref="InvalidOperationException">当值大于最大值时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateMax(MaxSize, 100, nameof(MaxSize));
    /// ValidateMax(MaxSize, 100, nameof(MaxSize), "最大大小不能超过 100");
    /// </code>
    /// </example>
    protected void ValidateMax(int value, int max, string paramName, string? message = null) {
        ValidateUtil.ValidateMax(value, max, paramName, message);
    }

    /// <summary>
    /// 验证字符串长度
    /// </summary>
    /// <param name="value">要验证的字符串值</param>
    /// <param name="minLength">最小长度（包含）</param>
    /// <param name="maxLength">最大长度（包含），null 表示不限制</param>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">自定义错误信息，为 null 时使用默认错误信息</param>
    /// <exception cref="InvalidOperationException">当字符串长度不在指定范围内时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateLength(SecretKey, 32, null, nameof(SecretKey), "密钥长度至少为 32 个字符");
    /// ValidateLength(Username, 3, 50, nameof(Username));
    /// </code>
    /// </example>
    protected void ValidateLength(string value, int minLength, int? maxLength, string paramName, string? message = null) {
        ValidateUtil.ValidateLength(value, minLength, maxLength, paramName, message);
    }

    /// <summary>
    /// 验证条件是否满足
    /// </summary>
    /// <param name="condition">要验证的条件</param>
    /// <param name="message">错误信息</param>
    /// <exception cref="InvalidOperationException">当条件为 false 时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateCondition(AllowCredentials &amp;&amp; !AllowAnyOrigin, "AllowCredentials 与 AllowAnyOrigin 不能同时为 true");
    /// </code>
    /// </example>
    protected void ValidateCondition(bool condition, string message) {
        ValidateUtil.ValidateCondition(condition, message);
    }

    /// <summary>
    /// 验证 URL 格式是否有效
    /// </summary>
    /// <param name="url">要验证的 URL</param>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">自定义错误信息，为 null 时使用默认错误信息</param>
    /// <exception cref="InvalidOperationException">当 URL 格式无效时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateUrl(origin, nameof(origin), "源地址格式无效");
    /// </code>
    /// </example>
    protected void ValidateUrl(string url, string paramName, string? message = null) {
        ValidateUtil.ValidateUrl(url, paramName, message);
    }

    /// <summary>
    /// 验证集合中的元素
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    /// <param name="collection">要验证的集合</param>
    /// <param name="validateElement">验证每个元素的函数</param>
    /// <param name="paramName">参数名称</param>
    /// <exception cref="InvalidOperationException">当集合中有元素验证失败时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateCollection(AllowedOrigins, (origin, index) => {
    ///     ValidateNotEmpty(origin, $"{paramName}[{index}]");
    ///     ValidateUrl(origin, $"{paramName}[{index}]");
    /// }, nameof(AllowedOrigins));
    /// </code>
    /// </example>
    protected void ValidateCollection<T>(IList<T> collection, Action<T, int> validateElement, string paramName) {
        ValidateUtil.ValidateCollection(collection, validateElement, paramName);
    }

    /// <summary>
    /// 验证集合中元素是否唯一
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    /// <typeparam name="TKey">用于判断唯一性的键类型</typeparam>
    /// <param name="collection">要验证的集合</param>
    /// <param name="keySelector">选择用于判断唯一性的键的函数</param>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">自定义错误信息，为 null 时使用默认错误信息</param>
    /// <exception cref="InvalidOperationException">当集合中有重复元素时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateUnique(EndpointPolicies, e => e.PolicyName, nameof(EndpointPolicies), "策略名称必须唯一");
    /// </code>
    /// </example>
    protected void ValidateUnique<T, TKey>(IEnumerable<T> collection, Func<T, TKey> keySelector, string paramName, string? message = null)
        where TKey : notnull {
        ValidateUtil.ValidateUnique(collection, keySelector, paramName, message);
    }
}
