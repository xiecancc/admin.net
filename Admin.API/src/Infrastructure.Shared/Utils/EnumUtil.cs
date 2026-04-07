/*
 * 文件名称：EnumUtils.cs
 * 功能描述：枚举类型工具类，提供字符串/数值与枚举之间的转换
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

namespace Infrastructure.Shared.Utils;

/// <summary>
/// 枚举类型工具类
/// <para>提供字符串与枚举、数值与枚举之间的相互转换</para>
/// </summary>
/// <remarks>
/// <para>主要功能：</para>
/// <list type="bullet">
///   <item>字符串转枚举（不区分大小写，失败时抛出异常）</item>
///   <item>枚举转字符串</item>
///   <item>数值转枚举（失败时抛出异常）</item>
///   <item>枚举转数值</item>
///   <item>验证字符串是否为有效枚举值</item>
///   <item>获取所有枚举值名称</item>
/// </list>
/// <para>异常处理：所有转换失败时抛出 <see cref="InvalidOperationException"/>，包含详细的错误信息</para>
/// <para>线程安全性：</para>
/// <list type="bullet">
///   <item>所有公共方法都是线程安全的</item>
///   <item>无共享状态，每次调用独立执行</item>
///   <item>可在多线程环境中安全使用</item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // 字符串转枚举
/// var dbType = EnumUtils.ToEnum&lt;SqlSugar.DbType&gt;("MySql");
/// 
/// // 枚举转字符串
/// var dbTypeStr = EnumUtils.FromEnum(SqlSugar.DbType.MySql);
/// 
/// // 数值转枚举
/// var logLevel = EnumUtils.ToEnum&lt;LogLevel&gt;(2);
/// 
/// // 枚举转数值
/// var logLevelNum = EnumUtils.ToNumber(LogLevel.Information);
/// 
/// // 带默认值的转换
/// var dbType = EnumUtils.ToEnum("Invalid", SqlSugar.DbType.MySql);
/// 
/// // 验证枚举值
/// var isValid = EnumUtils.IsValid&lt;SqlSugar.DbType&gt;("MySql");
/// </code>
/// </example>
public static class EnumUtil {
    /// <summary>
    /// 将字符串转换为枚举值
    /// <para>不区分大小写，转换失败时抛出异常</para>
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="value">字符串值</param>
    /// <returns>对应的枚举值</returns>
    /// <exception cref="ArgumentNullException">当 value 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当转换失败时抛出，包含所有有效值列表</exception>
    /// <remarks>
    /// <para>转换规则：</para>
    /// <list type="bullet">
    ///   <item>忽略大小写匹配</item>
    ///   <item>支持枚举名称和数值字符串</item>
    ///   <item>失败时抛出异常，提供所有有效值列表</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// var dbType = EnumUtils.ToEnum&lt;SqlSugar.DbType&gt;("MySql");
    /// // 返回：SqlSugar.DbType.MySql
    /// </code>
    /// </example>
    public static T ToEnum<T>(string value) where T : struct, Enum {
        ArgumentNullException.ThrowIfNull(value);

        if (Enum.TryParse<T>(value, ignoreCase: true, out var result)) {
            return result;
        }

        var validValues = string.Join(", ", Enum.GetNames<T>());
        throw new InvalidOperationException(
            $"无效的枚举值：'{value}'。类型 {typeof(T).Name} 的有效值包括：{validValues}");
    }

    /// <summary>
    /// 将字符串转换为枚举值，转换失败时返回默认值
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="value">字符串值</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>对应的枚举值或默认值</returns>
    /// <remarks>
    /// <para>当 value 为 null、空字符串或无效值时返回 defaultValue</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var dbType = EnumUtils.ToEnum("Invalid", SqlSugar.DbType.MySql);
    /// // 返回：SqlSugar.DbType.MySql（默认值）
    /// </code>
    /// </example>
    public static T ToEnum<T>(string? value, T defaultValue) where T : struct, Enum {
        return string.IsNullOrEmpty(value)
            ? defaultValue
            : Enum.TryParse<T>(value, ignoreCase: true, out var result)
            ? result
            : defaultValue;
    }

    /// <summary>
    /// 将字符串转换为枚举值，转换失败时返回默认值（可空类型）
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="value">字符串值</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>对应的枚举值或默认值</returns>
    /// <remarks>
    /// <para>当 value 为 null、空字符串或无效值时返回 defaultValue</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var languageType = EnumUtils.ToEnum(null, SqlSugar.LanguageType.Default);
    /// // 返回：SqlSugar.LanguageType.Default
    /// </code>
    /// </example>
    public static T? ToEnumNullable<T>(string? value, T? defaultValue = null) where T : struct, Enum {
        return string.IsNullOrEmpty(value)
            ? defaultValue
            : Enum.TryParse<T>(value, ignoreCase: true, out var result)
            ? result
            : defaultValue;
    }

    /// <summary>
    /// 将枚举值转换为字符串
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="value">枚举值</param>
    /// <returns>对应的字符串名称</returns>
    /// <remarks>
    /// <para>返回枚举值的名称（而非数值）</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var dbTypeStr = EnumUtils.FromEnum(SqlSugar.DbType.MySql);
    /// // 返回："MySql"
    /// </code>
    /// </example>
    public static string FromEnum<T>(T value) where T : struct, Enum {
        return value.ToString();
    }

    /// <summary>
    /// 将数值转换为枚举值
    /// <para>转换失败时抛出异常</para>
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="value">数值</param>
    /// <returns>对应的枚举值</returns>
    /// <exception cref="InvalidOperationException">当数值不是有效的枚举值时抛出</exception>
    /// <remarks>
    /// <para>验证数值是否在枚举定义范围内</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var dbType = EnumUtils.ToEnum&lt;SqlSugar.DbType&gt;(1);
    /// // 返回：SqlSugar.DbType.MySql（假设 MySql 的数值为 1）
    /// </code>
    /// </example>
    public static T ToEnum<T>(int value) where T : struct, Enum {
        if (Enum.IsDefined(typeof(T), value)) {
            return (T)Enum.ToObject(typeof(T), value);
        }

        var validValues = string.Join(", ", Enum.GetNames<T>());
        var validNumbers = string.Join(", ", Enum.GetValues(typeof(T)).Cast<int>());
        throw new InvalidOperationException(
            $"无效的枚举数值：{value}。类型 {typeof(T).Name} 的有效数值包括：{validNumbers} " +
            $"（对应名称：{validValues}）");
    }

    /// <summary>
    /// 将数值转换为枚举值，转换失败时返回默认值
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="value">数值</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>对应的枚举值或默认值</returns>
    /// <remarks>
    /// <para>当数值不是有效的枚举值时返回 defaultValue</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var dbType = EnumUtils.ToEnum(999, SqlSugar.DbType.MySql);
    /// // 返回：SqlSugar.DbType.MySql（默认值）
    /// </code>
    /// </example>
    public static T ToEnum<T>(int value, T defaultValue) where T : struct, Enum {
        return Enum.IsDefined(typeof(T), value)
            ? (T)Enum.ToObject(typeof(T), value)
            : defaultValue;
    }

    /// <summary>
    /// 将枚举值转换为数值
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="value">枚举值</param>
    /// <returns>对应的数值</returns>
    /// <example>
    /// <code>
    /// var number = EnumUtils.ToNumber(SqlSugar.DbType.MySql);
    /// // 返回：1（假设 MySql 的数值为 1）
    /// </code>
    /// </example>
    public static int ToNumber<T>(T value) where T : struct, Enum {
        return Convert.ToInt32(value);
    }

    /// <summary>
    /// 验证字符串是否为有效的枚举值
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="value">字符串值</param>
    /// <returns>true 表示有效；false 表示无效</returns>
    /// <remarks>
    /// <para>不区分大小写验证</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var isValid = EnumUtils.IsValid&lt;SqlSugar.DbType&gt;("MySql");
    /// // 返回：true
    /// 
    /// var isInvalid = EnumUtils.IsValid&lt;SqlSugar.DbType&gt;("Invalid");
    /// // 返回：false
    /// </code>
    /// </example>
    public static bool IsValid<T>(string? value) where T : struct, Enum {
        return !string.IsNullOrEmpty(value) && Enum.TryParse<T>(value, ignoreCase: true, out _);
    }

    /// <summary>
    /// 获取枚举类型的所有名称
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <returns>枚举名称数组</returns>
    /// <example>
    /// <code>
    /// var names = EnumUtils.GetNames&lt;SqlSugar.DbType&gt;();
    /// // 返回：["MySql", "SqlServer", "Sqlite", ...]
    /// </code>
    /// </example>
    public static string[] GetNames<T>() where T : struct, Enum {
        return Enum.GetNames<T>();
    }

    /// <summary>
    /// 获取枚举类型的所有数值
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <returns>数值数组</returns>
    /// <example>
    /// <code>
    /// var values = EnumUtils.GetValues&lt;SqlSugar.DbType&gt;();
    /// // 返回：[0, 1, 2, ...]
    /// </code>
    /// </example>
    public static int[] GetValues<T>() where T : struct, Enum {
        return Enum.GetValues(typeof(T)).Cast<int>().ToArray();
    }

    /// <summary>
    /// 获取枚举类型的描述信息（如果有 DescriptionAttribute）
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="value">枚举值</param>
    /// <returns>描述信息，如果没有则返回枚举名称</returns>
    /// <example>
    /// <code>
    /// [Description("MySQL 数据库")]
    /// MySql = 1
    /// 
    /// var desc = EnumUtils.GetDescription(SqlSugar.DbType.MySql);
    /// // 返回："MySQL 数据库"
    /// </code>
    /// </example>
    public static string GetDescription<T>(T value) where T : struct, Enum {
        var enumType = typeof(T);
        var memberInfo = enumType.GetMember(value.ToString());

        if (memberInfo.Length > 0) {
            var attributes = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (attributes.Length > 0) {
                return ((System.ComponentModel.DescriptionAttribute)attributes[0]).Description;
            }
        }

        return value.ToString();
    }
}
