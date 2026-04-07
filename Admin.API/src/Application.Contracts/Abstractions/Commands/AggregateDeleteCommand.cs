/*
 * 文件名称: DomainDeleteCommand.cs
 * 功能描述: 通用删除命令，用于所有领域实体的删除操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Contracts.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;

namespace Application.Contracts.Abstractions.Commands;

/// <summary>
/// 聚合根删除命令
/// <para>用于聚合根实体的删除操作</para>
/// <para>聚合根实体支持软删除</para>
/// </summary>
/// <typeparam name="TAggregate">聚合根类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TActionDto">操作DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="data">操作数据</param>
public class AggregateDeleteCommand<TAggregate, TRepository, TActionDto>(List<TActionDto> data) : DomainCommand<TAggregate, TRepository, bool>
    where TAggregate : AggregateBase, new()
    where TRepository : IAggregateRepository<TAggregate>
    where TActionDto : AggregateActionDto {
    /// <summary>
    /// 操作数据
    /// </summary>
    /// <value>删除操作的数据</value>
    public List<TActionDto> Data { get; set; } = data;
}
