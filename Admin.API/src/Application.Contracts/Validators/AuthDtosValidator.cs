/*
 * 文件名称：AuthDtosValidator.cs
 * 功能描述：认证相关 DTO 验证器，包含登录、注册、刷新令牌等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

using Application.Contracts.Dtos;
using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// 登录请求 DTO 验证器
/// </summary>
public class LoginRequestDTOValidator : DtoValidatorBase<LoginRequestDTO> {
    /// <summary>
    /// 初始化登录请求 DTO 验证器
    /// </summary>
    public LoginRequestDTOValidator() {
        _ = RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式无效")
            .MaximumLength(100).WithMessage("邮箱长度不能超过 100 个字符");

        _ = RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MaximumLength(100).WithMessage("密码长度不能超过 100 个字符");
    }
}

/// <summary>
/// 注册请求 DTO 验证器
/// </summary>
public class RegisterRequestDTOValidator : DtoValidatorBase<RegisterRequestDTO> {
    /// <summary>
    /// 初始化注册请求 DTO 验证器
    /// </summary>
    public RegisterRequestDTOValidator() {
        _ = RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式无效")
            .MaximumLength(100).WithMessage("邮箱长度不能超过 100 个字符");

        _ = RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(8).WithMessage("密码长度至少为 8 个字符")
            .MaximumLength(100).WithMessage("密码长度不能超过 100 个字符")
            .Matches(@"[A-Z]").WithMessage("密码必须包含至少一个大写字母")
            .Matches(@"[a-z]").WithMessage("密码必须包含至少一个小写字母")
            .Matches(@"[0-9]").WithMessage("密码必须包含至少一个数字");

        _ = RuleFor(x => x.NickName)
            .MaximumLength(50).WithMessage("昵称长度不能超过 50 个字符")
            .When(x => !string.IsNullOrEmpty(x.NickName));

        _ = RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("手机号长度不能超过 20 个字符")
            .Matches(@"^1[3-9]\d{9}$").WithMessage("手机号格式无效")
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}

/// <summary>
/// 刷新令牌请求 DTO 验证器
/// </summary>
public class RefreshTokenRequestDTOValidator : DtoValidatorBase<RefreshTokenRequestDTO> {
    /// <summary>
    /// 初始化刷新令牌请求 DTO 验证器
    /// </summary>
    public RefreshTokenRequestDTOValidator() {
        _ = RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("访问令牌不能为空");

        _ = RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("刷新令牌不能为空");
    }
}
