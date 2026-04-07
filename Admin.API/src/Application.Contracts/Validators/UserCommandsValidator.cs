/*
 * 文件名称：UserCommandsValidator.cs
 * 功能描述：用户相关命令验证器，包含用户创建、更新、删除、启用、禁用等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-06
 */

using Application.Contracts.Commands;
using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// 用户创建命令验证器
/// </summary>
public class UserCreateCommandValidator : DtoValidatorBase<UserCreateCommand> {
    /// <summary>
    /// 初始化用户创建命令验证器
    /// </summary>
    public UserCreateCommandValidator() {
        _ = RuleFor(x => x.Data)
            .NotEmpty().WithMessage("用户创建数据不能为空");

        _ = RuleForEach(x => x.Data)
            .SetValidator(new UserCreateDtoValidator());
    }
}

/// <summary>
/// 用户更新命令验证器
/// </summary>
public class UserUpdateCommandValidator : DtoValidatorBase<UserUpdateCommand> {
    /// <summary>
    /// 初始化用户更新命令验证器
    /// </summary>
    public UserUpdateCommandValidator() {
        _ = RuleFor(x => x.Data)
            .NotEmpty().WithMessage("用户更新数据不能为空");

        _ = RuleForEach(x => x.Data)
            .SetValidator(new UserUpdateDtoValidator());
    }
}

/// <summary>
/// 用户删除命令验证器
/// </summary>
public class UserDeleteCommandValidator : DtoValidatorBase<UserDeleteCommand> {
    /// <summary>
    /// 初始化用户删除命令验证器
    /// </summary>
    public UserDeleteCommandValidator() {
        _ = RuleFor(x => x.Data)
            .NotEmpty().WithMessage("用户删除数据不能为空");
    }
}

/// <summary>
/// 用户恢复命令验证器
/// </summary>
public class UserRestoreCommandValidator : DtoValidatorBase<UserRestoreCommand> {
    /// <summary>
    /// 初始化用户恢复命令验证器
    /// </summary>
    public UserRestoreCommandValidator() {
        _ = RuleFor(x => x.Data)
            .NotEmpty().WithMessage("用户恢复数据不能为空");
    }
}

/// <summary>
/// 用户启用命令验证器
/// </summary>
public class UserEnableCommandValidator : DtoValidatorBase<UserEnableCommand> {
    /// <summary>
    /// 初始化用户启用命令验证器
    /// </summary>
    public UserEnableCommandValidator() {
        _ = RuleFor(x => x.UserIds)
            .NotEmpty().WithMessage("用户ID列表不能为空");
    }
}

/// <summary>
/// 用户禁用命令验证器
/// </summary>
public class UserDisableCommandValidator : DtoValidatorBase<UserDisableCommand> {
    /// <summary>
    /// 初始化用户禁用命令验证器
    /// </summary>
    public UserDisableCommandValidator() {
        _ = RuleFor(x => x.UserIds)
            .NotEmpty().WithMessage("用户ID列表不能为空");
    }
}
