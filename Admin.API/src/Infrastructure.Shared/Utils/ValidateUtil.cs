/*
 * 文件名称：ValidateUtil.cs
 * 功能描述：验证工具类，提供通用的验证方法，用于配置选项、DTO、领域对象等的验证
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

namespace Infrastructure.Shared.Utils;

/// <summary>
/// 验证工具类
/// <para>提供通用的验证方法，用于配置选项、DTO、领域对象等的验证</para>
/// </summary>
/// <remarks>
/// <para>职责：提供统一的验证方法和错误消息格式</para>
/// <para>验证失败时抛出 InvalidOperationException 异常</para>
/// <para>所有方法均为静态方法，可在任何地方调用</para>
/// <para>线程安全性：</para>
/// <list type="bullet">
///   <item>所有公共方法都是线程安全的</item>
///   <item>无共享状态，每次调用独立执行</item>
///   <item>可在多线程环境中安全使用</item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // 验证不为 null
/// ValidateUtil.ValidateNotNull(user, nameof(user), "用户不能为空");
/// 
/// // 验证字符串不为空
/// ValidateUtil.ValidateNotEmpty(name, nameof(name), "名称不能为空");
/// 
/// // 验证范围
/// ValidateUtil.ValidateRange(age, 0, 150, nameof(age), "年龄必须在 0-150 之间");
/// </code>
/// </example>
public static class ValidateUtil {
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
    /// ValidateUtil.ValidateNotNull(user, nameof(user));
    /// ValidateUtil.ValidateNotNull(user, nameof(user), "用户不能为空");
    /// </code>
    /// </example>
    public static void ValidateNotNull<T>(T? value, string paramName, string? message = null) {
        if (value is null) {
            var errorMessage = message ?? $"{paramName} 不能为 null";
            throw new InvalidOperationException(errorMessage);
        }
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
    /// ValidateUtil.ValidateNotEmpty(name, nameof(name));
    /// ValidateUtil.ValidateNotEmpty(name, nameof(name), "名称不能为空");
    /// </code>
    /// </example>
    public static void ValidateNotEmpty(string value, string paramName, string? message = null) {
        if (string.IsNullOrEmpty(value)) {
            var errorMessage = message ?? $"{paramName} 不能为空";
            throw new InvalidOperationException(errorMessage);
        }
    }

    /// <summary>
    /// 验证字符串不为空白
    /// </summary>
    /// <param name="value">要验证的字符串值</param>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">自定义错误信息，为 null 时使用默认错误信息</param>
    /// <exception cref="InvalidOperationException">当字符串为空白时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateUtil.ValidateNotWhiteSpace(name, nameof(name));
    /// ValidateUtil.ValidateNotWhiteSpace(name, nameof(name), "名称不能为空白");
    /// </code>
    /// </example>
    public static void ValidateNotWhiteSpace(string value, string paramName, string? message = null) {
        if (string.IsNullOrWhiteSpace(value)) {
            var errorMessage = message ?? $"{paramName} 不能为空白";
            throw new InvalidOperationException(errorMessage);
        }
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
    /// ValidateUtil.ValidateRange(port, 1, 65535, nameof(port));
    /// ValidateUtil.ValidateRange(port, 1, 65535, nameof(port), "端口号必须在 1-65535 之间");
    /// </code>
    /// </example>
    public static void ValidateRange<T>(T value, T min, T max, string paramName, string? message = null)
        where T : IComparable<T> {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0) {
            var errorMessage = message ?? $"{paramName} 必须在 {min} 到 {max} 之间";
            throw new InvalidOperationException(errorMessage);
        }
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
    /// ValidateUtil.ValidateMin(age, 0, nameof(age));
    /// ValidateUtil.ValidateMin(age, 0, nameof(age), "年龄不能小于 0");
    /// </code>
    /// </example>
    public static void ValidateMin(int value, int min, string paramName, string? message = null) {
        if (value < min) {
            var errorMessage = message ?? $"{paramName} 不能小于 {min}";
            throw new InvalidOperationException(errorMessage);
        }
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
    /// ValidateUtil.ValidateMax(count, 100, nameof(count));
    /// ValidateUtil.ValidateMax(count, 100, nameof(count), "数量不能超过 100");
    /// </code>
    /// </example>
    public static void ValidateMax(int value, int max, string paramName, string? message = null) {
        if (value > max) {
            var errorMessage = message ?? $"{paramName} 不能大于 {max}";
            throw new InvalidOperationException(errorMessage);
        }
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
    /// ValidateUtil.ValidateLength(password, 8, null, nameof(password), "密码长度至少为 8 个字符");
    /// ValidateUtil.ValidateLength(username, 3, 50, nameof(username));
    /// </code>
    /// </example>
    public static void ValidateLength(string value, int minLength, int? maxLength, string paramName, string? message = null) {
        if (value.Length < minLength) {
            var errorMessage = message ?? $"{paramName} 长度不能小于 {minLength}";
            throw new InvalidOperationException(errorMessage);
        }

        if (maxLength.HasValue && value.Length > maxLength.Value) {
            var errorMessage = message ?? $"{paramName} 长度不能大于 {maxLength.Value}";
            throw new InvalidOperationException(errorMessage);
        }
    }

    /// <summary>
    /// 验证条件是否满足
    /// </summary>
    /// <param name="condition">要验证的条件</param>
    /// <param name="message">错误信息</param>
    /// <exception cref="InvalidOperationException">当条件为 false 时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateUtil.ValidateCondition(age >= 18, "年龄必须大于等于 18 岁");
    /// </code>
    /// </example>
    public static void ValidateCondition(bool condition, string message) {
        if (!condition) {
            throw new InvalidOperationException(message);
        }
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
    /// ValidateUtil.ValidateUrl(website, nameof(website));
    /// ValidateUtil.ValidateUrl(website, nameof(website), "网站地址格式无效");
    /// </code>
    /// </example>
    public static void ValidateUrl(string url, string paramName, string? message = null) {
        var isValid = Uri.TryCreate(url, UriKind.Absolute, out Uri? result)
            && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);

        if (!isValid) {
            var errorMessage = message ?? $"{paramName} 格式无效，必须是有效的 URL（如 https://example.com）";
            throw new InvalidOperationException(errorMessage);
        }
    }

    /// <summary>
    /// 验证邮箱格式是否有效
    /// </summary>
    /// <param name="email">要验证的邮箱地址</param>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">自定义错误信息，为 null 时使用默认错误信息</param>
    /// <exception cref="InvalidOperationException">当邮箱格式无效时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateUtil.ValidateEmail(email, nameof(email));
    /// ValidateUtil.ValidateEmail(email, nameof(email), "邮箱地址格式无效");
    /// </code>
    /// </example>
    public static void ValidateEmail(string email, string paramName, string? message = null) {
        var isValid = !string.IsNullOrWhiteSpace(email)
            && email.Contains('@')
            && email.IndexOf('@') > 0
            && email.IndexOf('@') < email.Length - 1;

        if (!isValid) {
            var errorMessage = message ?? $"{paramName} 格式无效，必须是有效的邮箱地址";
            throw new InvalidOperationException(errorMessage);
        }
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
    /// ValidateUtil.ValidateCollection(tags, (tag, index) => {
    ///     ValidateUtil.ValidateNotEmpty(tag, $"{nameof(tags)}[{index}]");
    /// }, nameof(tags));
    /// </code>
    /// </example>
    public static void ValidateCollection<T>(IList<T> collection, Action<T, int> validateElement, string paramName) {
        for (int i = 0; i < collection.Count; i++) {
            validateElement(collection[i], i);
        }
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
    /// ValidateUtil.ValidateUnique(users, u => u.Id, nameof(users), "用户 ID 必须唯一");
    /// </code>
    /// </example>
    public static void ValidateUnique<T, TKey>(IEnumerable<T> collection, Func<T, TKey> keySelector, string paramName, string? message = null)
        where TKey : notnull {
        var duplicateKeys = collection
            .Select(keySelector)
            .Where(key => key != null && !key.Equals(default(TKey)))
            .GroupBy(key => key)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateKeys.Count > 0) {
            var errorMessage = message ?? $"{paramName} 中存在重复的元素：{string.Join(", ", duplicateKeys)}";
            throw new InvalidOperationException(errorMessage);
        }
    }

    /// <summary>
    /// 验证集合不为空
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    /// <param name="collection">要验证的集合</param>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">自定义错误信息，为 null 时使用默认错误信息</param>
    /// <exception cref="InvalidOperationException">当集合为空时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateUtil.ValidateCollectionNotEmpty(items, nameof(items));
    /// ValidateUtil.ValidateCollectionNotEmpty(items, nameof(items), "列表不能为空");
    /// </code>
    /// </example>
    public static void ValidateCollectionNotEmpty<T>(ICollection<T> collection, string paramName, string? message = null) {
        if (collection == null || collection.Count == 0) {
            var errorMessage = message ?? $"{paramName} 不能为空";
            throw new InvalidOperationException(errorMessage);
        }
    }

    /// <summary>
    /// 验证日期范围
    /// </summary>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">自定义错误信息，为 null 时使用默认错误信息</param>
    /// <exception cref="InvalidOperationException">当开始日期大于结束日期时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateUtil.ValidateDateRange(startDate, endDate, nameof(startDate));
    /// ValidateUtil.ValidateDateRange(startDate, endDate, nameof(startDate), "开始日期不能大于结束日期");
    /// </code>
    /// </example>
    public static void ValidateDateRange(DateTime startDate, DateTime endDate, string paramName, string? message = null) {
        if (startDate > endDate) {
            var errorMessage = message ?? $"{paramName} 开始日期不能大于结束日期";
            throw new InvalidOperationException(errorMessage);
        }
    }

    /// <summary>
    /// 验证枚举值是否有效
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="value">要验证的枚举值</param>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">自定义错误信息，为 null 时使用默认错误信息</param>
    /// <exception cref="InvalidOperationException">当枚举值无效时抛出异常</exception>
    /// <example>
    /// <code>
    /// ValidateUtil.ValidateEnum(status, nameof(status));
    /// ValidateUtil.ValidateEnum(status, nameof(status), "状态值无效");
    /// </code>
    /// </example>
    public static void ValidateEnum<T>(T value, string paramName, string? message = null)
        where T : struct, Enum {
        if (!Enum.IsDefined(typeof(T), value)) {
            var errorMessage = message ?? $"{paramName} 值 {value} 不是有效的枚举值";
            throw new InvalidOperationException(errorMessage);
        }
    }
}
