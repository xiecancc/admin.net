/*
 * 文件名称: CacheKeyBuilder.cs
 * 功能描述: 缓存键构建器，提供流式 API 构建结构化缓存键
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using System.Text;

namespace Application.Caching;

/// <summary>
/// 缓存键构建器
/// <para>提供流式 API 构建结构化缓存键，自动处理分隔符和空值</para>
/// </summary>
/// <remarks>
/// <para>使用示例：</para>
/// <code>
/// var cacheKey = CacheKeyBuilder.Create("User")
///     .Append("list")
///     .AppendIfNotEmpty("name", query.Name)
///     .AppendIfNotEmpty("status", query.Status?.ToString())
///     .Append("page", query.Page)
///     .Build();
/// // 结果: "user:list|name=xxx|status=active|page=1"
/// </code>
/// </remarks>
public sealed class CacheKeyBuilder {
    private readonly StringBuilder _builder = new();

    private CacheKeyBuilder(string prefix) {
        _builder.Append(prefix.ToLowerInvariant());
    }

    /// <summary>
    /// 创建缓存键构建器
    /// </summary>
    /// <param name="prefix">缓存键前缀（通常是实体名称）</param>
    /// <returns>缓存键构建器实例</returns>
    public static CacheKeyBuilder Create(string prefix) {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix, nameof(prefix));
        return new CacheKeyBuilder(prefix);
    }

    /// <summary>
    /// 追加键部分
    /// </summary>
    /// <param name="key">键名</param>
    /// <param name="value">键值</param>
    /// <returns>当前构建器实例</returns>
    public CacheKeyBuilder Append(string key, object? value) {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

        _builder.Append(':');
        _builder.Append(key.ToLowerInvariant());

        if (value is not null) {
            _builder.Append('=');
            _builder.Append(value);
        }

        return this;
    }

    /// <summary>
    /// 追加简单部分（无键值对）
    /// </summary>
    /// <param name="part">部分名称</param>
    /// <returns>当前构建器实例</returns>
    public CacheKeyBuilder Append(string part) {
        ArgumentException.ThrowIfNullOrWhiteSpace(part, nameof(part));

        _builder.Append(':');
        _builder.Append(part.ToLowerInvariant());
        return this;
    }

    /// <summary>
    /// 仅当值非空时追加键部分
    /// </summary>
    /// <param name="key">键名</param>
    /// <param name="value">键值（为 null 或空白时不追加）</param>
    /// <returns>当前构建器实例</returns>
    public CacheKeyBuilder AppendIfNotEmpty(string key, string? value) {
        if (!string.IsNullOrWhiteSpace(value)) {
            Append(key, value);
        }
        return this;
    }

    /// <summary>
    /// 仅当值非空时追加键部分
    /// </summary>
    /// <param name="key">键名</param>
    /// <param name="value">键值（为 null 时不追加）</param>
    /// <returns>当前构建器实例</returns>
    public CacheKeyBuilder AppendIfNotEmpty(string key, Guid? value) {
        if (value.HasValue && value.Value != Guid.Empty) {
            Append(key, value.Value);
        }
        return this;
    }

    /// <summary>
    /// 仅当值非空时追加键部分
    /// </summary>
    /// <param name="key">键名</param>
    /// <param name="value">键值（为 null 时不追加）</param>
    /// <returns>当前构建器实例</returns>
    public CacheKeyBuilder AppendIfNotEmpty(string key, int? value) {
        if (value.HasValue) {
            Append(key, value.Value);
        }
        return this;
    }

    /// <summary>
    /// 仅当值非空时追加键部分
    /// </summary>
    /// <param name="key">键名</param>
    /// <param name="value">键值（为 null 时不追加）</param>
    /// <returns>当前构建器实例</returns>
    public CacheKeyBuilder AppendIfNotEmpty(string key, long? value) {
        if (value.HasValue) {
            Append(key, value.Value);
        }
        return this;
    }

    /// <summary>
    /// 仅当值非空时追加键部分
    /// </summary>
    /// <param name="key">键名</param>
    /// <param name="value">键值（为 null 时不追加）</param>
    /// <returns>当前构建器实例</returns>
    public CacheKeyBuilder AppendIfNotEmpty(string key, DateTime? value) {
        if (value.HasValue) {
            Append(key, value.Value.ToString("O"));
        }
        return this;
    }

    /// <summary>
    /// 仅当值非空时追加键部分
    /// </summary>
    /// <param name="key">键名</param>
    /// <param name="value">键值（为 null 时不追加）</param>
    /// <returns>当前构建器实例</returns>
    public CacheKeyBuilder AppendIfNotEmpty(string key, bool? value) {
        if (value.HasValue) {
            Append(key, value.Value ? "1" : "0");
        }
        return this;
    }

    /// <summary>
    /// 仅当条件为 true 时追加键部分
    /// </summary>
    /// <param name="key">键名</param>
    /// <param name="value">键值</param>
    /// <param name="condition">是否追加的条件</param>
    /// <returns>当前构建器实例</returns>
    public CacheKeyBuilder AppendIf(string key, object? value, bool condition) {
        if (condition) {
            Append(key, value);
        }
        return this;
    }

    /// <summary>
    /// 追加分页参数
    /// </summary>
    /// <param name="page">页码</param>
    /// <param name="size">每页大小</param>
    /// <returns>当前构建器实例</returns>
    public CacheKeyBuilder AppendPaging(int page, int size) {
        Append("page", page);
        Append("size", size);
        return this;
    }

    /// <summary>
    /// 构建最终的缓存键
    /// </summary>
    /// <returns>缓存键字符串</returns>
    public string Build() {
        return _builder.ToString();
    }

    /// <summary>
    /// 隐式转换为字符串
    /// </summary>
    /// <param name="builder">缓存键构建器</param>
    public static implicit operator string(CacheKeyBuilder builder) {
        return builder.Build();
    }

    /// <inheritdoc/>
    public override string ToString() => Build();
}
