/*
 * 文件名称：AuthCommandsValidator.cs
 * 功能描述：认证相关命令验证器，包含登录、注册、刷新令牌、登出等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-06
 */

using Application.Contracts.Commands;
using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// 登录命令验证器
/// </summary>
public class LoginCommandValidator : DtoValidatorBase<LoginCommand> {
    /// <summary>
    /// 初始化登录命令验证器
    /// </summary>
    public LoginCommandValidator() {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式无效")
            .MaximumLength(100).WithMessage("邮箱长度不能超过 100 个字符");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MaximumLength(100).WithMessage("密码长度不能超过 100 个字符");
    }
}

/// <summary>
/// 注册命令验证器
/// </summary>
public class RegisterCommandValidator : DtoValidatorBase<RegisterCommand> {
    /// <summary>
    /// 初始化注册命令验证器
    /// </summary>
    public RegisterCommandValidator() {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式无效")
            .MaximumLength(100).WithMessage("邮箱长度不能超过 100 个字符");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(8).WithMessage("密码长度至少为 8 个字符")
            .MaximumLength(100).WithMessage("密码长度不能超过 100 个字符")
            .Matches(@"[A-Z]").WithMessage("密码必须包含至少一个大写字母")
            .Matches(@"[a-z]").WithMessage("密码必须包含至少一个小写字母")
            .Matches(@"[0-9]").WithMessage("密码必须包含至少一个数字");

        RuleFor(x => x.NickName)
            .MaximumLength(50).WithMessage("昵称长度不能超过 50 个字符")
            .When(x => !string.IsNullOrEmpty(x.NickName));

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("手机号长度不能超过 20 个字符")
            .Matches(@"^1[3-9]\d{9}$").WithMessage("手机号格式无效")
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}

/// <summary>
/// 刷新令牌命令验证器
/// </summary>
public class RefreshTokenCommandValidator : DtoValidatorBase<RefreshTokenCommand> {
    /// <summary>
    /// 初始化刷新令牌命令验证器
    /// </summary>
    public RefreshTokenCommandValidator() {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("访问令牌不能为空");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("刷新令牌不能为空");
    }
}

/// <summary>
/// 登出命令验证器
/// </summary>
public class LogoutCommandValidator : DtoValidatorBase<LogoutCommand> {
    /// <summary>
    /// 初始化登出命令验证器
    /// </summary>
    public LogoutCommandValidator() {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("令牌不能为空");
    }
}
