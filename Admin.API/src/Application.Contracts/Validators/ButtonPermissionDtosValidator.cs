/*
 * 文件名称：ButtonPermissionDtosValidator.cs
 * 功能描述：按钮权限相关 DTO 验证器，包含按钮权限创建、更新、查询参数等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

using Application.Contracts.Dtos;
using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// 按钮权限创建 DTO 验证器
/// </summary>
public class ButtonPermissionCreateDtoValidator : PermissionCreateDtoValidatorBase<ButtonPermissionCreateDto> {
    /// <summary>
    /// 初始化按钮权限创建 DTO 验证器
    /// </summary>
    public ButtonPermissionCreateDtoValidator() {
        AddPermissionRules(x => x.Name, x => x.Code, x => x.Type, x => x.Sort);
        AddButtonPermissionRules();
    }

    /// <summary>
    /// 添加按钮权限特有验证规则
    /// </summary>
    private void AddButtonPermissionRules() {
        RuleFor(x => x.ActionType)
            .NotEmpty().WithMessage("操作类型不能为空")
            .MaximumLength(50).WithMessage("操作类型长度不能超过 50 个字符");
    }
}

/// <summary>
/// 按钮权限更新 DTO 验证器
/// </summary>
public class ButtonPermissionUpdateDtoValidator : PermissionUpdateDtoValidatorBase<ButtonPermissionUpdateDto> {
    /// <summary>
    /// 初始化按钮权限更新 DTO 验证器
    /// </summary>
    public ButtonPermissionUpdateDtoValidator() {
        AddPermissionRules(x => x.Name, x => x.Code, x => x.Type, x => x.Sort);
        AddButtonPermissionRules();
    }

    /// <summary>
    /// 添加按钮权限特有验证规则
    /// </summary>
    private void AddButtonPermissionRules() {
        RuleFor(x => x.ActionType)
            .NotEmpty().WithMessage("操作类型不能为空")
            .MaximumLength(50).WithMessage("操作类型长度不能超过 50 个字符");
    }
}

/// <summary>
/// 按钮权限操作 DTO 验证器
/// </summary>
public class ButtonPermissionActionDtoValidator : DtoValidatorBase<ButtonPermissionActionDto> {
    /// <summary>
    /// 初始化按钮权限操作 DTO 验证器
    /// </summary>
    public ButtonPermissionActionDtoValidator() {
    }
}
