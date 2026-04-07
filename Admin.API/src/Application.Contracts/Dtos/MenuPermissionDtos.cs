/*
 * 文件名称: MenuPermissionDtos.cs
 * 功能描述: 菜单权限相关数据传输对象，包含创建、更新、列表等模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

namespace Application.Contracts.Dtos;

/// <summary>
/// 菜单权限创建 DTO
/// <para>用于创建菜单权限</para>
/// </summary>
public class MenuPermissionCreateDto : PermissionCreateDto {
    /// <summary>
    /// 菜单路径
    /// </summary>
    /// <value>菜单的访问路径，长度不超过300个字符，可以为空</value>
    public string? Path {
        get; set;
    }

    /// <summary>
    /// 菜单图标
    /// </summary>
    /// <value>菜单的图标标识，长度不超过100个字符，可以为空</value>
    public string? Icon {
        get; set;
    }

    /// <summary>
    /// 组件路径
    /// </summary>
    /// <value>菜单对应的组件路径，长度不超过500个字符，可以为空</value>
    public string? Component {
        get; set;
    }

    /// <summary>
    /// 是否可见
    /// </summary>
    /// <value>菜单是否在前端显示，默认为 true</value>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 是否外部链接
    /// </summary>
    /// <value>是否为外部链接，默认为 false</value>
    public bool IsExternal { get; set; } = false;

    /// <summary>
    /// 是否缓存页面
    /// </summary>
    /// <value>是否缓存页面，默认为 true</value>
    public bool KeepAlive { get; set; } = true;

    /// <summary>
    /// 重定向地址
    /// </summary>
    /// <value>菜单的重定向地址，长度不超过500个字符，可以为空</value>
    public string? Redirect {
        get; set;
    }

    /// <summary>
    /// 元信息（JSON 格式，用于存储菜单的额外配置）
    /// </summary>
    /// <value>菜单的元信息，JSON 格式，可以为空</value>
    public string? Meta {
        get; set;
    }
}

/// <summary>
/// 菜单权限更新 DTO
/// <para>用于更新菜单权限</para>
/// </summary>
public class MenuPermissionUpdateDto : PermissionUpdateDto {
    /// <summary>
    /// 菜单路径
    /// </summary>
    /// <value>菜单的访问路径，长度不超过300个字符，可以为空</value>
    public string? Path {
        get; set;
    }

    /// <summary>
    /// 菜单图标
    /// </summary>
    /// <value>菜单的图标标识，长度不超过100个字符，可以为空</value>
    public string? Icon {
        get; set;
    }

    /// <summary>
    /// 组件路径
    /// </summary>
    /// <value>菜单对应的组件路径，长度不超过500个字符，可以为空</value>
    public string? Component {
        get; set;
    }

    /// <summary>
    /// 是否可见
    /// </summary>
    /// <value>菜单是否在前端显示</value>
    public bool IsVisible {
        get; set;
    }

    /// <summary>
    /// 是否外部链接
    /// </summary>
    /// <value>是否为外部链接</value>
    public bool IsExternal {
        get; set;
    }

    /// <summary>
    /// 是否缓存页面
    /// </summary>
    /// <value>是否缓存页面</value>
    public bool KeepAlive {
        get; set;
    }

    /// <summary>
    /// 重定向地址
    /// </summary>
    /// <value>菜单的重定向地址，长度不超过500个字符，可以为空</value>
    public string? Redirect {
        get; set;
    }

    /// <summary>
    /// 元信息（JSON 格式，用于存储菜单的额外配置）
    /// </summary>
    /// <value>菜单的元信息，JSON 格式，可以为空</value>
    public string? Meta {
        get; set;
    }
}

/// <summary>
/// 菜单权限列表 DTO
/// <para>用于列表展示</para>
/// </summary>
public class MenuPermissionListDto : PermissionListDto {
    /// <summary>
    /// 菜单路径
    /// </summary>
    /// <value>菜单的访问路径，长度不超过300个字符，可以为空</value>
    public string? Path {
        get; set;
    }

    /// <summary>
    /// 菜单图标
    /// </summary>
    /// <value>菜单的图标标识，长度不超过100个字符，可以为空</value>
    public string? Icon {
        get; set;
    }

    /// <summary>
    /// 组件路径
    /// </summary>
    /// <value>菜单对应的组件路径，长度不超过500个字符，可以为空</value>
    public string? Component {
        get; set;
    }

    /// <summary>
    /// 是否可见
    /// </summary>
    /// <value>菜单是否在前端显示</value>
    public bool IsVisible {
        get; set;
    }

    /// <summary>
    /// 是否外部链接
    /// </summary>
    /// <value>是否为外部链接</value>
    public bool IsExternal {
        get; set;
    }

    /// <summary>
    /// 是否缓存页面
    /// </summary>
    /// <value>是否缓存页面</value>
    public bool KeepAlive {
        get; set;
    }
}

/// <summary>
/// 菜单权限详情 DTO
/// <para>用于详细信息展示</para>
/// </summary>
public class MenuPermissionDetailDto : PermissionDetailDto {
    /// <summary>
    /// 菜单路径
    /// </summary>
    /// <value>菜单的访问路径，长度不超过300个字符，可以为空</value>
    public string? Path {
        get; set;
    }

    /// <summary>
    /// 菜单图标
    /// </summary>
    /// <value>菜单的图标标识，长度不超过100个字符，可以为空</value>
    public string? Icon {
        get; set;
    }

    /// <summary>
    /// 组件路径
    /// </summary>
    /// <value>菜单对应的组件路径，长度不超过500个字符，可以为空</value>
    public string? Component {
        get; set;
    }

    /// <summary>
    /// 是否可见
    /// </summary>
    /// <value>菜单是否在前端显示</value>
    public bool IsVisible {
        get; set;
    }

    /// <summary>
    /// 是否外部链接
    /// </summary>
    /// <value>是否为外部链接</value>
    public bool IsExternal {
        get; set;
    }

    /// <summary>
    /// 是否缓存页面
    /// </summary>
    /// <value>是否缓存页面</value>
    public bool KeepAlive {
        get; set;
    }

    /// <summary>
    /// 重定向地址
    /// </summary>
    /// <value>菜单的重定向地址，长度不超过500个字符，可以为空</value>
    public string? Redirect {
        get; set;
    }

    /// <summary>
    /// 元信息（JSON 格式，用于存储菜单的额外配置）
    /// </summary>
    /// <value>菜单的元信息，JSON 格式，可以为空</value>
    public string? Meta {
        get; set;
    }
}

/// <summary>
/// 菜单权限分页 DTO
/// <para>用于分页响应中的列表数据</para>
/// </summary>
public class MenuPermissionPagedDto : PermissionPagedDto {
    /// <summary>
    /// 菜单路径
    /// </summary>
    /// <value>菜单的访问路径，长度不超过300个字符，可以为空</value>
    public string? Path {
        get; set;
    }

    /// <summary>
    /// 菜单图标
    /// </summary>
    /// <value>菜单的图标标识，长度不超过100个字符，可以为空</value>
    public string? Icon {
        get; set;
    }

    /// <summary>
    /// 组件路径
    /// </summary>
    /// <value>菜单对应的组件路径，长度不超过500个字符，可以为空</value>
    public string? Component {
        get; set;
    }

    /// <summary>
    /// 是否可见
    /// </summary>
    /// <value>菜单是否在前端显示</value>
    public bool IsVisible {
        get; set;
    }
}

/// <summary>
/// 菜单权限操作 DTO
/// <para>用于简单操作的数据传输对象</para>
/// </summary>
public class MenuPermissionActionDto : PermissionActionDto {
}

/// <summary>
/// 菜单权限查询参数
/// <para>用于菜单权限列表查询的参数</para>
/// </summary>
public class MenuPermissionQueryParameters : AggregateQueryParameters<Domain.Entities.MenuPermission> {
    /// <summary>
    /// 权限编码
    /// </summary>
    /// <value>权限编码，用于模糊搜索</value>
    public string? Code {
        get; set;
    }

    /// <summary>
    /// 权限名称
    /// </summary>
    /// <value>权限名称，用于模糊搜索</value>
    public string? Name {
        get; set;
    }

    /// <summary>
    /// 菜单路径
    /// </summary>
    /// <value>菜单路径，用于模糊搜索</value>
    public string? Path {
        get; set;
    }

    /// <summary>
    /// 是否可见
    /// </summary>
    /// <value>是否可见，用于筛选</value>
    public bool? IsVisible {
        get; set;
    }

    /// <summary>
    /// 父权限ID
    /// </summary>
    /// <value>父权限ID，用于筛选</value>
    public Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 查询条件列表
    /// </summary>
    public override List<System.Linq.Expressions.Expression<System.Func<Domain.Entities.MenuPermission, bool>>> Predicates() {
        var predicates = base.Predicates();

        if (!string.IsNullOrWhiteSpace(Code)) {
            predicates.Add(p => p.Code.Contains(Code!));
        }

        if (!string.IsNullOrWhiteSpace(Name)) {
            predicates.Add(p => p.Name.Contains(Name!));
        }

        if (!string.IsNullOrWhiteSpace(Path)) {
            predicates.Add(p => p.Path != null && p.Path.Contains(Path!));
        }

        if (IsVisible.HasValue) {
            predicates.Add(p => p.IsVisible == IsVisible.Value);
        }

        if (ParentId.HasValue) {
            predicates.Add(p => p.ParentId == ParentId.Value);
        }

        return predicates;
    }
}
