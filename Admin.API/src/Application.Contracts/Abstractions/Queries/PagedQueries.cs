/*
 * 文件名称: DomainPagedQuery.cs
 * 功能描述: 通用分页查询，用于所有领域实体的分页获取操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Contracts.Dtos;
using Domain.Shared.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;

namespace Application.Contracts.Abstractions.Queries;

/// <summary>
/// 通用分页查询
/// <para>用于所有领域实体的分页获取操作，包括聚合根和关系表</para>
/// <para>聚合根实体支持软删除和更新，关系表使用物理删除，不支持更新</para>
/// </summary>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TPagedDto">响应DTO类型</typeparam>
/// <typeparam name="TQueryParameters">查询参数类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="queryParameters">查询参数</param>
public abstract class DomainPagedQuery<TDomain, TRepository, TPagedDto, TQueryParameters>(TQueryParameters queryParameters) : DomainQuery<TDomain, TRepository, PagedResponse<TPagedDto>>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TPagedDto : DomainPagedDto
    where TQueryParameters : DomainQueryParameters<TDomain>, new() {

    /// <summary>
    /// 当前页面
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 分页大小
    /// </summary>
    public int Size { get; set; } = 10;

    /// <summary>
    /// 查询参数
    /// </summary>
    /// <value>查询参数对象</value>
    public TQueryParameters QueryParameters { get; set; } = queryParameters;
}
