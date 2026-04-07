/*
 * 文件名称: GuidExtensions.cs
 * 功能描述: GUID 扩展方法，提供常用的 GUID 操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Shared.Utils;

/// <summary>
/// GUID 扩展方法
/// <para>提供常用的 GUID 操作，包括生成、验证、格式化等</para>
/// </summary>
/// <example>
/// <code>
/// // 判断 GUID 是否为空
/// var guid = Guid.Empty;
/// var isEmpty = guid.IsEmpty(); // 返回：true
/// 
/// // 生成有序 GUID（适合数据库主键）
/// var sequentialGuid = GuidExtensions.NewSequentialGuid();
/// 
/// // 生成短 GUID（22个字符）
/// var shortGuid = GuidExtensions.NewShortGuid(); // 如："Xm9zF4T7Ee6F3G8HjK2LmN"
/// 
/// // 从字符串解析 GUID
/// var parsed = "a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11".TryParseToGuid();
/// </code>
/// </example>
public static class GuidExtensions {
    /// <summary>
    /// 判断 GUID 是否为空
    /// </summary>
    /// <param name="guid">GUID</param>
    /// <returns>如果为空则返回 true</returns>
    public static bool IsEmpty(this Guid guid) {
        return guid == Guid.Empty;
    }

    /// <summary>
    /// 判断 GUID 是否不为空
    /// </summary>
    /// <param name="guid">GUID</param>
    /// <returns>如果不为空则返回 true</returns>
    public static bool IsNotEmpty(this Guid guid) {
        return guid != Guid.Empty;
    }

    /// <summary>
    /// 判断可空 GUID 是否为空或 null
    /// </summary>
    /// <param name="guid">可空 GUID</param>
    /// <returns>如果为空或 null 则返回 true</returns>
    public static bool IsNullOrEmpty(this Guid? guid) {
        return guid is null || guid.Value == Guid.Empty;
    }

    /// <summary>
    /// 判断可空 GUID 是否不为空
    /// </summary>
    /// <param name="guid">可空 GUID</param>
    /// <returns>如果不为空且不为 null 则返回 true</returns>
    public static bool IsNotNullOrEmpty(this Guid? guid) {
        return guid is not null && guid.Value != Guid.Empty;
    }

    /// <summary>
    /// 如果 GUID 为空则返回 null
    /// </summary>
    /// <param name="guid">GUID</param>
    /// <returns>GUID 或 null</returns>
    public static Guid? NullIfEmpty(this Guid guid) {
        return guid == Guid.Empty ? null : guid;
    }

    /// <summary>
    /// 如果可空 GUID 为 null 则返回空 GUID
    /// </summary>
    /// <param name="guid">可空 GUID</param>
    /// <returns>GUID</returns>
    public static Guid EmptyIfNull(this Guid? guid) {
        return guid ?? Guid.Empty;
    }

    /// <summary>
    /// 转换为不带连字符的字符串
    /// <para>格式：N（32位数字）</para>
    /// </summary>
    /// <param name="guid">GUID</param>
    /// <returns>不带连字符的字符串</returns>
    public static string ToNoDashString(this Guid guid) {
        return guid.ToString("N");
    }

    /// <summary>
    /// 转换为大写字符串
    /// </summary>
    /// <param name="guid">GUID</param>
    /// <returns>大写字符串</returns>
    public static string ToUpperString(this Guid guid) {
        return guid.ToString().ToUpperInvariant();
    }

    /// <summary>
    /// 转换为不带连字符的大写字符串
    /// </summary>
    /// <param name="guid">GUID</param>
    /// <returns>不带连字符的大写字符串</returns>
    public static string ToUpperNoDashString(this Guid guid) {
        return guid.ToString("N").ToUpperInvariant();
    }

    /// <summary>
    /// 尝试从字符串解析 GUID
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>解析后的 GUID，如果解析失败则返回 null</returns>
    public static Guid? TryParseToGuid(this string? value) {
        return string.IsNullOrWhiteSpace(value) ? null : Guid.TryParse(value, out var guid) ? guid : null;
    }

    /// <summary>
    /// 判断字符串是否是有效的 GUID
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>如果是有效的 GUID 则返回 true</returns>
    public static bool IsValidGuid(this string? value) {
        return !string.IsNullOrWhiteSpace(value) && Guid.TryParse(value, out _);
    }

    /// <summary>
    /// 生成有序 GUID（用于数据库主键）
    /// <para>生成的 GUID 按时间排序，适合作为聚集索引主键</para>
    /// </summary>
    /// <returns>有序 GUID</returns>
    public static Guid NewSequentialGuid() {
        var timestamp = DateTime.UtcNow.Ticks / 10000L;
        var timestampBytes = BitConverter.GetBytes(timestamp);

        if (BitConverter.IsLittleEndian) {
            Array.Reverse(timestampBytes);
        }

        var randomBytes = RandomNumberGenerator.GetBytes(8);

        var guidBytes = new byte[16];
        Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
        Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

        return new Guid(guidBytes);
    }

    /// <summary>
    /// 生成短 GUID（22个字符的 Base64 编码）
    /// </summary>
    /// <returns>短 GUID 字符串</returns>
    public static string NewShortGuid() {
        return Guid.NewGuid().ToShortString();
    }

    /// <summary>
    /// 将 GUID 转换为短字符串（22个字符的 Base64 编码）
    /// </summary>
    /// <param name="guid">GUID</param>
    /// <returns>短字符串</returns>
    public static string ToShortString(this Guid guid) {
        var base64 = Convert.ToBase64String(guid.ToByteArray());
        return base64.Replace("/", "_").Replace("+", "-")[..22];
    }

    /// <summary>
    /// 从短字符串解析 GUID
    /// </summary>
    /// <param name="shortGuid">短字符串</param>
    /// <returns>GUID</returns>
    public static Guid FromShortString(string shortGuid) {
        ArgumentException.ThrowIfNullOrEmpty(shortGuid);

        var base64 = shortGuid.Replace("_", "/").Replace("-", "+") + "==";
        var bytes = Convert.FromBase64String(base64);
        return new Guid(bytes);
    }

    /// <summary>
    /// 尝试从短字符串解析 GUID
    /// </summary>
    /// <param name="shortGuid">短字符串</param>
    /// <returns>GUID，如果解析失败则返回 null</returns>
    public static Guid? TryParseFromShortString(string? shortGuid) {
        if (string.IsNullOrWhiteSpace(shortGuid) || shortGuid.Length != 22) {
            return null;
        }

        try {
            return FromShortString(shortGuid);
        }
        catch {
            return null;
        }
    }

    /// <summary>
    /// 判断字符串是否是有效的短 GUID
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>如果是有效的短 GUID 则返回 true</returns>
    public static bool IsValidShortGuid(this string? value) {
        return TryParseFromShortString(value).HasValue;
    }

    /// <summary>
    /// 生成基于名称的 GUID（版本5）
    /// <para>相同命名空间和名称总是生成相同的 GUID</para>
    /// </summary>
    /// <param name="namespaceId">命名空间 GUID</param>
    /// <param name="name">名称</param>
    /// <returns>基于名称的 GUID</returns>
    public static Guid NewNameBasedGuid(Guid namespaceId, string name) {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var namespaceBytes = namespaceId.ToByteArray();
        var nameBytes = Encoding.UTF8.GetBytes(name);

        if (BitConverter.IsLittleEndian) {
            SwapByteOrder(namespaceBytes);
        }

        var hash = SHA1.HashData([.. namespaceBytes, .. nameBytes]);

        var guidBytes = new byte[16];
        Array.Copy(hash, guidBytes, 16);

        guidBytes[6] = (byte)((guidBytes[6] & 0x0F) | 0x50);
        guidBytes[8] = (byte)((guidBytes[8] & 0x3F) | 0x80);

        return new Guid(guidBytes);
    }

    private static void SwapByteOrder(byte[] guid) {
        (guid[0], guid[3]) = (guid[3], guid[0]);
        (guid[1], guid[2]) = (guid[2], guid[1]);
        (guid[4], guid[5]) = (guid[5], guid[4]);
        (guid[6], guid[7]) = (guid[7], guid[6]);
    }
}
