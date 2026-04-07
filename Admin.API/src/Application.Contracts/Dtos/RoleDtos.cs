/*
 * 文件名称: RoleDtos.cs
 * 功能描述: 角色相关数据传输对象，包含创建、更新、列表等模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

namespace Application.Contracts.Dtos;

/// <summary>
/// 角色创建 DTO
/// <para>用于创建角色</para>
/// </summary>
public class RoleCreateDto : AggregateCreateDto {
    /// <summary>
    /// 角色名称
    /// </summary>
    /// <value>角色的名称，不能为空</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色代码
    /// </summary>
    /// <value>角色的代码，不能为空</value>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>角色的排序值，默认为 0</value>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>角色的状态，默认为 true</value>
    public bool Status { get; set; } = true;
}

/// <summary>
/// 角色更新 DTO
/// <para>用于更新角色</para>
/// </summary>
public class RoleUpdateDto : AggregateUpdateDto {
    /// <summary>
    /// 角色名称
    /// </summary>
    /// <value>角色的名称，不能为空</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色代码
    /// </summary>
    /// <value>角色的代码，不能为空</value>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>角色的排序值</value>
    public int Sort {
        get; set;
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>角色的状态</value>
    public bool Status {
        get; set;
    }
}

/// <summary>
/// 角色列表 DTO
/// <para>用于列表展示</para>
/// </summary>
public class RoleListDto : AggregateListDto {
    /// <summary>
    /// 角色名称
    /// </summary>
    /// <value>角色的名称</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色代码
    /// </summary>
    /// <value>角色的代码</value>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>角色的排序值</value>
    public int Sort {
        get; set;
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>角色的状态</value>
    public bool Status {
        get; set;
    }
}

/// <summary>
/// 角色详情 DTO
/// <para>用于详细信息展示</para>
/// </summary>
public class RoleDetailDto : AggregateDetailDto {
    /// <summary>
    /// 角色名称
    /// </summary>
    /// <value>角色的名称</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色代码
    /// </summary>
    /// <value>角色的代码</value>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>角色的排序值</value>
    public int Sort {
        get; set;
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>角色的状态</value>
    public bool Status {
        get; set;
    }

    /// <summary>
    /// 权限列表
    /// </summary>
    /// <value>角色拥有的权限 ID 列表</value>
    public List<Guid> PermissionIds { get; set; } = [];
}

/// <summary>
/// 角色分页 DTO
/// <para>用于分页响应中的列表数据</para>
/// </summary>
public class RolePagedDto : AggregatePagedDto {
    /// <summary>
    /// 角色名称
    /// </summary>
    /// <value>角色的名称</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色代码
    /// </summary>
    /// <value>角色的代码</value>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>角色的排序值</value>
    public int Sort {
        get; set;
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>角色的状态</value>
    public bool Status {
        get; set;
    }
}

/// <summary>
/// 角色操作 DTO
/// <para>用于简单操作的数据传输对象</para>
/// </summary>
public class RoleActionDto : AggregateActionDto {
}

/// <summary>
/// 角色查询参数
/// <para>用于角色列表查询的参数</para>
/// </summary>
public class RoleQueryParameters : AggregateQueryParameters<Domain.Entities.Role> {
    /// <summary>
    /// 角色编码
    /// </summary>
    /// <value>角色编码，用于模糊搜索</value>
    public string? Code {
        get; set;
    }

    /// <summary>
    /// 角色名称
    /// </summary>
    /// <value>角色名称，用于模糊搜索</value>
    public string? Name {
        get; set;
    }

    /// <summary>
    /// 父角色ID
    /// </summary>
    /// <value>父角色ID，用于筛选</value>
    public Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 查询条件列表
    /// </summary>
    public override List<System.Linq.Expressions.Expression<System.Func<Domain.Entities.Role, bool>>> Predicates() {
        var predicates = base.Predicates();

        if (!string.IsNullOrWhiteSpace(Code)) {
            predicates.Add(r => r.Code.Contains(Code!));
        }

        if (!string.IsNullOrWhiteSpace(Name)) {
            predicates.Add(r => r.Name.Contains(Name!));
        }

        if (ParentId.HasValue) {
            predicates.Add(r => r.ParentId == ParentId.Value);
        }

        return predicates;
    }
}
