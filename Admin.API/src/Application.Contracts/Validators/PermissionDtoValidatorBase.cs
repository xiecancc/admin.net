/*
 * 文件名称：PermissionDtoValidatorBase.cs
 * 功能描述：权限 DTO 验证器基类，提供权限相关 DTO 的通用验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

using System.Linq.Expressions;
using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// 权限 DTO 验证器基类
/// <para>提供权限相关 DTO 的通用验证规则</para>
/// </summary>
/// <typeparam name="T">要验证的 DTO 类型</typeparam>
/// <remarks>
/// <para>包含权限实体的公共字段验证：Name, Code, Type, Sort</para>
/// <para>适用于：Permission, ApiPermission, MenuPermission, ButtonPermission</para>
/// </remarks>
public abstract class PermissionDtoValidatorBase<T> : DtoValidatorBase<T> {
    /// <summary>
    /// 初始化权限验证规则
    /// </summary>
    protected PermissionDtoValidatorBase() {
        InitializePermissionRules();
    }

    /// <summary>
    /// 初始化权限公共验证规则
    /// </summary>
    protected virtual void InitializePermissionRules() {
    }

    /// <summary>
    /// 添加权限基础字段验证规则
    /// </summary>
    /// <param name="nameSelector">名称字段选择器</param>
    /// <param name="codeSelector">编码字段选择器</param>
    /// <param name="typeSelector">类型字段选择器</param>
    /// <param name="sortSelector">排序字段选择器</param>
    protected void AddPermissionRules(
        Expression<Func<T, string>> nameSelector,
        Expression<Func<T, string>> codeSelector,
        Expression<Func<T, int>> typeSelector,
        Expression<Func<T, int>> sortSelector) {
        _ = RuleFor(nameSelector)
            .NotEmpty().WithMessage("权限名称不能为空")
            .MaximumLength(50).WithMessage("权限名称长度不能超过 50 个字符");

        _ = RuleFor(codeSelector)
            .NotEmpty().WithMessage("权限编码不能为空")
            .MaximumLength(100).WithMessage("权限编码长度不能超过 100 个字符")
            .Matches(@"^[a-zA-Z0-9_:]+$").WithMessage("权限编码只能包含字母、数字、下划线和冒号");

        _ = RuleFor(typeSelector)
            .InclusiveBetween(1, 3).WithMessage("权限类型必须在 1-3 之间");

        _ = RuleFor(sortSelector)
            .GreaterThanOrEqualTo(0).WithMessage("排序值不能小于 0");
    }
}

/// <summary>
/// 权限创建 DTO 验证器基类
/// </summary>
/// <typeparam name="T">要验证的 DTO 类型</typeparam>
public abstract class PermissionCreateDtoValidatorBase<T> : PermissionDtoValidatorBase<T> {
    /// <summary>
    /// 初始化权限创建验证规则
    /// </summary>
    protected PermissionCreateDtoValidatorBase() : base() {
    }
}

/// <summary>
/// 权限更新 DTO 验证器基类
/// </summary>
/// <typeparam name="T">要验证的 DTO 类型</typeparam>
public abstract class PermissionUpdateDtoValidatorBase<T> : PermissionDtoValidatorBase<T> {
    /// <summary>
    /// 初始化权限更新验证规则
    /// </summary>
    protected PermissionUpdateDtoValidatorBase() : base() {
    }
}
