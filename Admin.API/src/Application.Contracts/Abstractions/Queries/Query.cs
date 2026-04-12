/*
 * 文件名称: DomainQuery.cs
 * 功能描述: 请求基类，所有命令和查询的基础类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Dtos;

namespace Application.Contracts.Abstractions.Queries;

/// <summary>
/// 查询基类
/// 所有查询的基础类，用于领域实体（包括关系表）的查询操作
/// </summary>
/// <typeparam name="TQueryDto">查询参数 DTO 类型</typeparam>
/// <typeparam name="TResponseDto">响应类型</typeparam>
/// <param name="QueryDto">查询参数 DTO</param>
public abstract class Query<TQueryDto, TResponseDto>(TQueryDto QueryDto) : Request<TResponseDto>
    where TQueryDto : QueryDto
{
    /// <summary>
    /// 查询参数 DTO
    /// </summary>
    public TQueryDto QueryDto { get; } = QueryDto;
}
