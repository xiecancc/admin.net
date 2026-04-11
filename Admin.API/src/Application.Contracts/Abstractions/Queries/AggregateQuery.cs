/*
 * 文件名称: AggregateQuery.cs
 * 功能描述: 请求基类，所有命令和查询的基础类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

namespace Application.Contracts.Abstractions.Queries;

/// <summary>
/// 聚合根查询基类
/// <para>用于聚合根实体的查询操作，包含通用查询属性</para>
/// </summary>
/// <typeparam name="TResponse">响应类型</typeparam>
public abstract class AggregateQuery<TResponse>
    : Query<TResponse>
{
    /// <summary>
    /// 描述
    /// </summary>
    public string? Description
    {
        get; set;
    }

    /// <summary>
    /// 是否已删除（软删除标记）
    /// </summary>
    public bool? IsDeleted
    {
        get; set;
    }

    /// <summary>
    /// 创建时间开始
    /// </summary>
    public DateTime? CreatedAtStart
    {
        get; set;
    }

    /// <summary>
    /// 创建时间结束
    /// </summary>
    public DateTime? CreatedAtEnd
    {
        get; set;
    }

    /// <summary>
    /// 更新时间开始
    /// </summary>
    public DateTime? UpdatedAtStart
    {
        get; set;
    }

    /// <summary>
    /// 更新时间结束
    /// </summary>
    public DateTime? UpdatedAtEnd
    {
        get; set;
    }

    /// <summary>
    /// 删除时间开始
    /// </summary>
    public DateTime? DeletedAtStart
    {
        get; set;
    }

    /// <summary>
    /// 删除时间结束
    /// </summary>
    public DateTime? DeletedAtEnd
    {
        get; set;
    }

    /// <summary>
    /// 创建人 ID
    /// </summary>
    public string? CreatedBy
    {
        get; set;
    }

    /// <summary>
    /// 更新人 ID
    /// </summary>
    public string? UpdatedBy
    {
        get; set;
    }

    /// <summary>
    /// 删除人 ID
    /// </summary>
    public string? DeletedBy
    {
        get; set;
    }
}
