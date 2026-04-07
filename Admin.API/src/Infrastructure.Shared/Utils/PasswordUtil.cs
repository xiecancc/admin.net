/*
 * 文件名称: PasswordUtils.cs
 * 功能描述: 密码加密工具类，使用 BCrypt 算法进行密码哈希和验证
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

namespace Infrastructure.Shared.Utils;

/// <summary>
/// 密码加密工具类
/// <para>使用 BCrypt 算法进行密码哈希，工作因子为 12</para>
/// </summary>
/// <remarks>
/// <para>主要功能：</para>
/// <list type="bullet">
///   <item>对密码进行哈希处理</item>
///   <item>验证密码是否正确</item>
/// </list>
/// <para>使用 BCrypt 算法的优势：</para>
/// <list type="bullet">
///   <item>自动生成和存储盐值</item>
///   <item>使用工作因子控制哈希强度</item>
///   <item>使用常量时间比较防止时序攻击</item>
/// </list>
/// <para>线程安全性：</para>
/// <list type="bullet">
///   <item>所有公共方法都是线程安全的</item>
///   <item>无共享状态，每次调用独立执行</item>
///   <item>BCrypt.Net.BCrypt 内部无静态状态</item>
///   <item>可在多线程环境中安全使用</item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // 哈希密码
/// var plainPassword = "MySecretPassword123!";
/// var hashedPassword = PasswordUtil.HashPassword(plainPassword);
/// // 返回类似：$2a$12$N9qo8uLOickgx2ZMRZoMy.MrqJ3W4F8Z6Z6Z6Z6Z6Z6Z6Z6Z6Z6Z6
/// 
/// // 验证密码
/// var isValid = PasswordUtil.VerifyPassword(hashedPassword, plainPassword); // 返回：true
/// var isInvalid = PasswordUtil.VerifyPassword(hashedPassword, "WrongPassword"); // 返回：false
/// </code>
/// </example>
public static class PasswordUtil {
    /// <summary>
    /// 对密码进行哈希处理
    /// </summary>
    /// <param name="password">原始密码（明文）</param>
    /// <returns>哈希后的密码（包含盐值）</returns>
    /// <remarks>
    /// <para>工作因子：12（2^12 次迭代）</para>
    /// <para>返回值格式：$2a$12$salt+hash</para>
    /// </remarks>
    public static string HashPassword(string password) {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    /// <summary>
    /// 验证密码是否正确
    /// </summary>
    /// <param name="hashedPassword">存储的哈希密码</param>
    /// <param name="providedPassword">用户提供的密码（明文）</param>
    /// <returns>密码是否匹配</returns>
    /// <remarks>
    /// <para>自动从哈希值中提取盐值进行验证</para>
    /// <para>使用常量时间比较防止时序攻击</para>
    /// <para>如果哈希密码或提供的密码为空，则返回 false</para>
    /// </remarks>
    public static bool VerifyPassword(string? hashedPassword, string providedPassword) {
        return !string.IsNullOrWhiteSpace(hashedPassword) && !string.IsNullOrWhiteSpace(providedPassword) && BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
}
