/*
 * 文件名称：MenuPermissionCommandsValidator.cs
 * 功能描述：菜单权限相关命令验证器，包含菜单权限创建、更新、删除、恢复等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-06
 */

using Application.Contracts.Commands;
using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// 菜单权限创建命令验证器
/// </summary>
public class MenuPermissionCreateCommandValidator : DtoValidatorBase<MenuPermissionCreateCommand> {
    /// <summary>
    /// 初始化菜单权限创建命令验证器
    /// </summary>
    public MenuPermissionCreateCommandValidator() {
        _ = RuleFor(x => x.Data)
            .NotEmpty().WithMessage("菜单权限创建数据不能为空");

        _ = RuleForEach(x => x.Data)
            .SetValidator(new MenuPermissionCreateDtoValidator());
    }
}

/// <summary>
/// 菜单权限更新命令验证器
/// </summary>
public class MenuPermissionUpdateCommandValidator : DtoValidatorBase<MenuPermissionUpdateCommand> {
    /// <summary>
    /// 初始化菜单权限更新命令验证器
    /// </summary>
    public MenuPermissionUpdateCommandValidator() {
        _ = RuleFor(x => x.Data)
            .NotEmpty().WithMessage("菜单权限更新数据不能为空");

        _ = RuleForEach(x => x.Data)
            .SetValidator(new MenuPermissionUpdateDtoValidator());
    }
}

/// <summary>
/// 菜单权限删除命令验证器
/// </summary>
public class MenuPermissionDeleteCommandValidator : DtoValidatorBase<MenuPermissionDeleteCommand> {
    /// <summary>
    /// 初始化菜单权限删除命令验证器
    /// </summary>
    public MenuPermissionDeleteCommandValidator() {
        _ = RuleFor(x => x.Data)
            .NotEmpty().WithMessage("菜单权限删除数据不能为空");
    }
}

/// <summary>
/// 菜单权限恢复命令验证器
/// </summary>
public class MenuPermissionRestoreCommandValidator : DtoValidatorBase<MenuPermissionRestoreCommand> {
    /// <summary>
    /// 初始化菜单权限恢复命令验证器
    /// </summary>
    public MenuPermissionRestoreCommandValidator() {
        _ = RuleFor(x => x.Data)
            .NotEmpty().WithMessage("菜单权限恢复数据不能为空");
    }
}
