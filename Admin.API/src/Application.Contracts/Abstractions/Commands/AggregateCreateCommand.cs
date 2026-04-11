/*
 * 文件名称: DomainCreateCommands.cs
 * 功能描述: 通用创建命令，用于所有领域实体的创建操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Dtos;

namespace Application.Contracts.Abstractions.Commands;

/// <summary>
/// 聚合根创建命令
/// <para>用于聚合根实体的创建操作</para>
/// </summary>
/// <typeparam name="TCreateDto">请求DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="data">请求数据</param>
public class AggregateCreateCommand<TCreateDto>(List<TCreateDto> data)
    : CreateCommand<TCreateDto>(data)
    where TCreateDto : AggregateCreateDto
{
}
