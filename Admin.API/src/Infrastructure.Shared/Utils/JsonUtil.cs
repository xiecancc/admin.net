/*
 * 文件名称: JsonUtils.cs
 * 功能描述: JSON 序列化工具类，提供对象与 JSON 之间的转换功能
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Shared.Utils;

/// <summary>
/// JSON 序列化工具类
/// <para>提供对象与 JSON 之间的转换功能</para>
/// </summary>
/// <remarks>
/// <para>主要功能：</para>
/// <list type="bullet">
///   <item>将对象序列化为 JSON 字符串</item>
///   <item>将 JSON 字符串反序列化为对象</item>
///   <item>将对象序列化为 JSON 字节数组</item>
///   <item>将 JSON 字节数组反序列化为对象</item>
/// </list>
/// <para>默认序列化选项：</para>
/// <list type="bullet">
///   <item>使用驼峰命名策略</item>
///   <item>无缩进（生产环境友好）</item>
///   <item>忽略 null 值</item>
///   <item>枚举转换为驼峰命名的字符串</item>
///   <item>UTC 时间转换为本地时间</item>
/// </list>
/// <para>线程安全性：</para>
/// <list type="bullet">
///   <item>所有公共方法都是线程安全的</item>
///   <item>DefaultOptions 为静态只读字段，初始化后不可变</item>
///   <item>System.Text.Json.JsonSerializer 是无状态的，支持并发调用</item>
///   <item>可在多线程环境中安全使用</item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // 序列化对象为 JSON 字符串
/// var user = new User { Id = 1, Name = "Alice" };
/// var json = JsonUtil.Serialize(user); // {"id":1,"name":"Alice"}
/// 
/// // 反序列化 JSON 字符串为对象
/// var user2 = JsonUtil.Deserialize&lt;User&gt;("{\"id\":2,\"name\":\"Bob\"}");
/// 
/// // 序列化为字节数组
/// var bytes = JsonUtil.SerializeToBytes(user);
/// 
/// // 从字节数组反序列化
/// var user3 = JsonUtil.DeserializeFromBytes&lt;User&gt;(bytes);
/// </code>
/// </example>
public static class JsonUtil {
    /// <summary>
    /// 紧凑的 JSON 序列化选项（生产环境友好，无缩进）
    /// </summary>
    public static readonly JsonSerializerOptions DefaultOptions = new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
            new UtcToLocalDateTimeConverter(),
            new UtcToLocalNullableDateTimeConverter()
        }
    };

    /// <summary>
    /// 将对象序列化为 JSON 字符串
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="obj">要序列化的对象</param>
    /// <param name="options">序列化选项，默认为 <see cref="DefaultOptions"/></param>
    /// <returns>JSON 字符串</returns>
    public static string Serialize<T>(T obj, JsonSerializerOptions? options = null) {
        return JsonSerializer.Serialize(obj, options ?? DefaultOptions);
    }

    /// <summary>
    /// 将 JSON 字符串反序列化为对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="json">JSON 字符串</param>
    /// <param name="options">序列化选项，默认为 <see cref="DefaultOptions"/></param>
    /// <returns>反序列化后的对象</returns>
    public static T? Deserialize<T>(string json, JsonSerializerOptions? options = null) {
        return JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
    }

    /// <summary>
    /// 将对象序列化为 JSON 字节数组
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="obj">要序列化的对象</param>
    /// <param name="options">序列化选项，默认为 <see cref="DefaultOptions"/></param>
    /// <returns>JSON 字节数组</returns>
    public static byte[] SerializeToBytes<T>(T obj, JsonSerializerOptions? options = null) {
        return JsonSerializer.SerializeToUtf8Bytes(obj, options ?? DefaultOptions);
    }

    /// <summary>
    /// 将 JSON 字节数组反序列化为对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="bytes">JSON 字节数组</param>
    /// <param name="options">序列化选项，默认为 <see cref="DefaultOptions"/></param>
    /// <returns>反序列化后的对象</returns>
    public static T? DeserializeFromBytes<T>(byte[] bytes, JsonSerializerOptions? options = null) {
        return JsonSerializer.Deserialize<T>(bytes, options ?? DefaultOptions);
    }
}

/// <summary>
/// UTC 时间转本地时间的 JSON 转换器（内部私有）
/// </summary>
internal sealed class UtcToLocalDateTimeConverter : JsonConverter<DateTime> {
    /// <summary>
    /// 读取 JSON 并转换为 DateTime
    /// </summary>
    /// <param name="reader">JSON 读取器</param>
    /// <param name="typeToConvert">要转换的类型</param>
    /// <param name="options">序列化选项</param>
    /// <returns>转换后的 DateTime</returns>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var dateTime = reader.GetDateTime();
        return dateTime.Kind == DateTimeKind.Utc ? dateTime.ToLocalTime() : dateTime;
    }

    /// <summary>
    /// 将 DateTime 写入 JSON，UTC 时间转换为本地时间
    /// </summary>
    /// <param name="writer">JSON 写入器</param>
    /// <param name="value">要写入的 DateTime 值</param>
    /// <param name="options">序列化选项</param>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) {
        var localTime = value.Kind == DateTimeKind.Utc ? value.ToLocalTime() : value;
        writer.WriteStringValue(localTime.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
    }
}

/// <summary>
/// 可空 DateTime 的 UTC 转本地时间转换器（内部私有）
/// </summary>
internal sealed class UtcToLocalNullableDateTimeConverter : JsonConverter<DateTime?> {
    /// <summary>
    /// 读取 JSON 并转换为可空 DateTime
    /// </summary>
    /// <param name="reader">JSON 读取器</param>
    /// <param name="typeToConvert">要转换的类型</param>
    /// <param name="options">序列化选项</param>
    /// <returns>转换后的可空 DateTime</returns>
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var dateTime = reader.GetDateTime();
        return dateTime.Kind == DateTimeKind.Utc ? dateTime.ToLocalTime() : dateTime;
    }

    /// <summary>
    /// 将可空 DateTime 写入 JSON，UTC 时间转换为本地时间
    /// </summary>
    /// <param name="writer">JSON 写入器</param>
    /// <param name="value">要写入的可空 DateTime 值</param>
    /// <param name="options">序列化选项</param>
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options) {
        if (value is null) {
            writer.WriteNullValue();
            return;
        }

        var localTime = value.Value.Kind == DateTimeKind.Utc ? value.Value.ToLocalTime() : value.Value;
        writer.WriteStringValue(localTime.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
    }
}
