/*
 * 文件名称：ButtonPermissionCommandsValidator.cs
 * 功能描述：按钮权限相关命令验证器，包含按钮权限创建、更新、删除、恢复等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-06
 */

using Application.Contracts.Commands;
using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// 按钮权限创建命令验证器
/// </summary>
public class ButtonPermissionCreateCommandValidator : DtoValidatorBase<ButtonPermissionCreateCommand> {
    /// <summary>
    /// 初始化按钮权限创建命令验证器
    /// </summary>
    public ButtonPermissionCreateCommandValidator() {
        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("按钮权限创建数据不能为空");

        RuleForEach(x => x.Data)
            .SetValidator(new ButtonPermissionCreateDtoValidator());
    }
}

/// <summary>
/// 按钮权限更新命令验证器
/// </summary>
public class ButtonPermissionUpdateCommandValidator : DtoValidatorBase<ButtonPermissionUpdateCommand> {
    /// <summary>
    /// 初始化按钮权限更新命令验证器
    /// </summary>
    public ButtonPermissionUpdateCommandValidator() {
        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("按钮权限更新数据不能为空");

        RuleForEach(x => x.Data)
            .SetValidator(new ButtonPermissionUpdateDtoValidator());
    }
}

/// <summary>
/// 按钮权限删除命令验证器
/// </summary>
public class ButtonPermissionDeleteCommandValidator : DtoValidatorBase<ButtonPermissionDeleteCommand> {
    /// <summary>
    /// 初始化按钮权限删除命令验证器
    /// </summary>
    public ButtonPermissionDeleteCommandValidator() {
        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("按钮权限删除数据不能为空");
    }
}

/// <summary>
/// 按钮权限恢复命令验证器
/// </summary>
public class ButtonPermissionRestoreCommandValidator : DtoValidatorBase<ButtonPermissionRestoreCommand> {
    /// <summary>
    /// 初始化按钮权限恢复命令验证器
    /// </summary>
    public ButtonPermissionRestoreCommandValidator() {
        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("按钮权限恢复数据不能为空");
    }
}
