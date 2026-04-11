/*
 * 文件名称：ApiPermissionCommandsValidator.cs
 * 功能描述：API权限相关命令验证器，包含API权限创建、更新、删除、恢复等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-06
 */

using Application.Contracts.Commands;
using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// API权限创建命令验证器
/// </summary>
public class ApiPermissionCreateCommandValidator : DtoValidatorBase<ApiPermissionCreateCommand> {
    /// <summary>
    /// 初始化API权限创建命令验证器
    /// </summary>
    public ApiPermissionCreateCommandValidator() {
        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("API权限创建数据不能为空");

        RuleForEach(x => x.Data)
            .SetValidator(new ApiPermissionCreateDtoValidator());
    }
}

/// <summary>
/// API权限更新命令验证器
/// </summary>
public class ApiPermissionUpdateCommandValidator : DtoValidatorBase<ApiPermissionUpdateCommand> {
    /// <summary>
    /// 初始化API权限更新命令验证器
    /// </summary>
    public ApiPermissionUpdateCommandValidator() {
        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("API权限更新数据不能为空");

        RuleForEach(x => x.Data)
            .SetValidator(new ApiPermissionUpdateDtoValidator());
    }
}

/// <summary>
/// API权限删除命令验证器
/// </summary>
public class ApiPermissionDeleteCommandValidator : DtoValidatorBase<ApiPermissionDeleteCommand> {
    /// <summary>
    /// 初始化API权限删除命令验证器
    /// </summary>
    public ApiPermissionDeleteCommandValidator() {
        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("API权限删除数据不能为空");
    }
}

/// <summary>
/// API权限恢复命令验证器
/// </summary>
public class ApiPermissionRestoreCommandValidator : DtoValidatorBase<ApiPermissionRestoreCommand> {
    /// <summary>
    /// 初始化API权限恢复命令验证器
    /// </summary>
    public ApiPermissionRestoreCommandValidator() {
        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("API权限恢复数据不能为空");
    }
}
