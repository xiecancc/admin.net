/*
 * 文件名称: AggregateDtos.cs
 * 功能描述: 聚合根数据传输对象基类，包含基础模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

namespace Application.Contracts.Dtos;

/// <summary>
/// 聚合根创建 DTO
/// <para>用于所有创建操作的数据传输对象</para>
/// </summary>
public abstract class AggregateCreateDto : CreateDto;

/// <summary>
/// 聚合根更新 DTO
/// <para>用于所有更新操作的数据传输对象</para>
/// </summary>
public abstract class AggregateUpdateDto : UpdateDto {
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
public abstract class AggregateListDto : ListDto {
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
public abstract class AggregateDetailDto : DetailDto {
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
public abstract class AggregatePagedDto : PagedDto {
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
public abstract class AggregateActionDto : ActionDto {
    /// <summary>
    /// 主键 ID
    /// </summary>
    /// <value>实体的唯一标识符</value>
    public Guid Id { get; set; } = Guid.Empty;
}

/// <summary>
/// 聚合根查询参数 DTO 基类
/// <para>用于聚合根实体的查询参数封装，包含通用查询属性</para>
/// </summary>
public abstract class AggregateQueryDto : QueryDto
{
    /// <summary>
    /// 描述
    /// </summary>
    /// <value>描述信息，支持模糊查询</value>
    public string? Description { get; set; }

    /// <summary>
    /// 是否已删除（软删除标记）
    /// </summary>
    /// <value>是否已删除，用于筛选已删除或未删除的记录</value>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// 创建时间开始
    /// </summary>
    /// <value>创建时间范围的起始时间</value>
    public DateTime? CreatedAtStart { get; set; }

    /// <summary>
    /// 创建时间结束
    /// </summary>
    /// <value>创建时间范围的结束时间</value>
    public DateTime? CreatedAtEnd { get; set; }

    /// <summary>
    /// 更新时间开始
    /// </summary>
    /// <value>更新时间范围的起始时间</value>
    public DateTime? UpdatedAtStart { get; set; }

    /// <summary>
    /// 更新时间结束
    /// </summary>
    /// <value>更新时间范围的结束时间</value>
    public DateTime? UpdatedAtEnd { get; set; }

    /// <summary>
    /// 删除时间开始
    /// </summary>
    /// <value>删除时间范围的起始时间</value>
    public DateTime? DeletedAtStart { get; set; }

    /// <summary>
    /// 删除时间结束
    /// </summary>
    /// <value>删除时间范围的结束时间</value>
    public DateTime? DeletedAtEnd { get; set; }

    /// <summary>
    /// 创建人 ID
    /// </summary>
    /// <value>创建人的 ID</value>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新人 ID
    /// </summary>
    /// <value>更新人的 ID</value>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 删除人 ID
    /// </summary>
    /// <value>删除人的 ID</value>
    public string? DeletedBy { get; set; }
}
