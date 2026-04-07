/*
 * 文件名称：RoleCommandsValidator.cs
 * 功能描述：角色相关命令验证器，包含角色创建、更新、删除、恢复等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-06
 */

using Application.Contracts.Commands;
using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// 角色创建命令验证器
/// </summary>
public class RoleCreateCommandValidator : DtoValidatorBase<RoleCreateCommand> {
    /// <summary>
    /// 初始化角色创建命令验证器
    /// </summary>
    public RoleCreateCommandValidator() {
        _ = RuleFor(x => x.Data)
            .NotEmpty().WithMessage("角色创建数据不能为空");

        _ = RuleForEach(x => x.Data)
            .SetValidator(new RoleCreateDtoValidator());
    }
}

/// <summary>
/// 角色更新命令验证器
/// </summary>
public class RoleUpdateCommandValidator : DtoValidatorBase<RoleUpdateCommand> {
    /// <summary>
    /// 初始化角色更新命令验证器
    /// </summary>
    public RoleUpdateCommandValidator() {
        _ = RuleFor(x => x.Data)
            .NotEmpty().WithMessage("角色更新数据不能为空");

        _ = RuleForEach(x => x.Data)
            .SetValidator(new RoleUpdateDtoValidator());
    }
}

/// <summary>
/// 角色删除命令验证器
/// </summary>
public class RoleDeleteCommandValidator : DtoValidatorBase<RoleDeleteCommand> {
    /// <summary>
    /// 初始化角色删除命令验证器
    /// </summary>
    public RoleDeleteCommandValidator() {
        _ = RuleFor(x => x.Data)
            .NotEmpty().WithMessage("角色删除数据不能为空");
    }
}

/// <summary>
/// 角色恢复命令验证器
/// </summary>
public class RoleRestoreCommandValidator : DtoValidatorBase<RoleRestoreCommand> {
    /// <summary>
    /// 初始化角色恢复命令验证器
    /// </summary>
    public RoleRestoreCommandValidator() {
        _ = RuleFor(x => x.Data)
            .NotEmpty().WithMessage("角色恢复数据不能为空");
    }
}
