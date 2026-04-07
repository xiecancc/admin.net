/*
 * 文件名称: CollectionExtensions.cs
 * 功能描述: 集合扩展方法，提供常用的集合操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

namespace Infrastructure.Shared.Utils;

/// <summary>
/// 集合扩展方法
/// <para>提供常用的集合操作，包括分页、分组、转换等</para>
/// </summary>
public static class CollectionExtensions {
    /// <summary>
    /// 判断集合是否为空或 null
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <returns>如果集合为 null 或没有元素则返回 true</returns>
    /// <example>
    /// <code>
    /// List&lt;int&gt;? numbers = null;
    /// var isEmpty = numbers.IsNullOrEmpty(); // 返回：true
    /// 
    /// var emptyList = new List&lt;string&gt;();
    /// var isEmpty2 = emptyList.IsNullOrEmpty(); // 返回：true
    /// 
    /// var items = new List&lt;int&gt; { 1, 2, 3 };
    /// var isEmpty3 = items.IsNullOrEmpty(); // 返回：false
    /// </code>
    /// </example>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source) {
        return source is null || !source.Any();
    }

    /// <summary>
    /// 判断集合是否不为空
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <returns>如果集合不为 null 且有元素则返回 true</returns>
    /// <example>
    /// <code>
    /// var items = new List&lt;int&gt; { 1, 2, 3 };
    /// var isNotEmpty = items.IsNotNullOrEmpty(); // 返回：true
    /// 
    /// List&lt;string&gt;? nullList = null;
    /// var isNotEmpty2 = nullList.IsNotNullOrEmpty(); // 返回：false
    /// </code>
    /// </example>
    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T>? source) {
        return source is not null && source.Any();
    }

    /// <summary>
    /// 对集合的每个元素执行指定操作
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="action">要执行的操作</param>
    /// <example>
    /// <code>
    /// var items = new List&lt;string&gt; { "a", "b", "c" };
    /// items.ForEach(item => Console.WriteLine(item));
    /// // 输出：
    /// // a
    /// // b
    /// // c
    /// </code>
    /// </example>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in source) {
            action(item);
        }
    }

    /// <summary>
    /// 对集合的每个元素执行指定操作（带索引）
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="action">要执行的操作</param>
    /// <example>
    /// <code>
    /// var items = new List&lt;string&gt; { "a", "b", "c" };
    /// items.ForEach((item, index) => Console.WriteLine($"{index}: {item}"));
    /// // 输出：
    /// // 0: a
    /// // 1: b
    /// // 2: c
    /// </code>
    /// </example>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action) {
        ArgumentNullException.ThrowIfNull(action);

        var index = 0;
        foreach (var item in source) {
            action(item, index++);
        }
    }

    /// <summary>
    /// 将集合分页
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="pageNumber">页码（从1开始）</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>分页后的集合</returns>
    /// <example>
    /// <code>
    /// var items = Enumerable.Range(1, 100);
    /// var page1 = items.Paginate(1, 10).ToList(); // 返回：[1, 2, 3, ..., 10]
    /// var page2 = items.Paginate(2, 10).ToList(); // 返回：[11, 12, 13, ..., 20]
    /// </code>
    /// </example>
    public static IEnumerable<T> Paginate<T>(this IEnumerable<T> source, int pageNumber, int pageSize) {
        if (pageNumber < 1) {
            throw new ArgumentException("页码必须大于0", nameof(pageNumber));
        }

        return pageSize < 1
            ? throw new ArgumentException("每页大小必须大于0", nameof(pageSize))
            : source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// 将集合分页
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="pageNumber">页码（从1开始）</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>分页后的集合</returns>
    /// <example>
    /// <code>
    /// var query = dbContext.Users.AsQueryable();
    /// var page1 = query.Paginate(1, 20).ToList(); // 获取第1页，每页20条
    /// </code>
    /// </example>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int pageNumber, int pageSize) {
        if (pageNumber < 1) {
            throw new ArgumentException("页码必须大于0", nameof(pageNumber));
        }

        return pageSize < 1
            ? throw new ArgumentException("每页大小必须大于0", nameof(pageSize))
            : source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// 将集合转换为分页结果
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="pageNumber">页码（从1开始）</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>分页结果</returns>
    /// <example>
    /// <code>
    /// var items = Enumerable.Range(1, 100);
    /// var result = items.ToPagedResult(1, 10);
    /// // result.Items = [1, 2, 3, ..., 10]
    /// // result.TotalCount = 100
    /// // result.PageNumber = 1
    /// // result.PageSize = 10
    /// // result.TotalPages = 10
    /// </code>
    /// </example>
    public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> source, int pageNumber, int pageSize) {
        var items = source.ToList();
        var totalCount = items.Count;
        var pagedItems = items.Paginate(pageNumber, pageSize).ToList();

        return new PagedResult<T>(
            pagedItems,
            totalCount,
            pageNumber,
            pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize));
    }

    /// <summary>
    /// 将集合按指定大小分组
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="size">每组大小</param>
    /// <returns>分组后的集合</returns>
    public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int size) {
        if (size < 1) {
            throw new ArgumentException("分组大小必须大于0", nameof(size));
        }

        var batch = new List<T>(size);
        foreach (var item in source) {
            batch.Add(item);

            if (batch.Count == size) {
                yield return batch;
                batch = new List<T>(size);
            }
        }

        if (batch.Count > 0) {
            yield return batch;
        }
    }

    /// <summary>
    /// 去除集合中的 null 元素
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <returns>去除 null 后的集合</returns>
    /// <example>
    /// <code>
    /// var items = new List&lt;string?&gt; { "a", null, "b", null, "c" };
    /// var filtered = items.WhereNotNull().ToList(); // 返回：["a", "b", "c"]
    /// </code>
    /// </example>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class {
        return source.Where(x => x is not null)!;
    }

    /// <summary>
    /// 去除集合中的 null 元素（值类型）
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <returns>去除 null 后的集合</returns>
    /// <example>
    /// <code>
    /// var items = new List&lt;int?&gt; { 1, null, 2, null, 3 };
    /// var filtered = items.WhereNotNull().ToList(); // 返回：[1, 2, 3]
    /// </code>
    /// </example>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : struct {
        return source.Where(x => x.HasValue).Select(x => x!.Value);
    }

    /// <summary>
    /// 将集合转换为字典（忽略重复键）
    /// </summary>
    /// <typeparam name="TSource">源元素类型</typeparam>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="keySelector">键选择器</param>
    /// <returns>字典</returns>
    /// <example>
    /// <code>
    /// var items = new List&lt;(int Id, string Name)&gt;
    /// {
    ///     (1, "A"),
    ///     (2, "B"),
    ///     (1, "C") // 重复键，将被忽略
    /// };
    /// var dict = items.ToDictionarySafe(x => x.Id);
    /// // dict = { [1] = (1, "A"), [2] = (2, "B") }
    /// </code>
    /// </example>
    public static Dictionary<TKey, TSource> ToDictionarySafe<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector) where TKey : notnull {
        var dict = new Dictionary<TKey, TSource>();
        foreach (var item in source) {
            var key = keySelector(item);
            if (!dict.ContainsKey(key)) {
                dict[key] = item;
            }
        }

        return dict;
    }

    /// <summary>
    /// 将集合按指定键分组并转换为字典
    /// </summary>
    /// <typeparam name="TSource">源元素类型</typeparam>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="keySelector">键选择器</param>
    /// <returns>分组字典</returns>
    /// <example>
    /// <code>
    /// var items = new List&lt;(string Category, string Name)&gt;
    /// {
    ///     ("Fruit", "Apple"),
    ///     ("Fruit", "Banana"),
    ///     ("Vegetable", "Carrot")
    /// };
    /// var dict = items.ToLookupDictionary(x => x.Category);
    /// // dict = {
    /// //     ["Fruit"] = [("Fruit", "Apple"), ("Fruit", "Banana")],
    /// //     ["Vegetable"] = [("Vegetable", "Carrot")]
    /// // }
    /// </code>
    /// </example>
    public static Dictionary<TKey, List<TSource>> ToLookupDictionary<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector) where TKey : notnull {
        var dict = new Dictionary<TKey, List<TSource>>();
        foreach (var item in source) {
            var key = keySelector(item);
            if (!dict.TryGetValue(key, out var list)) {
                list = [];
                dict[key] = list;
            }

            list.Add(item);
        }

        return dict;
    }

    /// <summary>
    /// 将集合连接为字符串
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="separator">分隔符</param>
    /// <returns>连接后的字符串</returns>
    /// <example>
    /// <code>
    /// var items = new List&lt;int&gt; { 1, 2, 3 };
    /// var result = items.JoinToString(","); // 返回："1,2,3"
    /// var result2 = items.JoinToString("-"); // 返回："1-2-3"
    /// </code>
    /// </example>
    public static string JoinToString<T>(this IEnumerable<T> source, string separator = ",") {
        return string.Join(separator, source);
    }

    /// <summary>
    /// 将集合连接为字符串
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="separator">分隔符</param>
    /// <param name="selector">选择器</param>
    /// <returns>连接后的字符串</returns>
    /// <example>
    /// <code>
    /// var users = new List&lt;User&gt; { new User { Name = "Alice" }, new User { Name = "Bob" } };
    /// var result = users.JoinToString(", ", u => u.Name); // 返回："Alice, Bob"
    /// </code>
    /// </example>
    public static string JoinToString<T>(this IEnumerable<T> source, string separator, Func<T, string> selector) {
        return string.Join(separator, source.Select(selector));
    }

    /// <summary>
    /// 判断集合是否包含所有指定元素
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="items">要检查的元素</param>
    /// <returns>如果包含所有指定元素则返回 true</returns>
    /// <example>
    /// <code>
    /// var items = new List&lt;int&gt; { 1, 2, 3, 4, 5 };
    /// var containsAll = items.ContainsAll(1, 2, 3); // 返回：true
    /// var containsAll2 = items.ContainsAll(1, 6); // 返回：false
    /// </code>
    /// </example>
    public static bool ContainsAll<T>(this IEnumerable<T> source, params T[] items) {
        var set = new HashSet<T>(source);
        return items.All(set.Contains);
    }

    /// <summary>
    /// 判断集合是否包含任意指定元素
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="items">要检查的元素</param>
    /// <returns>如果包含任意指定元素则返回 true</returns>
    /// <example>
    /// <code>
    /// var items = new List&lt;int&gt; { 1, 2, 3, 4, 5 };
    /// var containsAny = items.ContainsAny(1, 6); // 返回：true
    /// var containsAny2 = items.ContainsAny(6, 7); // 返回：false
    /// </code>
    /// </example>
    public static bool ContainsAny<T>(this IEnumerable<T> source, params T[] items) {
        var set = new HashSet<T>(source);
        return items.Any(set.Contains);
    }

    /// <summary>
    /// 获取集合的随机元素
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <returns>随机元素</returns>
    /// <example>
    /// <code>
    /// var items = new List&lt;string&gt; { "a", "b", "c", "d", "e" };
    /// var randomItem = items.RandomElement(); // 随机返回其中一个元素
    /// </code>
    /// </example>
    public static T? RandomElement<T>(this IEnumerable<T> source) {
        var list = source as IList<T> ?? source.ToList();
        return list.Count == 0 ? default : list[Random.Shared.Next(list.Count)];
    }

    /// <summary>
    /// 打乱集合顺序
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <returns>打乱顺序后的集合</returns>
    /// <example>
    /// <code>
    /// var items = Enumerable.Range(1, 10);
    /// var shuffled = items.Shuffle().ToList(); // 随机打乱顺序，如：[3, 7, 1, 9, ...]
    /// </code>
    /// </example>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) {
        var list = source.ToList();
        for (var i = list.Count - 1; i > 0; i--) {
            var j = Random.Shared.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        return list;
    }
}

/// <summary>
/// 分页结果
/// </summary>
/// <typeparam name="T">元素类型</typeparam>
/// <param name="Items">当前页数据</param>
/// <param name="TotalCount">总记录数</param>
/// <param name="PageNumber">当前页码</param>
/// <param name="PageSize">每页大小</param>
/// <param name="TotalPages">总页数</param>
public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages) {
    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPrevious => PageNumber > 1;

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNext => PageNumber < TotalPages;
}
