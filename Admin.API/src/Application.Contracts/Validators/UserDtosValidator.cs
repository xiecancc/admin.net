/*
 * 文件名称：UserDtosValidator.cs
 * 功能描述：用户相关 DTO 验证器，包含用户创建、更新等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-11
 */

using System.Linq.Expressions;
using Application.Contracts.Dtos;
using Application.Contracts.Queries;
using FluentValidation;

namespace Application.Contracts.Validators;

#region 用户验证器基类

/// <summary>
/// 用户 DTO 验证器基类
/// <para>提供用户相关 DTO 的通用验证规则</para>
/// </summary>
/// <typeparam name="T">要验证的 DTO 类型</typeparam>
/// <remarks>
/// <para>包含用户实体的公共字段验证：NickName, Phone, Avatar</para>
/// </remarks>
public abstract class UserDtoValidatorBase<T> : DtoValidatorBase<T> {
    /// <summary>
    /// 初始化用户验证规则
    /// </summary>
    protected UserDtoValidatorBase() {
    }

    /// <summary>
    /// 添加用户可选字段验证规则
    /// </summary>
    /// <param name="nickNameSelector">昵称字段选择器</param>
    /// <param name="phoneSelector">手机号字段选择器</param>
    /// <param name="avatarSelector">头像字段选择器</param>
    protected void AddUserOptionalRules(
        Expression<Func<T, string?>> nickNameSelector,
        Expression<Func<T, string?>> phoneSelector,
        Expression<Func<T, string?>> avatarSelector) {
        RuleFor(nickNameSelector)
            .MaximumLength(50).WithMessage("昵称长度不能超过 50 个字符")
            .When(x => !string.IsNullOrEmpty(nickNameSelector.Compile()(x)));

        RuleFor(phoneSelector)
            .MaximumLength(20).WithMessage("手机号长度不能超过 20 个字符")
            .Matches(@"^1[3-9]\d{9}$").WithMessage("手机号格式无效")
            .When(x => !string.IsNullOrEmpty(phoneSelector.Compile()(x)));

        RuleFor(avatarSelector)
            .MaximumLength(500).WithMessage("头像 URL 长度不能超过 500 个字符")
            .Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("头像 URL 格式无效")
            .When(x => !string.IsNullOrEmpty(avatarSelector.Compile()(x)));
    }
}

/// <summary>
/// 用户创建 DTO 验证器基类
/// </summary>
/// <typeparam name="T">要验证的 DTO 类型</typeparam>
public abstract class UserCreateDtoValidatorBase<T> : UserDtoValidatorBase<T> {
    /// <summary>
    /// 初始化用户创建验证规则
    /// </summary>
    protected UserCreateDtoValidatorBase() : base() {
    }

    /// <summary>
    /// 添加用户必填字段验证规则
    /// </summary>
    /// <param name="emailSelector">邮箱字段选择器</param>
    /// <param name="passwordSelector">密码字段选择器</param>
    protected void AddUserRequiredRules(
        Expression<Func<T, string>> emailSelector,
        Expression<Func<T, string>> passwordSelector) {
        RuleFor(emailSelector)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式无效")
            .MaximumLength(100).WithMessage("邮箱长度不能超过 100 个字符");

        RuleFor(passwordSelector)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(8).WithMessage("密码长度至少为 8 个字符")
            .MaximumLength(100).WithMessage("密码长度不能超过 100 个字符")
            .Matches(@"[A-Z]").WithMessage("密码必须包含至少一个大写字母")
            .Matches(@"[a-z]").WithMessage("密码必须包含至少一个小写字母")
            .Matches(@"[0-9]").WithMessage("密码必须包含至少一个数字");
    }
}

/// <summary>
/// 用户更新 DTO 验证器基类
/// </summary>
/// <typeparam name="T">要验证的 DTO 类型</typeparam>
public abstract class UserUpdateDtoValidatorBase<T> : UserDtoValidatorBase<T> {
    /// <summary>
    /// 初始化用户更新验证规则
    /// </summary>
    protected UserUpdateDtoValidatorBase() : base() {
    }
}

#endregion

#region 用户具体验证器

/// <summary>
/// 用户创建 DTO 验证器
/// </summary>
public class UserCreateDtoValidator : UserCreateDtoValidatorBase<UserCreateDto> {
    /// <summary>
    /// 初始化用户创建 DTO 验证器
    /// </summary>
    public UserCreateDtoValidator() {
        AddUserRequiredRules(x => x.Email, x => x.Password);
        AddUserOptionalRules(x => x.NickName, x => x.Phone, x => x.Avatar);
    }
}

/// <summary>
/// 用户更新 DTO 验证器
/// </summary>
public class UserUpdateDtoValidator : UserUpdateDtoValidatorBase<UserUpdateDto> {
    /// <summary>
    /// 初始化用户更新 DTO 验证器
    /// </summary>
    public UserUpdateDtoValidator() {
        AddUserOptionalRules(x => x.NickName, x => x.Phone, x => x.Avatar);
    }
}

/// <summary>
/// 用户操作 DTO 验证器
/// </summary>
public class UserActionDtoValidator : DtoValidatorBase<UserActionDto> {
    /// <summary>
    /// 初始化用户操作 DTO 验证器
    /// </summary>
    public UserActionDtoValidator() {
    }
}

/// <summary>
/// 用户列表查询验证器
/// </summary>
public class UserListQueryValidator : DtoValidatorBase<UserListQuery> {
    /// <summary>
    /// 初始化用户列表查询验证器
    /// </summary>
    public UserListQueryValidator() {
        RuleFor(x => x.QueryDto.Email)
            .EmailAddress().WithMessage("邮箱格式无效")
            .MaximumLength(100).WithMessage("邮箱长度不能超过 100 个字符")
            .When(x => x.QueryDto != null && !string.IsNullOrEmpty(x.QueryDto.Email));

        RuleFor(x => x.QueryDto.NickName)
            .MaximumLength(50).WithMessage("昵称长度不能超过 50 个字符")
            .When(x => x.QueryDto != null && !string.IsNullOrEmpty(x.QueryDto.NickName));

        RuleFor(x => x.QueryDto.Phone)
            .MaximumLength(20).WithMessage("手机号长度不能超过 20 个字符")
            .When(x => x.QueryDto != null && !string.IsNullOrEmpty(x.QueryDto.Phone));
    }
}

/// <summary>
/// 用户分页查询验证器
/// </summary>
public class UserPagedQueryValidator : DtoValidatorBase<UserPagedQuery> {
    /// <summary>
    /// 初始化用户分页查询验证器
    /// </summary>
    public UserPagedQueryValidator() {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("页码必须大于等于 1");

        RuleFor(x => x.Size)
            .GreaterThanOrEqualTo(1).WithMessage("每页大小必须大于等于 1")
            .LessThanOrEqualTo(100).WithMessage("每页大小不能超过 100");
    }
}

/// <summary>
/// 用户查询参数 DTO 验证器
/// </summary>
public class UserQueryDtoValidator : DtoValidatorBase<UserQueryDto> {
    /// <summary>
    /// 初始化用户查询参数 DTO 验证器
    /// </summary>
    public UserQueryDtoValidator() {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("邮箱格式无效")
            .MaximumLength(100).WithMessage("邮箱长度不能超过 100 个字符")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.NickName)
            .MaximumLength(50).WithMessage("昵称长度不能超过 50 个字符")
            .When(x => !string.IsNullOrEmpty(x.NickName));

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("手机号长度不能超过 20 个字符")
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}

#endregion
