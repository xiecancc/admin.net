/*
 * 文件名称: Request.cs
 * 功能描述: 请求基类，所有命令和查询的基础类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using MediatR;

namespace Application.Contracts.Abstractions;

/// <summary>
/// 领域请求基类
/// 所有领域相关请求的基础类
/// </summary>
/// <typeparam name="TResponse">响应类型</typeparam>
public abstract class DomainRequest<TResponse> : IRequest<TResponse>;

/// <summary>
/// 查询基类
/// 所有查询的基础类，用于领域实体（包括关系表）的查询操作
/// </summary>
/// <typeparam name="TResponse">响应类型</typeparam>
public abstract class DomainQuery<TResponse> : DomainRequest<TResponse>;

/// <summary>
/// 领域命令基类
/// 所有命令的基础类，用于领域实体（包括关系表）的操作
/// </summary>
/// <typeparam name="TResponse">响应类型</typeparam>
public abstract class DomainCommand<TResponse> : DomainRequest<TResponse>;

/// <summary>
/// 聚合根查询基类
/// <para>用于聚合根实体的查询操作，包含通用查询属性</para>
/// </summary>
/// <typeparam name="TResponse">响应类型</typeparam>
public abstract class AggregateQuery<TResponse> : DomainQuery<TResponse> {

    /// <summary>
    /// 主键 ID
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 是否已删除（软删除标记）
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// 创建时间开始
    /// </summary>
    public DateTime? CreatedAtStart { get; set; }

    /// <summary>
    /// 创建时间结束
    /// </summary>
    public DateTime? CreatedAtEnd { get; set; }

    /// <summary>
    /// 更新时间开始
    /// </summary>
    public DateTime? UpdatedAtStart { get; set; }

    /// <summary>
    /// 更新时间结束
    /// </summary>
    public DateTime? UpdatedAtEnd { get; set; }

    /// <summary>
    /// 删除时间开始
    /// </summary>
    public DateTime? DeletedAtStart { get; set; }

    /// <summary>
    /// 删除时间结束
    /// </summary>
    public DateTime? DeletedAtEnd { get; set; }

    /// <summary>
    /// 创建人 ID
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新人 ID
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 删除人 ID
    /// </summary>
    public string? DeletedBy { get; set; }
}
