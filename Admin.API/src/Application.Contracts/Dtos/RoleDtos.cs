/*
 * 文件名称: RoleDtos.cs
 * 功能描述: 角色相关数据传输对象，包含创建、更新、列表等模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
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
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色代码
    /// </summary>
    /// <value>角色的代码，不能为空</value>
    public required string Code { get; set; } = string.Empty;

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
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色代码
    /// </summary>
    /// <value>角色的代码，不能为空</value>
    public required string Code { get; set; } = string.Empty;

    /// <summary>
    /// 父角色 ID
    /// </summary>
    /// <value>父角色的 ID，为空表示顶级角色</value>
    public Guid? ParentId { get; set; }

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
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色代码
    /// </summary>
    /// <value>角色的代码</value>
    public required string Code { get; set; } = string.Empty;

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
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色代码
    /// </summary>
    /// <value>角色的代码</value>
    public required string Code { get; set; } = string.Empty;

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
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色代码
    /// </summary>
    /// <value>角色的代码</value>
    public required string Code { get; set; } = string.Empty;

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
/// 角色查询参数 DTO
/// <para>用于角色查询的参数封装</para>
/// </summary>
public class RoleQueryDto : AggregateQueryDto {
    /// <summary>
    /// 角色代码
    /// </summary>
    /// <value>角色的代码，支持模糊查询</value>
    public string? Code { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    /// <value>角色的名称，支持模糊查询</value>
    public string? Name { get; set; }

    /// <summary>
    /// 父角色 ID
    /// </summary>
    /// <value>父角色的 ID</value>
    public Guid? ParentId { get; set; }
}
