/*
 * 文件名称: AggregatePagedQuery.cs
 * 功能描述: 聚合根分页查询，用于聚合根实体的分页获取操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Dtos;
using Domain.Shared.Dtos;

namespace Application.Contracts.Abstractions.Queries;

/// <summary>
/// 聚合根分页查询
/// <para>用于聚合根实体的分页获取操作，包含通用查询属性和分页属性</para>
/// </summary>
/// <typeparam name="TPagedDto">响应DTO类型</typeparam>
public abstract class AggregatePagedQuery<TPagedDto>
    : AggregateQuery<PagedResponse<TPagedDto>>
    where TPagedDto : AggregatePagedDto
{
    /// <summary>
    /// 主键 ID（支持模糊查询）
    /// </summary>
    public string? Id
    {
        get; set;
    }

    /// <summary>
    /// 当前页面
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 分页大小
    /// </summary>
    public int Size { get; set; } = 10;
}
