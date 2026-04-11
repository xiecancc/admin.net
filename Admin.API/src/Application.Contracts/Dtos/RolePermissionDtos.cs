/*
 * 文件名称: RolePermissionDtos.cs
 * 功能描述: 角色权限关联关系DTO类，用于角色权限关系的数据传输
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

namespace Application.Contracts.Dtos;

/// <summary>
/// 角色权限关联创建 DTO
/// <para>用于创建角色权限关联关系的数据传输</para>
/// </summary>
public class RolePermissionCreateDto : DomainCreateDto {
    /// <summary>
    /// 角色 ID
    /// </summary>
    /// <value>角色的唯一标识符，不能为空</value>
    public Guid RoleId {
        get; set;
    }

    /// <summary>
    /// 权限 ID
    /// </summary>
    /// <value>权限的唯一标识符，不能为空</value>
    public Guid PermissionId {
        get; set;
    }
}

/// <summary>
/// 角色权限关联操作 DTO
/// <para>用于角色权限关联关系的操作（如删除）</para>
/// </summary>
public class RolePermissionActionDto : DomainActionDto {
    /// <summary>
    /// 角色 ID
    /// </summary>
    /// <value>角色的唯一标识符，不能为空</value>
    public Guid RoleId {
        get; set;
    }

    /// <summary>
    /// 权限 ID
    /// </summary>
    /// <value>权限的唯一标识符，不能为空</value>
    public Guid PermissionId {
        get; set;
    }
}

/// <summary>
/// 角色权限关联列表 DTO
/// <para>用于角色权限关联关系的列表展示</para>
/// </summary>
public class RolePermissionListDto : DomainListDto {
    /// <summary>
    /// 角色 ID
    /// </summary>
    /// <value>角色的唯一标识符</value>
    public Guid RoleId {
        get; set;
    }

    /// <summary>
    /// 权限 ID
    /// </summary>
    /// <value>权限的唯一标识符</value>
    public Guid PermissionId {
        get; set;
    }

    /// <summary>
    /// 角色名称
    /// </summary>
    /// <value>角色的名称，可能为空</value>
    public string? RoleName {
        get; set;
    }

    /// <summary>
    /// 权限名称
    /// </summary>
    /// <value>权限的名称，可能为空</value>
    public string? PermissionName {
        get; set;
    }

    /// <summary>
    /// 权限类型
    /// </summary>
    /// <value>权限的类型，可能为空</value>
    public string? PermissionType {
        get; set;
    }
}

/// <summary>
/// 角色权限关联分页 DTO
/// <para>用于角色权限关联关系的分页展示</para>
/// </summary>
public class RolePermissionPagedDto : DomainPagedDto {
    /// <summary>
    /// 角色 ID
    /// </summary>
    /// <value>角色的唯一标识符</value>
    public Guid RoleId {
        get; set;
    }

    /// <summary>
    /// 权限 ID
    /// </summary>
    /// <value>权限的唯一标识符</value>
    public Guid PermissionId {
        get; set;
    }

    /// <summary>
    /// 角色名称
    /// </summary>
    /// <value>角色的名称，可能为空</value>
    public string? RoleName {
        get; set;
    }

    /// <summary>
    /// 权限名称
    /// </summary>
    /// <value>权限的名称，可能为空</value>
    public string? PermissionName {
        get; set;
    }

    /// <summary>
    /// 权限类型
    /// </summary>
    /// <value>权限的类型，可能为空</value>
    public string? PermissionType {
        get; set;
    }
}
