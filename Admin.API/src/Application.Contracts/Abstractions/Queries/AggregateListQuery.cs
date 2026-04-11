/*
 * 文件名称: AggregateListQuery.cs
 * 功能描述: 聚合根列表查询，用于聚合根实体的列表获取操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Dtos;

namespace Application.Contracts.Abstractions.Queries;

/// <summary>
/// 聚合根列表查询
/// <para>用于聚合根实体的列表获取操作，包含通用查询属性</para>
/// </summary>
/// <typeparam name="TListDto">响应DTO类型</typeparam>
public abstract class AggregateListQuery<TListDto>
    : AggregateQuery<List<TListDto>>
    where TListDto : AggregateListDto
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    public Guid? Id
    {
        get; set;
    }
}
