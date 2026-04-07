/*
 * 文件名称: DomainBase.cs
 * 功能描述: 领域模型基类，所有领域模型都应继承此类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

namespace Domain.Shared.Entities;

/// <summary>
/// 领域模型基类
/// <para>所有领域模型都应继承此类</para>
/// </summary>
/// <remarks>
/// <para>继承关系：</para>
/// <list type="bullet">
///   <item>DomainBase：基类</item>
///   <item>AggregateBase：聚合根基类，继承 DomainBase，包含 Id 和审计字段</item>
///   <item>实体表：继承 AggregateBase</item>
///   <item>关系表：直接继承 DomainBase（使用联合主键）</item>
/// </list>
/// </remarks>
public abstract class DomainBase {
}
