/*
 * 文件名称: AggregateUpdateCommand.cs
 * 功能描述: 聚合根更新命令，用于聚合根实体的更新操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Dtos;

namespace Application.Contracts.Abstractions.Commands;

/// <summary>
/// 聚合根更新命令
/// <para>用于聚合根实体的更新操作</para>
/// <para>聚合根实体支持更新操作</para>
/// </summary>
/// <typeparam name="TUpdateDto">请求DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="data">请求数据</param>
public class AggregateUpdateCommand<TUpdateDto>(List<TUpdateDto> data)
    : Command<bool>
    where TUpdateDto : AggregateUpdateDto
{
    /// <summary>
    /// 请求数据
    /// </summary>
    /// <value>更新操作的请求数据</value>
    public List<TUpdateDto> Data { get; set; } = data;
}
