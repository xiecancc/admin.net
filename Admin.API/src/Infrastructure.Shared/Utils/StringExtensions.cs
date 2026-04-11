/*
 * 文件名称: StringExtensions.cs
 * 功能描述: 字符串扩展方法，提供常用的字符串操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Shared.Utils;

/// <summary>
/// 字符串扩展方法
/// <para>提供常用的字符串操作，包括脱敏、截断、格式化等</para>
/// </summary>
public static partial class StringExtensions {
    /// <summary>
    /// 手机号脱敏
    /// <para>将手机号中间4位替换为星号</para>
    /// </summary>
    /// <param name="phone">手机号</param>
    /// <returns>脱敏后的手机号，如 138****8888</returns>
    /// <example>
    /// <code>
    /// var phone = "13812345678";
    /// var masked = phone.MaskPhone(); // "138****5678"
    /// </code>
    /// </example>
    public static string MaskPhone(this string? phone) {
        return string.IsNullOrEmpty(phone) || phone.Length < 7
            ? phone ?? string.Empty
            : string.Concat(phone.AsSpan(0, 3), "****", phone.AsSpan(phone.Length - 4));
    }

    /// <summary>
    /// 邮箱脱敏
    /// <para>将邮箱用户名部分替换为星号</para>
    /// </summary>
    /// <param name="email">邮箱地址</param>
    /// <returns>脱敏后的邮箱，如 t***@example.com</returns>
    /// <example>
    /// <code>
    /// var email = "test@example.com";
    /// var masked = email.MaskEmail(); // "t***@example.com"
    /// </code>
    /// </example>
    public static string MaskEmail(this string? email) {
        if (string.IsNullOrEmpty(email)) {
            return string.Empty;
        }

        var atIndex = email.IndexOf('@');
        if (atIndex <= 0) {
            return email;
        }

        var userName = email.AsSpan(0, atIndex);
        var domain = email.AsSpan(atIndex);

        if (userName.Length <= 1) {
            return string.Concat("*", domain);
        }

        var maskLength = Math.Min(3, userName.Length - 1);
        var mask = new string('*', maskLength);

        return string.Concat(userName[..1], mask, domain);
    }

    /// <summary>
    /// 身份证号脱敏
    /// <para>将身份证号中间部分替换为星号</para>
    /// </summary>
    /// <param name="idCard">身份证号</param>
    /// <returns>脱敏后的身份证号，如 110***********1234</returns>
    public static string MaskIdCard(this string? idCard) {
        return string.IsNullOrEmpty(idCard)
            ? string.Empty
            : idCard.Length switch {
            15 => string.Concat(idCard.AsSpan(0, 3), "*********", idCard.AsSpan(12)),
            18 => string.Concat(idCard.AsSpan(0, 3), "***********", idCard.AsSpan(14)),
            _ => idCard
        };
    }

    /// <summary>
    /// 银行卡号脱敏
    /// <para>保留前4位和后4位，中间替换为星号</para>
    /// </summary>
    /// <param name="bankCard">银行卡号</param>
    /// <returns>脱敏后的银行卡号，如 6222****1234</returns>
    public static string MaskBankCard(this string? bankCard) {
        if (string.IsNullOrEmpty(bankCard) || bankCard.Length < 8) {
            return bankCard ?? string.Empty;
        }

        var maskLength = bankCard.Length - 8;
        var mask = new string('*', maskLength);

        return string.Concat(bankCard.AsSpan(0, 4), mask, bankCard.AsSpan(bankCard.Length - 4));
    }

    /// <summary>
    /// 姓名脱敏
    /// <para>保留姓氏，名字替换为星号</para>
    /// </summary>
    /// <param name="name">姓名</param>
    /// <returns>脱敏后的姓名，如 张**</returns>
    public static string MaskName(this string? name) {
        return string.IsNullOrEmpty(name)
            ? string.Empty
            : name.Length switch {
            1 => name,
            2 => string.Concat(name.AsSpan(0, 1), "*"),
            _ => string.Concat(name.AsSpan(0, 1), new string('*', name.Length - 1))
        };
    }

    /// <summary>
    /// 截断字符串并添加省略号
    /// </summary>
    /// <param name="value">原字符串</param>
    /// <param name="maxLength">最大长度</param>
    /// <param name="suffix">后缀，默认为 "..."</param>
    /// <returns>截断后的字符串</returns>
    public static string Truncate(this string? value, int maxLength, string suffix = "...") {
        return string.IsNullOrEmpty(value) || value.Length <= maxLength
            ? value ?? string.Empty
            : string.Concat(value.AsSpan(0, maxLength - suffix.Length), suffix);
    }

    /// <summary>
    /// 判断字符串是否为空或空白
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>如果为 null、空字符串或仅包含空白字符则返回 true</returns>
    public static bool IsNullOrWhiteSpace(this string? value) {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 判断字符串是否不为空或空白
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>如果不为 null、空字符串且不仅包含空白字符则返回 true</returns>
    public static bool IsNotNullOrWhiteSpace(this string? value) {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 如果字符串为空则返回默认值
    /// </summary>
    /// <param name="value">原字符串</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>原字符串或默认值</returns>
    public static string OrDefault(this string? value, string defaultValue) {
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }

    /// <summary>
    /// 转换为驼峰命名
    /// <para>将 PascalCase 转换为 camelCase</para>
    /// </summary>
    /// <param name="value">原字符串</param>
    /// <returns>驼峰命名的字符串</returns>
    public static string ToCamelCase(this string? value) {
        return string.IsNullOrEmpty(value)
            ? string.Empty
            : string.Create(value.Length, value, (span, src) => {
            span[0] = char.ToLowerInvariant(src[0]);
            src.AsSpan(1).CopyTo(span[1..]);
        });
    }

    /// <summary>
    /// 转换为帕斯卡命名
    /// <para>将 camelCase 转换为 PascalCase</para>
    /// </summary>
    /// <param name="value">原字符串</param>
    /// <returns>帕斯卡命名的字符串</returns>
    public static string ToPascalCase(this string? value) {
        return string.IsNullOrEmpty(value)
            ? string.Empty
            : string.Create(value.Length, value, (span, src) => {
            span[0] = char.ToUpperInvariant(src[0]);
            src.AsSpan(1).CopyTo(span[1..]);
        });
    }

    /// <summary>
    /// 转换为下划线命名
    /// <para>将 PascalCase 或 camelCase 转换为 snake_case</para>
    /// </summary>
    /// <param name="value">原字符串</param>
    /// <returns>下划线命名的字符串</returns>
    public static string ToSnakeCase(this string? value) {
        if (string.IsNullOrEmpty(value)) {
            return string.Empty;
        }

        var builder = new StringBuilder();
        foreach (var c in value) {
            if (char.IsUpper(c)) {
                if (builder.Length > 0) {
                    builder.Append('_');
                }
                builder.Append(char.ToLowerInvariant(c));
            }
            else {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// 移除字符串中的所有空白字符
    /// </summary>
    /// <param name="value">原字符串</param>
    /// <returns>移除空白后的字符串</returns>
    public static string RemoveWhitespace(this string? value) {
        return string.IsNullOrEmpty(value) ? string.Empty : WhitespaceRegex().Replace(value, string.Empty);
    }

    /// <summary>
    /// 判断字符串是否是有效的手机号
    /// </summary>
    /// <param name="phone">手机号字符串</param>
    /// <returns>如果是有效的手机号返回 true</returns>
    public static bool IsValidPhone(this string? phone) {
        return !string.IsNullOrEmpty(phone) && PhoneRegex().IsMatch(phone);
    }

    /// <summary>
    /// 判断字符串是否是有效的邮箱
    /// </summary>
    /// <param name="email">邮箱字符串</param>
    /// <returns>如果是有效的邮箱返回 true</returns>
    public static bool IsValidEmail(this string? email) {
        return !string.IsNullOrEmpty(email) && EmailRegex().IsMatch(email);
    }

    /// <summary>
    /// 判断字符串是否是有效的身份证号
    /// </summary>
    /// <param name="idCard">身份证号字符串</param>
    /// <returns>如果是有效的身份证号返回 true</returns>
    public static bool IsValidIdCard(this string? idCard) {
        return !string.IsNullOrEmpty(idCard) && IdCardRegex().IsMatch(idCard);
    }

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"^1[3-9]\d{9}$", RegexOptions.Compiled)]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"^[\w.-]+@[\w.-]+\.\w+$", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^\d{15}(\d{2}[0-9Xx])?$", RegexOptions.Compiled)]
    private static partial Regex IdCardRegex();
}
