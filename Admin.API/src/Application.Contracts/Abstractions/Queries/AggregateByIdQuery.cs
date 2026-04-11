/*
 * 文件名称: AggregateByIdQuery.cs
 * 功能描述: 聚合根详情查询，用于聚合根实体的详情获取操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions;
using Application.Contracts.Dtos;

namespace Application.Contracts.Abstractions.Queries;

/// <summary>
/// 聚合根详情查询
/// <para>用于聚合根实体的详情获取操作，包含主键 ID 属性</para>
/// </summary>
/// <typeparam name="TDetailDto">响应DTO类型</typeparam>
public abstract class AggregateByIdQuery<TDetailDto> : AggregateQuery<TDetailDto>
    where TDetailDto : AggregateDetailDto {

    /// <summary>
    /// 主键 ID
    /// </summary>
    public new Guid Id { get; set; }
}
