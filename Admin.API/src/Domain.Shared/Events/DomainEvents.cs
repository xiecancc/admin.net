using Domain.Shared.Entities;

namespace Domain.Shared.Events;

/// <summary>
/// 领域事件基类
/// 所有领域事件都应继承此类，包含事件的基本信息
/// </summary>
public abstract class DomainEvent {
    /// <summary>
    /// 事件 ID
    /// </summary>
    public Guid EventId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 事件发生时间（UTC）
    /// </summary>
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 事件描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// 基于领域模型集合的事件抽象基类
/// 适用于包含完整领域模型的事件场景
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
public abstract class DomainEventWithDomains<TDomain> : DomainEvent where TDomain : DomainBase {
    /// <summary>
    /// 领域模型集合
    /// </summary>
    public IEnumerable<TDomain> Domains {
        get;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="domains">领域模型集合</param>
    /// <param name="action">操作类型（如"创建"、"更新"）</param>
    /// <param name="entityName">实体中文名称（可选，默认为类型名）</param>
    protected DomainEventWithDomains(IEnumerable<TDomain> domains, string action, string? entityName = null) {
        Domains = domains;
        var name = entityName ?? typeof(TDomain).Name;
        Description = $"{action} {domains.Count()} 个{name}";
    }
}

/// <summary>
/// 领域模型创建事件
/// 包含完整的领域模型数据，用于需要访问实体全部信息的场景
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="domains">领域模型集合</param>
/// <param name="entityName">实体中文名称（可选，默认为类型名）</param>
public class DomainCreatedEvent<TDomain>(IEnumerable<TDomain> domains, string? entityName = null) : DomainEventWithDomains<TDomain>(domains, "创建了", entityName) where TDomain : DomainBase {
}

/// <summary>
/// 领域模型更新事件
/// 包含完整的领域模型数据，用于需要访问更新后实体全部信息的场景
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="domains">领域模型集合</param>
/// <param name="entityName">实体中文名称（可选，默认为类型名）</param>
public class DomainUpdatedEvent<TDomain>(IEnumerable<TDomain> domains, string? entityName = null) : DomainEventWithDomains<TDomain>(domains, "更新了", entityName) where TDomain : DomainBase {
}

/// <summary>
/// 领域模型软删除事件
/// 包含完整的领域模型数据，用于需要访问实体全部信息的场景
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="domains">领域模型集合</param>
/// <param name="entityName">实体中文名称（可选，默认为类型名）</param>
public class DomainDeletedEvent<TDomain>(IEnumerable<TDomain> domains, string? entityName = null) : DomainEventWithDomains<TDomain>(domains, "软删除了", entityName) where TDomain : DomainBase {
}

/// <summary>
/// 领域模型恢复事件
/// 包含完整的领域模型数据，用于需要访问实体全部信息的场景
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="domains">领域模型集合</param>
/// <param name="entityName">实体中文名称（可选，默认为类型名）</param>
public class DomainRestoredEvent<TDomain>(IEnumerable<TDomain> domains, string? entityName = null) : DomainEventWithDomains<TDomain>(domains, "恢复了", entityName) where TDomain : DomainBase {
}
