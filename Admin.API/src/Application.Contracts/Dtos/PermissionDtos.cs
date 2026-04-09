/*
 * 文件名称: PermissionDtos.cs
 * 功能描述: 权限相关数据传输对象，包含创建、更新、列表等模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

namespace Application.Contracts.Dtos;

/// <summary>
/// 权限创建 DTO
/// <para>用于创建权限</para>
/// </summary>
public class PermissionCreateDto : AggregateCreateDto {
    /// <summary>
    /// 权限名称
    /// </summary>
    /// <value>权限的名称，不能为空</value>
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// 权限代码
    /// </summary>
    /// <value>权限的代码，不能为空</value>
    public required string Code { get; set; } = string.Empty;

    /// <summary>
    /// 权限类型
    /// </summary>
    /// <value>权限的类型，1: 菜单权限, 2: 按钮权限, 3: API 权限</value>
    public int Type {
        get; set;
    }

    /// <summary>
    /// 父级 ID
    /// </summary>
    /// <value>父级权限的 ID，可以为空</value>
    public Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>权限的排序值，默认为 0</value>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>权限的状态，默认为 true</value>
    public bool Status { get; set; } = true;
}

/// <summary>
/// 权限更新 DTO
/// <para>用于更新权限</para>
/// </summary>
public class PermissionUpdateDto : AggregateUpdateDto {
    /// <summary>
    /// 权限名称
    /// </summary>
    /// <value>权限的名称，不能为空</value>
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// 权限代码
    /// </summary>
    /// <value>权限的代码，不能为空</value>
    public required string Code { get; set; } = string.Empty;

    /// <summary>
    /// 权限类型
    /// </summary>
    /// <value>权限的类型，1: 菜单权限, 2: 按钮权限, 3: API 权限</value>
    public int Type {
        get; set;
    }

    /// <summary>
    /// 父级 ID
    /// </summary>
    /// <value>父级权限的 ID，可以为空</value>
    public Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>权限的排序值</value>
    public int Sort {
        get; set;
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>权限的状态</value>
    public bool Status {
        get; set;
    }
}

/// <summary>
/// 权限列表 DTO
/// <para>用于列表展示</para>
/// </summary>
public class PermissionListDto : AggregateListDto {
    /// <summary>
    /// 权限名称
    /// </summary>
    /// <value>权限的名称</value>
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// 权限代码
    /// </summary>
    /// <value>权限的代码</value>
    public required string Code { get; set; } = string.Empty;

    /// <summary>
    /// 权限类型
    /// </summary>
    /// <value>权限的类型，1: 菜单权限, 2: 按钮权限, 3: API 权限</value>
    public int Type {
        get; set;
    }

    /// <summary>
    /// 父级 ID
    /// </summary>
    /// <value>父级权限的 ID，可以为空</value>
    public Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>权限的排序值</value>
    public int Sort {
        get; set;
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>权限的状态</value>
    public bool Status {
        get; set;
    }
}

/// <summary>
/// 权限详情 DTO
/// <para>用于详细信息展示</para>
/// </summary>
public class PermissionDetailDto : AggregateDetailDto {
    /// <summary>
    /// 权限名称
    /// </summary>
    /// <value>权限的名称</value>
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// 权限代码
    /// </summary>
    /// <value>权限的代码</value>
    public required string Code { get; set; } = string.Empty;

    /// <summary>
    /// 权限类型
    /// </summary>
    /// <value>权限的类型，1: 菜单权限, 2: 按钮权限, 3: API 权限</value>
    public int Type {
        get; set;
    }

    /// <summary>
    /// 父级 ID
    /// </summary>
    /// <value>父级权限的 ID，可以为空</value>
    public Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>权限的排序值</value>
    public int Sort {
        get; set;
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>权限的状态</value>
    public bool Status {
        get; set;
    }
}

/// <summary>
/// 权限分页 DTO
/// <para>用于分页响应中的列表数据</para>
/// </summary>
public class PermissionPagedDto : AggregatePagedDto {
    /// <summary>
    /// 权限名称
    /// </summary>
    /// <value>权限的名称</value>
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// 权限代码
    /// </summary>
    /// <value>权限的代码</value>
    public required string Code { get; set; } = string.Empty;

    /// <summary>
    /// 权限类型
    /// </summary>
    /// <value>权限的类型，1: 菜单权限, 2: 按钮权限, 3: API 权限</value>
    public int Type {
        get; set;
    }

    /// <summary>
    /// 父级 ID
    /// </summary>
    /// <value>父级权限的 ID，可以为空</value>
    public Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>权限的排序值</value>
    public int Sort {
        get; set;
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>权限的状态</value>
    public bool Status {
        get; set;
    }
}

/// <summary>
/// 权限操作 DTO
/// <para>用于简单操作的数据传输对象</para>
/// </summary>
public class PermissionActionDto : AggregateActionDto {
}
