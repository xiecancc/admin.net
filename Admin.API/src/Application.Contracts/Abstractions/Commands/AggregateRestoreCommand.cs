/*
 * 文件名称: AggregateRestoreCommand.cs
 * 功能描述: 聚合根恢复命令，用于聚合根实体的恢复操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Dtos;

namespace Application.Contracts.Abstractions.Commands;

/// <summary>
/// 聚合根恢复命令
/// <para>用于聚合根实体的恢复操作</para>
/// <para>聚合根实体支持软删除和恢复</para>
/// </summary>
/// <typeparam name="TActionDto">操作DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="data">实体ID列表</param>
public class AggregateRestoreCommand<TActionDto>(List<TActionDto> data)
    : Command<bool>
    where TActionDto : AggregateActionDto
{
    /// <summary>
    /// 要恢复的实体ID列表
    /// </summary>
    /// <value>要恢复的实体ID列表</value>
    public List<TActionDto> Data { get; set; } = data;
}
