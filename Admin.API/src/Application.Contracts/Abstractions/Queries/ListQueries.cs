/*
 * 文件名称: DomainListQuery.cs
 * 功能描述: 通用列表查询，用于所有领域实体的列表获取操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Contracts.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;

namespace Application.Contracts.Abstractions.Queries;

/// <summary>
/// 通用列表查询
/// <para>用于所有领域实体的列表获取操作，包括聚合根和关系表</para>
/// <para>聚合根实体支持软删除和更新，关系表使用物理删除，不支持更新</para>
/// </summary>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TListDto">响应DTO类型</typeparam>
/// <typeparam name="TQueryParameters">查询参数类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public abstract class DomainListQuery<TDomain, TRepository, TListDto, TQueryParameters>(TQueryParameters queryParameters) : DomainQuery<TDomain, TRepository, List<TListDto>>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TListDto : DomainListDto
    where TQueryParameters : DomainQueryParameters<TDomain>, new() {
    /// <summary>
    /// 查询参数
    /// </summary>
    /// <value>查询参数对象</value>
    public TQueryParameters QueryParameters { get; set; } = queryParameters;
}
