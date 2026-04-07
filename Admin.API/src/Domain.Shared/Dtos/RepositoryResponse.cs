namespace Domain.Shared.Dtos;

/// <summary>
/// 分页响应类
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="items">数据列表</param>
/// <param name="total">总记录数</param>
/// <param name="page">当前页码</param>
/// <param name="size">每页大小</param>
public class PagedResponse<T>(List<T> items, int total, int page, int size) {
    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> Items { get; set; } = items;

    /// <summary>
    /// 总记录数
    /// </summary>
    public int Total { get; set; } = total;

    /// <summary>
    /// 当前页码
    /// </summary>
    public int Page { get; set; } = page;

    /// <summary>
    /// 每页大小
    /// </summary>
    public int Size { get; set; } = size;

    /// <summary>
    /// 总页数
    /// </summary>
    public int Pages => (int)Math.Ceiling(Total / (double)Size);

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPrevious => Page > 1;

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNext => Page < Pages;
}
