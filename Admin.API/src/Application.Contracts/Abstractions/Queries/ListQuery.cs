/*
 * 文件名称: DomainListQuery.cs
 * 功能描述: 通用列表查询，用于所有领域实体的列表获取操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Dtos;

namespace Application.Contracts.Abstractions.Queries;

/// <summary>
/// 通用列表查询
/// <para>用于所有领域实体的列表获取操作，包括聚合根和关系表</para>
/// </summary>
/// <typeparam name="TQueryDto">查询参数 DTO 类型</typeparam>
/// <typeparam name="TListDto">响应DTO类型</typeparam>
/// <param name="QueryDto">查询参数 DTO</param>
public abstract class ListQuery<TQueryDto, TListDto>(TQueryDto QueryDto)
    : Query<TQueryDto, List<TListDto>>(QueryDto)
    where TQueryDto : QueryDto
    where TListDto : ListDto;
