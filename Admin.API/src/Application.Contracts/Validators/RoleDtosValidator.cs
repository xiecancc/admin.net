/*
 * 文件名称：RoleDtosValidator.cs
 * 功能描述：角色相关 DTO 验证器，包含角色创建、更新、查询参数等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-11
 */

using System.Linq.Expressions;
using Application.Contracts.Dtos;
using Application.Contracts.Queries;
using FluentValidation;

namespace Application.Contracts.Validators;

#region 角色验证器基类

/// <summary>
/// 角色 DTO 验证器基类
/// <para>提供角色相关 DTO 的通用验证规则</para>
/// </summary>
/// <typeparam name="T">要验证的 DTO 类型</typeparam>
/// <remarks>
/// <para>包含角色实体的公共字段验证：Name, Code, Sort</para>
/// </remarks>
public abstract class RoleDtoValidatorBase<T> : DtoValidatorBase<T> {
    /// <summary>
    /// 初始化角色验证规则
    /// </summary>
    protected RoleDtoValidatorBase() {
    }

    /// <summary>
    /// 添加角色字段验证规则
    /// </summary>
    /// <param name="nameSelector">名称字段选择器</param>
    /// <param name="codeSelector">编码字段选择器</param>
    /// <param name="sortSelector">排序字段选择器</param>
    protected void AddRoleRules(
        Expression<Func<T, string>> nameSelector,
        Expression<Func<T, string>> codeSelector,
        Expression<Func<T, int>> sortSelector) {
        RuleFor(nameSelector)
            .NotEmpty().WithMessage("角色名称不能为空")
            .MaximumLength(50).WithMessage("角色名称长度不能超过 50 个字符");

        RuleFor(codeSelector)
            .NotEmpty().WithMessage("角色编码不能为空")
            .MaximumLength(50).WithMessage("角色编码长度不能超过 50 个字符")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("角色编码只能包含字母、数字和下划线");

        RuleFor(sortSelector)
            .GreaterThanOrEqualTo(0).WithMessage("排序值不能小于 0");
    }
}

/// <summary>
/// 角色创建 DTO 验证器基类
/// </summary>
/// <typeparam name="T">要验证的 DTO 类型</typeparam>
public abstract class RoleCreateDtoValidatorBase<T> : RoleDtoValidatorBase<T> {
    /// <summary>
    /// 初始化角色创建验证规则
    /// </summary>
    protected RoleCreateDtoValidatorBase() : base() {
    }
}

/// <summary>
/// 角色更新 DTO 验证器基类
/// </summary>
/// <typeparam name="T">要验证的 DTO 类型</typeparam>
public abstract class RoleUpdateDtoValidatorBase<T> : RoleDtoValidatorBase<T> {
    /// <summary>
    /// 初始化角色更新验证规则
    /// </summary>
    protected RoleUpdateDtoValidatorBase() : base() {
    }
}

#endregion

#region 角色具体验证器

/// <summary>
/// 角色创建 DTO 验证器
/// </summary>
public class RoleCreateDtoValidator : RoleCreateDtoValidatorBase<RoleCreateDto> {
    /// <summary>
    /// 初始化角色创建 DTO 验证器
    /// </summary>
    public RoleCreateDtoValidator() {
        AddRoleRules(x => x.Name, x => x.Code, x => x.Sort);
    }
}

/// <summary>
/// 角色更新 DTO 验证器
/// </summary>
public class RoleUpdateDtoValidator : RoleUpdateDtoValidatorBase<RoleUpdateDto> {
    /// <summary>
    /// 初始化角色更新 DTO 验证器
    /// </summary>
    public RoleUpdateDtoValidator() {
        AddRoleRules(x => x.Name, x => x.Code, x => x.Sort);
    }
}

/// <summary>
/// 角色操作 DTO 验证器
/// </summary>
public class RoleActionDtoValidator : DtoValidatorBase<RoleActionDto> {
    /// <summary>
    /// 初始化角色操作 DTO 验证器
    /// </summary>
    public RoleActionDtoValidator() {
    }
}

/// <summary>
/// 角色列表查询验证器
/// </summary>
public class RoleListQueryValidator : DtoValidatorBase<RoleListQuery> {
    /// <summary>
    /// 初始化角色列表查询验证器
    /// </summary>
    public RoleListQueryValidator() {
        RuleFor(x => x.QueryDto.Name)
            .MaximumLength(50).WithMessage("角色名称长度不能超过 50 个字符")
            .When(x => x.QueryDto != null && !string.IsNullOrEmpty(x.QueryDto.Name));

        RuleFor(x => x.QueryDto.Code)
            .MaximumLength(50).WithMessage("角色编码长度不能超过 50 个字符")
            .When(x => x.QueryDto != null && !string.IsNullOrEmpty(x.QueryDto.Code));
    }
}

/// <summary>
/// 角色分页查询验证器
/// </summary>
public class RolePagedQueryValidator : DtoValidatorBase<RolePagedQuery> {
    /// <summary>
    /// 初始化角色分页查询验证器
    /// </summary>
    public RolePagedQueryValidator() {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("页码必须大于等于 1");

        RuleFor(x => x.Size)
            .InclusiveBetween(1, 100).WithMessage("每页大小必须在 1 到 100 之间");
    }
}

/// <summary>
/// 角色查询参数 DTO 验证器
/// </summary>
public class RoleQueryDtoValidator : DtoValidatorBase<RoleQueryDto> {
    /// <summary>
    /// 初始化角色查询参数 DTO 验证器
    /// </summary>
    public RoleQueryDtoValidator() {
        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("角色编码长度不能超过 50 个字符")
            .When(x => !string.IsNullOrEmpty(x.Code));

        RuleFor(x => x.Name)
            .MaximumLength(50).WithMessage("角色名称长度不能超过 50 个字符")
            .When(x => !string.IsNullOrEmpty(x.Name));
    }
}

#endregion
