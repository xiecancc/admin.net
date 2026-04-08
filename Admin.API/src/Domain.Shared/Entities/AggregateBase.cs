/*
 * 文件名称: AggregateBase.cs
 * 功能描述: 聚合根基类，继承自 DomainBase，包含 Id 和审计字段
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;

namespace Domain.Shared.Entities;

/// <summary>
/// 聚合根基类
/// <para>所有聚合根实体都应继承此类，包含 Id 和审计字段</para>
/// </summary>
/// <remarks>
/// <para>继承关系：</para>
/// <list type="bullet">
///   <item>继承自 DomainBase，拥有领域事件功能</item>
///   <item>实体表继承 AggregateBase</item>
///   <item>关系表直接继承 DomainBase（使用联合主键）</item>
/// </list>
/// <para>主要属性：</para>
/// <list type="bullet">
///   <item>Id：主键 ID（GUID）</item>
///   <item>IsDeleted：是否已删除（软删除标记）</item>
///   <item>CreatedAt：创建时间（UTC）</item>
///   <item>CreatedBy：创建人 ID</item>
///   <item>UpdatedAt：更新时间（UTC）</item>
///   <item>UpdatedBy：更新人 ID</item>
///   <item>DeletedAt：删除时间（UTC，仅软删除时填充）</item>
///   <item>DeletedBy：删除人 ID</item>
///   <item>Description：描述</item>
/// </list>
/// </remarks>
[SugarIndex("IX_AggregateBase_IsDeleted", nameof(IsDeleted), OrderByType.Asc)]
[SugarIndex("IX_AggregateBase_CreatedAt", nameof(CreatedAt), OrderByType.Asc)]
[SugarIndex("IX_AggregateBase_UpdatedAt", nameof(UpdatedAt), OrderByType.Asc)]
[SugarIndex("IX_AggregateBase_CreatedBy", nameof(CreatedBy), OrderByType.Asc)]
[SugarIndex("IX_AggregateBase_UpdatedBy", nameof(UpdatedBy), OrderByType.Asc)]
public abstract class AggregateBase : DomainBase {
    /// <summary>
    /// 主键 ID（GUID）
    /// </summary>
    /// <value>实体的唯一标识符，默认为 Guid.Empty</value>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "主键 ID")]
    public Guid Id { get; set; } = Guid.Empty;

    /// <summary>
    /// 是否已删除（软删除标记）
    /// </summary>
    /// <value>是否已删除，默认为 false</value>
    [SugarColumn(ColumnDescription = "是否已删除", IsNullable = false)]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 创建时间（UTC）
    /// </summary>
    /// <value>实体创建时间，默认为当前 UTC 时间</value>
    [SugarColumn(ColumnDescription = "创建时间", IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人 ID
    /// </summary>
    /// <value>创建人的 ID，可以为空</value>
    [SugarColumn(ColumnDescription = "创建人 ID", IsNullable = true)]
    public Guid? CreatedBy {
        get; set;
    }

    /// <summary>
    /// 更新时间（UTC）
    /// </summary>
    /// <value>实体更新时间，可以为空</value>
    [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
    public DateTime? UpdatedAt {
        get; set;
    }

    /// <summary>
    /// 更新人 ID
    /// </summary>
    /// <value>更新人的 ID，可以为空</value>
    [SugarColumn(ColumnDescription = "更新人 ID", IsNullable = true)]
    public Guid? UpdatedBy {
        get; set;
    }

    /// <summary>
    /// 删除时间（UTC，仅软删除时填充）
    /// </summary>
    /// <value>实体删除时间，可以为空</value>
    [SugarColumn(ColumnDescription = "删除时间", IsNullable = true)]
    public DateTime? DeletedAt {
        get; set;
    }

    /// <summary>
    /// 删除人 ID
    /// </summary>
    /// <value>删除人的 ID，可以为空</value>
    [SugarColumn(ColumnDescription = "删除人 ID", IsNullable = true)]
    public Guid? DeletedBy {
        get; set;
    }

    /// <summary>
    /// 描述
    /// </summary>
    /// <value>实体的描述信息，长度不超过500个字符，可以为空</value>
    [SugarColumn(ColumnDescription = "描述", Length = 500, IsNullable = true)]
    public string? Description {
        get; set;
    }
}
