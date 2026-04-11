/*
 * 文件名称: ApiPermissionDtos.cs
 * 功能描述: API权限相关数据传输对象，包含创建、更新、列表等模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

namespace Application.Contracts.Dtos;

/// <summary>
/// API 权限创建 DTO
/// <para>用于创建 API 权限</para>
/// </summary>
public class ApiPermissionCreateDto : PermissionCreateDto {
    /// <summary>
    /// HTTP 方法
    /// </summary>
    /// <value>HTTP 请求方法，如 GET、POST、PUT、DELETE 等，长度不超过10个字符，可以为空</value>
    public string? HttpMethod {
        get; set;
    }

    /// <summary>
    /// API 路径
    /// </summary>
    /// <value>API 的访问路径，长度不超过300个字符，不能为空</value>
    public required string ApiPath { get; set; } = string.Empty;

    /// <summary>
    /// 模块名称
    /// </summary>
    /// <value>API 所属的模块名称，长度不超过100个字符，可以为空</value>
    public string? ModuleName {
        get; set;
    }

    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限的 ID，可以为空</value>
    public Guid? MenuId {
        get; set;
    }
}

/// <summary>
/// API 权限更新 DTO
/// <para>用于更新 API 权限</para>
/// </summary>
public class ApiPermissionUpdateDto : PermissionUpdateDto {
    /// <summary>
    /// HTTP 方法
    /// </summary>
    /// <value>HTTP 请求方法，如 GET、POST、PUT、DELETE 等，长度不超过10个字符，可以为空</value>
    public string? HttpMethod {
        get; set;
    }

    /// <summary>
    /// API 路径
    /// </summary>
    /// <value>API 的访问路径，长度不超过300个字符，不能为空</value>
    public required string ApiPath { get; set; } = string.Empty;

    /// <summary>
    /// 模块名称
    /// </summary>
    /// <value>API 所属的模块名称，长度不超过100个字符，可以为空</value>
    public string? ModuleName {
        get; set;
    }

    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限的 ID，可以为空</value>
    public Guid? MenuId {
        get; set;
    }
}

/// <summary>
/// API 权限列表 DTO
/// <para>用于列表展示</para>
/// </summary>
public class ApiPermissionListDto : PermissionListDto {
    /// <summary>
    /// HTTP 方法
    /// </summary>
    /// <value>HTTP 请求方法，如 GET、POST、PUT、DELETE 等，长度不超过10个字符，可以为空</value>
    public string? HttpMethod {
        get; set;
    }

    /// <summary>
    /// API 路径
    /// </summary>
    /// <value>API 的访问路径，长度不超过300个字符，不能为空</value>
    public required string ApiPath { get; set; } = string.Empty;

    /// <summary>
    /// 模块名称
    /// </summary>
    /// <value>API 所属的模块名称，长度不超过100个字符，可以为空</value>
    public string? ModuleName {
        get; set;
    }

    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限的 ID，可以为空</value>
    public Guid? MenuId {
        get; set;
    }
}

/// <summary>
/// API 权限详情 DTO
/// <para>用于详细信息展示</para>
/// </summary>
public class ApiPermissionDetailDto : PermissionDetailDto {
    /// <summary>
    /// HTTP 方法
    /// </summary>
    /// <value>HTTP 请求方法，如 GET、POST、PUT、DELETE 等，长度不超过10个字符，可以为空</value>
    public string? HttpMethod {
        get; set;
    }

    /// <summary>
    /// API 路径
    /// </summary>
    /// <value>API 的访问路径，长度不超过300个字符，不能为空</value>
    public required string ApiPath { get; set; } = string.Empty;

    /// <summary>
    /// 模块名称
    /// </summary>
    /// <value>API 所属的模块名称，长度不超过100个字符，可以为空</value>
    public string? ModuleName {
        get; set;
    }

    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限的 ID，可以为空</value>
    public Guid? MenuId {
        get; set;
    }
}

/// <summary>
/// API 权限分页 DTO
/// <para>用于分页响应中的列表数据</para>
/// </summary>
public class ApiPermissionPagedDto : PermissionPagedDto {
    /// <summary>
    /// HTTP 方法
    /// </summary>
    /// <value>HTTP 请求方法，如 GET、POST、PUT、DELETE 等，长度不超过10个字符，可以为空</value>
    public string? HttpMethod {
        get; set;
    }

    /// <summary>
    /// API 路径
    /// </summary>
    /// <value>API 的访问路径，长度不超过300个字符，不能为空</value>
    public required string ApiPath { get; set; } = string.Empty;

    /// <summary>
    /// 模块名称
    /// </summary>
    /// <value>API 所属的模块名称，长度不超过100个字符，可以为空</value>
    public string? ModuleName {
        get; set;
    }

    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限的 ID，可以为空</value>
    public Guid? MenuId {
        get; set;
    }
}

/// <summary>
/// API 权限操作 DTO
/// <para>用于简单操作的数据传输对象</para>
/// </summary>
public class ApiPermissionActionDto : PermissionActionDto {
}
