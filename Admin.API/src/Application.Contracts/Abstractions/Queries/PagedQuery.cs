/*
 * 文件名称: DomainPagedQuery.cs
 * 功能描述: 通用分页查询，用于所有领域实体的分页获取操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Dtos;
using Domain.Shared.Dtos;

namespace Application.Contracts.Abstractions.Queries;

/// <summary>
/// 通用分页查询
/// <para>用于所有领域实体的分页获取操作，包括聚合根和关系表</para>
/// </summary>
/// <typeparam name="TQueryDto">查询参数 DTO 类型</typeparam>
/// <typeparam name="TPagedDto">响应DTO类型</typeparam>
/// <param name="QueryDto">查询参数 DTO</param>
public abstract class PagedQuery<TQueryDto, TPagedDto>(TQueryDto QueryDto)
    : Query<TQueryDto, PagedResponse<TPagedDto>>(QueryDto)
    where TQueryDto : QueryDto
    where TPagedDto : PagedDto
{
    /// <summary>
    /// 当前页面
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 分页大小
    /// </summary>
    public int Size { get; set; } = 10;
}
