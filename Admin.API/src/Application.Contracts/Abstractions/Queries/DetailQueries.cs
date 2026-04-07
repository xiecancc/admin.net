/*
 * 文件名称: DomainDetailQuery.cs
 * 功能描述: 通用详情查询，用于所有领域实体的详情获取操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Contracts.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;

namespace Application.Contracts.Abstractions.Queries;

/// <summary>
/// 通用详情查询
/// <para>用于所有领域实体的详情获取操作，包括聚合根和关系表</para>
/// <para>聚合根实体支持软删除和更新，关系表使用物理删除，不支持更新</para>
/// </summary>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TDetailDto">响应DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
public abstract class DomainDetailQuery<TDomain, TRepository, TDetailDto> : DomainQuery<TDomain, TRepository, TDetailDto>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TDetailDto : DomainDetailDto {
}


/// <summary>
/// 聚合根根据ID查询
/// <para>用于聚合根实体的根据ID获取操作</para>
/// <para>聚合根实体支持软删除和更新</para>
/// </summary>
/// <typeparam name="TAggregate">聚合根类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TDetailDto">响应DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="id">聚合根ID</param>
public abstract class AggregateByIdQuery<TAggregate, TRepository, TDetailDto>(Guid id) : DomainDetailQuery<TAggregate, TRepository, TDetailDto>
    where TAggregate : AggregateBase, new()
    where TRepository : IAggregateRepository<TAggregate>
    where TDetailDto : AggregateDetailDto {
    /// <summary>
    /// 聚合根ID
    /// </summary>
    /// <value>聚合根的唯一标识符</value>
    public Guid Id { get; set; } = id;
}
