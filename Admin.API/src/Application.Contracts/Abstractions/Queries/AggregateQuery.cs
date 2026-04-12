/*
 * 文件名称: AggregateQuery.cs
 * 功能描述: 请求基类，所有命令和查询的基础类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Dtos;

namespace Application.Contracts.Abstractions.Queries;

/// <summary>
/// 聚合根查询基类
/// <para>用于聚合根实体的查询操作，包含通用查询属性</para>
/// </summary>
/// <typeparam name="TQueryDto">查询参数 DTO 类型</typeparam>
/// <typeparam name="TResponseDto">响应类型</typeparam>
/// <param name="QueryDto">查询参数 DTO</param>
public abstract class AggregateQuery<TQueryDto, TResponseDto>(TQueryDto QueryDto)
    : Query<TQueryDto, TResponseDto>(QueryDto)
    where TQueryDto : AggregateQueryDto;
