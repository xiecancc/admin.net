/*
 * 文件名称: AggregateDtos.cs
 * 功能描述: 聚合根数据传输对象基类，包含基础模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Domain.Shared.Entities;
using System.Linq.Expressions;
using SqlSugar;

namespace Application.Contracts.Dtos;

/// <summary>
/// 聚合根创建 DTO
/// <para>用于所有创建操作的数据传输对象</para>
/// </summary>
public abstract class AggregateCreateDto : DomainCreateDto {
    /// <summary>
    /// 主键 ID
    /// </summary>
    /// <value>实体的唯一标识符</value>
    public Guid Id { get; set; } = Guid.Empty;
}

/// <summary>
/// 聚合根更新 DTO
/// <para>用于所有更新操作的数据传输对象</para>
/// </summary>
public abstract class AggregateUpdateDto : DomainUpdateDto {
    /// <summary>
    /// 主键 ID
    /// </summary>
    /// <value>实体的唯一标识符</value>
    public Guid Id { get; set; } = Guid.Empty;
}

/// <summary>
/// 聚合根列表 DTO
/// <para>用于列表展示的数据传输对象</para>
/// </summary>
public abstract class AggregateListDto : DomainListDto {
    /// <summary>
    /// 主键 ID
    /// </summary>
    /// <value>实体的唯一标识符</value>
    public Guid Id { get; set; } = Guid.Empty;

    /// <summary>
    /// 是否已删除（软删除标记）
    /// </summary>
    /// <value>是否已删除</value>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <value>实体创建时间</value>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人 ID
    /// </summary>
    /// <value>创建人的 ID，可以为空</value>
    public Guid? CreatedBy {
        get; set;
    }

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <value>实体更新时间，可以为空</value>
    public DateTime? UpdatedAt {
        get; set;
    }

    /// <summary>
    /// 更新人 ID
    /// </summary>
    /// <value>更新人的 ID，可以为空</value>
    public Guid? UpdatedBy {
        get; set;
    }

    /// <summary>
    /// 删除时间
    /// </summary>
    /// <value>实体删除时间，可以为空</value>
    public DateTime? DeletedAt {
        get; set;
    }

    /// <summary>
    /// 删除人 ID
    /// </summary>
    /// <value>删除人的 ID，可以为空</value>
    public Guid? DeletedBy {
        get; set;
    }
}

/// <summary>
/// 聚合根详情 DTO
/// <para>用于详细信息展示的数据传输对象</para>
/// </summary>
public abstract class AggregateDetailDto : DomainDetailDto {
    /// <summary>
    /// 主键 ID
    /// </summary>
    /// <value>实体的唯一标识符</value>
    public Guid Id { get; set; } = Guid.Empty;

    /// <summary>
    /// 是否已删除（软删除标记）
    /// </summary>
    /// <value>是否已删除</value>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <value>实体创建时间</value>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人 ID
    /// </summary>
    /// <value>创建人的 ID，可以为空</value>
    public Guid? CreatedBy {
        get; set;
    }

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <value>实体更新时间，可以为空</value>
    public DateTime? UpdatedAt {
        get; set;
    }

    /// <summary>
    /// 更新人 ID
    /// </summary>
    /// <value>更新人的 ID，可以为空</value>
    public Guid? UpdatedBy {
        get; set;
    }

    /// <summary>
    /// 删除时间
    /// </summary>
    /// <value>实体删除时间，可以为空</value>
    public DateTime? DeletedAt {
        get; set;
    }

    /// <summary>
    /// 删除人 ID
    /// </summary>
    /// <value>删除人的 ID，可以为空</value>
    public Guid? DeletedBy {
        get; set;
    }
}

/// <summary>
/// 聚合根分页 DTO
/// <para>用于分页响应中的数据传输对象</para>
/// </summary>
public abstract class AggregatePagedDto : DomainPagedDto {
    /// <summary>
    /// 主键 ID
    /// </summary>
    /// <value>实体的唯一标识符</value>
    public Guid Id { get; set; } = Guid.Empty;

    /// <summary>
    /// 是否已删除（软删除标记）
    /// </summary>
    /// <value>是否已删除</value>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <value>实体创建时间</value>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人 ID
    /// </summary>
    /// <value>创建人的 ID，可以为空</value>
    public Guid? CreatedBy {
        get; set;
    }

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <value>实体更新时间，可以为空</value>
    public DateTime? UpdatedAt {
        get; set;
    }

    /// <summary>
    /// 更新人 ID
    /// </summary>
    /// <value>更新人的 ID，可以为空</value>
    public Guid? UpdatedBy {
        get; set;
    }

    /// <summary>
    /// 删除时间
    /// </summary>
    /// <value>实体删除时间，可以为空</value>
    public DateTime? DeletedAt {
        get; set;
    }

    /// <summary>
    /// 删除人 ID
    /// </summary>
    /// <value>删除人的 ID，可以为空</value>
    public Guid? DeletedBy {
        get; set;
    }
}

/// <summary>
/// 聚合根操作 DTO
/// <para>用于简单操作的数据传输对象</para>
/// </summary>
public class AggregateActionDto : DomainActionDto {
    /// <summary>
    /// 主键 ID
    /// </summary>
    /// <value>实体的唯一标识符</value>
    public Guid Id { get; set; } = Guid.Empty;
}

/// <summary>
/// 聚合根查询参数 DTO
/// <para>用于所有聚合根实体的查询参数</para>
/// </summary>
/// <typeparam name="TAggregate">聚合根类型</typeparam>
public class AggregateQueryParameters<TAggregate> : DomainQueryParameters<TAggregate> where TAggregate : AggregateBase, new() {
    /// <summary>
    /// 主键 ID
    /// </summary>
    public Guid? Id {
        get; set;
    }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description {
        get; set;
    }

    /// <summary>
    /// 是否已删除（软删除标记）
    /// </summary>
    public bool? IsDeleted {
        get; set;
    }

    /// <summary>
    /// 创建时间开始
    /// </summary>
    public DateTime? CreatedAtStart {
        get; set;
    }

    /// <summary>
    /// 创建时间结束
    /// </summary>
    public DateTime? CreatedAtEnd {
        get; set;
    }

    /// <summary>
    /// 更新时间开始
    /// </summary>
    public DateTime? UpdatedAtStart {
        get; set;
    }

    /// <summary>
    /// 更新时间结束
    /// </summary>
    public DateTime? UpdatedAtEnd {
        get; set;
    }

    /// <summary>
    /// 删除时间开始
    /// </summary>
    public DateTime? DeletedAtStart {
        get; set;
    }

    /// <summary>
    /// 删除时间结束
    /// </summary>
    public DateTime? DeletedAtEnd {
        get; set;
    }

    /// <summary>
    /// 创建人 ID
    /// </summary>
    public string? CreatedBy {
        get; set;
    }

    /// <summary>
    /// 更新人 ID
    /// </summary>
    public string? UpdatedBy {
        get; set;
    }

    /// <summary>
    /// 删除人 ID
    /// </summary>
    public string? DeletedBy {
        get; set;
    }

    /// <summary>
    /// 排序字段列表
    /// </summary>
    public override IDictionary<Expression<Func<TAggregate, object>>, OrderByType> Orders() {
        var orders = base.Orders();
        orders.Add(t => t.CreatedAt, OrderByType.Desc);
        return orders;
    }

    /// <summary>
    /// 查询条件列表
    /// </summary>
    public override List<Expression<Func<TAggregate, bool>>> Predicates() {
        var predicates = base.Predicates();
        if (Id.HasValue) {
            predicates.Add(t => t.Id == Id.Value);
        }
        if (!string.IsNullOrWhiteSpace(Description)) {
            predicates.Add(t => t.Description != null && t.Description.Contains(Description));
        }
        if (IsDeleted.HasValue) {
            predicates.Add(t => t.IsDeleted == IsDeleted.Value);
        }
        if (CreatedAtStart.HasValue) {
            predicates.Add(t => t.CreatedAt >= CreatedAtStart.Value);
        }
        if (CreatedAtEnd.HasValue) {
            predicates.Add(t => t.CreatedAt <= CreatedAtEnd.Value);
        }
        if (UpdatedAtStart.HasValue) {
            predicates.Add(t => t.UpdatedAt != null && t.UpdatedAt >= UpdatedAtStart.Value);
        }
        if (UpdatedAtEnd.HasValue) {
            predicates.Add(t => t.UpdatedAt != null && t.UpdatedAt <= UpdatedAtEnd.Value);
        }
        if (DeletedAtStart.HasValue) {
            predicates.Add(t => t.DeletedAt != null && t.DeletedAt >= DeletedAtStart.Value);
        }
        if (DeletedAtEnd.HasValue) {
            predicates.Add(t => t.DeletedAt != null && t.DeletedAt <= DeletedAtEnd.Value);
        }
        if (!string.IsNullOrWhiteSpace(CreatedBy)) {
            predicates.Add(t => t.CreatedBy != null && t.CreatedBy.Value.ToString().Contains(CreatedBy));
        }
        if (!string.IsNullOrWhiteSpace(UpdatedBy)) {
            predicates.Add(t => t.UpdatedBy != null && t.UpdatedBy.Value.ToString().Contains(UpdatedBy));
        }
        if (!string.IsNullOrWhiteSpace(DeletedBy)) {
            predicates.Add(t => t.DeletedBy != null && t.DeletedBy.Value.ToString().Contains(DeletedBy));
        }
        return predicates;
    }

    /// <summary>
    /// 生成缓存键的参数部分
    /// <para>将所有非空参数使用 | 连接返回字符串</para>
    /// </summary>
    /// <returns>缓存键参数部分</returns>
    public override string CacheKey() {
        var parts = new List<string>();

        if (Id.HasValue) {
            parts.Add($"Id={Id.Value}");
        }
        if (!string.IsNullOrWhiteSpace(Description)) {
            parts.Add($"Description={Description}");
        }
        if (IsDeleted.HasValue) {
            parts.Add($"IsDeleted={IsDeleted.Value}");
        }
        if (CreatedAtStart.HasValue) {
            parts.Add($"CreatedAtStart={CreatedAtStart.Value:yyyy-MM-dd HH:mm:ss}");
        }
        if (CreatedAtEnd.HasValue) {
            parts.Add($"CreatedAtEnd={CreatedAtEnd.Value:yyyy-MM-dd HH:mm:ss}");
        }
        if (UpdatedAtStart.HasValue) {
            parts.Add($"UpdatedAtStart={UpdatedAtStart.Value:yyyy-MM-dd HH:mm:ss}");
        }
        if (UpdatedAtEnd.HasValue) {
            parts.Add($"UpdatedAtEnd={UpdatedAtEnd.Value:yyyy-MM-dd HH:mm:ss}");
        }
        if (DeletedAtStart.HasValue) {
            parts.Add($"DeletedAtStart={DeletedAtStart.Value:yyyy-MM-dd HH:mm:ss}");
        }
        if (DeletedAtEnd.HasValue) {
            parts.Add($"DeletedAtEnd={DeletedAtEnd.Value:yyyy-MM-dd HH:mm:ss}");
        }
        if (!string.IsNullOrWhiteSpace(CreatedBy)) {
            parts.Add($"CreatedBy={CreatedBy}");
        }
        if (!string.IsNullOrWhiteSpace(UpdatedBy)) {
            parts.Add($"UpdatedBy={UpdatedBy}");
        }
        if (!string.IsNullOrWhiteSpace(DeletedBy)) {
            parts.Add($"DeletedBy={DeletedBy}");
        }

        return string.Join("|", parts);
    }
}
