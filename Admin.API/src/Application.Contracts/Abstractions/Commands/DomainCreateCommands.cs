/*
 * 文件名称: DomainCreateCommand.cs
 * 功能描述: 通用创建命令，用于所有领域实体的创建操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Contracts.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;

namespace Application.Contracts.Abstractions.Commands;

/// <summary>
/// 通用创建命令
/// <para>用于所有领域实体的创建操作，包括聚合根和关系表</para>
/// <para>聚合根实体支持软删除和更新，关系表使用物理删除，不支持更新</para>
/// </summary>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TCreateDto">请求DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="data">请求数据</param>
public class DomainCreateCommands<TDomain, TRepository, TCreateDto>(List<TCreateDto> data) : DomainCommand<TDomain, TRepository, bool>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TCreateDto : DomainCreateDto {
    /// <summary>
    /// 请求数据
    /// </summary>
    /// <value>创建操作的请求数据</value>
    public List<TCreateDto> Data { get; set; } = data;
}
