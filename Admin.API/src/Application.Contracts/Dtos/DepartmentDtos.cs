/*
 * 文件名称: DepartmentDtos.cs
 * 功能描述: 部门相关数据传输对象，包含创建、更新、列表等模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

namespace Application.Contracts.Dtos;

/// <summary>
/// 部门创建 DTO
/// <para>用于创建部门</para>
/// </summary>
public class DepartmentCreateDto : AggregateCreateDto {
    /// <summary>
    /// 部门编码
    /// </summary>
    /// <value>部门的唯一编码，不能为空</value>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 部门名称
    /// </summary>
    /// <value>部门的名称，不能为空</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 父部门 ID
    /// </summary>
    /// <value>父部门的 ID，可以为空</value>
    public Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>部门的排序值，默认为 0</value>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 描述
    /// </summary>
    /// <value>部门的描述信息，可以为空</value>
    public string? Description {
        get; set;
    }
}

/// <summary>
/// 部门更新 DTO
/// <para>用于更新部门</para>
/// </summary>
public class DepartmentUpdateDto : AggregateUpdateDto {
    /// <summary>
    /// 部门编码
    /// </summary>
    /// <value>部门的唯一编码，不能为空</value>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 部门名称
    /// </summary>
    /// <value>部门的名称，不能为空</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 父部门 ID
    /// </summary>
    /// <value>父部门的 ID，可以为空</value>
    public Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>部门的排序值</value>
    public int Sort { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <value>部门的描述信息，可以为空</value>
    public string? Description {
        get; set;
    }
}

/// <summary>
/// 部门列表 DTO
/// <para>用于列表展示</para>
/// </summary>
public class DepartmentListDto : AggregateListDto {
    /// <summary>
    /// 部门编码
    /// </summary>
    /// <value>部门的唯一编码</value>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 部门名称
    /// </summary>
    /// <value>部门的名称</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 父部门 ID
    /// </summary>
    /// <value>父部门的 ID，可以为空</value>
    public Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 父部门名称
    /// </summary>
    /// <value>父部门的名称，可以为空</value>
    public string? ParentName {
        get; set;
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>部门的排序值</value>
    public int Sort { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <value>部门的描述信息，可以为空</value>
    public string? Description {
        get; set;
    }
}

/// <summary>
/// 部门详情 DTO
/// <para>用于详细信息展示</para>
/// </summary>
public class DepartmentDetailDto : AggregateDetailDto {
    /// <summary>
    /// 部门编码
    /// </summary>
    /// <value>部门的唯一编码</value>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 部门名称
    /// </summary>
    /// <value>部门的名称</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 父部门 ID
    /// </summary>
    /// <value>父部门的 ID，可以为空</value>
    public Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 父部门名称
    /// </summary>
    /// <value>父部门的名称，可以为空</value>
    public string? ParentName {
        get; set;
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>部门的排序值</value>
    public int Sort { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <value>部门的描述信息，可以为空</value>
    public string? Description {
        get; set;
    }

    /// <summary>
    /// 子部门列表
    /// </summary>
    /// <value>当前部门的子部门列表</value>
    public List<DepartmentListDto> Children { get; set; } = [];
}

/// <summary>
/// 部门分页 DTO
/// <para>用于分页响应中的列表数据</para>
/// </summary>
public class DepartmentPagedDto : AggregatePagedDto {
    /// <summary>
    /// 部门编码
    /// </summary>
    /// <value>部门的唯一编码</value>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 部门名称
    /// </summary>
    /// <value>部门的名称</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 父部门 ID
    /// </summary>
    /// <value>父部门的 ID，可以为空</value>
    public Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 父部门名称
    /// </summary>
    /// <value>父部门的名称，可以为空</value>
    public string? ParentName {
        get; set;
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>部门的排序值</value>
    public int Sort { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <value>部门的描述信息，可以为空</value>
    public string? Description {
        get; set;
    }
}

/// <summary>
/// 部门操作 DTO
/// <para>用于简单操作的数据传输对象</para>
/// </summary>
public class DepartmentActionDto : AggregateActionDto {
}

/// <summary>
/// 部门查询参数
/// <para>用于部门列表查询的参数</para>
/// </summary>
public class DepartmentQueryParameters : AggregateQueryParameters<Domain.Entities.Department> {
    /// <summary>
    /// 部门编码
    /// </summary>
    /// <value>部门编码，用于模糊搜索</value>
    public string? Code {
        get; set;
    }

    /// <summary>
    /// 部门名称
    /// </summary>
    /// <value>部门名称，用于模糊搜索</value>
    public string? Name {
        get; set;
    }

    /// <summary>
    /// 父部门 ID
    /// </summary>
    /// <value>父部门 ID，用于筛选</value>
    public Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 查询条件列表
    /// </summary>
    public override List<System.Linq.Expressions.Expression<System.Func<Domain.Entities.Department, bool>>> Predicates() {
        var predicates = base.Predicates();

        if (!string.IsNullOrWhiteSpace(Code)) {
            predicates.Add(d => d.Code.Contains(Code!));
        }

        if (!string.IsNullOrWhiteSpace(Name)) {
            predicates.Add(d => d.Name.Contains(Name!));
        }

        if (ParentId.HasValue) {
            predicates.Add(d => d.ParentId == ParentId.Value);
        }

        return predicates;
    }
}

/// <summary>
/// 用户部门角色 DTO
/// <para>用于用户部门角色关联</para>
/// </summary>
public class UserDepartmentRoleDto {
    /// <summary>
    /// 用户 ID
    /// </summary>
    /// <value>用户的 ID</value>
    public Guid UserId { get; set; }

    /// <summary>
    /// 部门 ID
    /// </summary>
    /// <value>部门的 ID</value>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// 角色 ID
    /// </summary>
    /// <value>角色的 ID</value>
    public Guid RoleId { get; set; }
}