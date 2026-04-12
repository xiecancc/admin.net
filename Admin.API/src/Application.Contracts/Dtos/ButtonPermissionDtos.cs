/*
 * 文件名称: ButtonPermissionDtos.cs
 * 功能描述: 按钮权限相关数据传输对象，包含创建、更新、列表等模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

namespace Application.Contracts.Dtos;

/// <summary>
/// 按钮权限创建 DTO
/// <para>用于创建按钮权限</para>
/// </summary>
public class ButtonPermissionCreateDto : PermissionCreateDto {
    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限的 ID，可以为空</value>
    public Guid? MenuId {
        get; set;
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    /// <value>按钮的操作类型，长度不超过50个字符，不能为空</value>
    public string ActionType { get; set; } = string.Empty;
}

/// <summary>
/// 按钮权限更新 DTO
/// <para>用于更新按钮权限</para>
/// </summary>
public class ButtonPermissionUpdateDto : PermissionUpdateDto {
    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限的 ID，可以为空</value>
    public Guid? MenuId {
        get; set;
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    /// <value>按钮的操作类型，长度不超过50个字符，不能为空</value>
    public string ActionType { get; set; } = string.Empty;
}

/// <summary>
/// 按钮权限列表 DTO
/// <para>用于列表展示</para>
/// </summary>
public class ButtonPermissionListDto : PermissionListDto {
    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限的 ID，可以为空</value>
    public Guid? MenuId {
        get; set;
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    /// <value>按钮的操作类型，长度不超过50个字符，不能为空</value>
    public string ActionType { get; set; } = string.Empty;
}

/// <summary>
/// 按钮权限详情 DTO
/// <para>用于详细信息展示</para>
/// </summary>
public class ButtonPermissionDetailDto : PermissionDetailDto {
    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限的 ID，可以为空</value>
    public Guid? MenuId {
        get; set;
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    /// <value>按钮的操作类型，长度不超过50个字符，不能为空</value>
    public string ActionType { get; set; } = string.Empty;
}

/// <summary>
/// 按钮权限分页 DTO
/// <para>用于分页响应中的列表数据</para>
/// </summary>
public class ButtonPermissionPagedDto : PermissionPagedDto {
    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限的 ID，可以为空</value>
    public Guid? MenuId {
        get; set;
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    /// <value>按钮的操作类型，长度不超过50个字符，不能为空</value>
    public string ActionType { get; set; } = string.Empty;
}

/// <summary>
/// 按钮权限操作 DTO
/// <para>用于简单操作的数据传输对象</para>
/// </summary>
public class ButtonPermissionActionDto : PermissionActionDto {
}

/// <summary>
/// 按钮权限查询参数 DTO
/// <para>用于按钮权限查询的参数封装</para>
/// </summary>
public class ButtonPermissionQueryDto : PermissionQueryDto {
    /// <summary>
    /// 操作类型
    /// </summary>
    /// <value>按钮的操作类型</value>
    public string? ActionType { get; set; }

    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限的 ID</value>
    public Guid? MenuId { get; set; }
}
