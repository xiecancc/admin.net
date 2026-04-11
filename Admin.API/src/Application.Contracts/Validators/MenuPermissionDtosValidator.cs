/*
 * 文件名称：MenuPermissionDtosValidator.cs
 * 功能描述：菜单权限相关 DTO 验证器，包含菜单权限创建、更新等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-11
 */

using Application.Contracts.Dtos;
using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// 菜单权限创建 DTO 验证器
/// </summary>
public class MenuPermissionCreateDtoValidator : PermissionCreateDtoValidatorBase<MenuPermissionCreateDto> {
    /// <summary>
    /// 初始化菜单权限创建 DTO 验证器
    /// </summary>
    public MenuPermissionCreateDtoValidator() {
        AddPermissionRules(x => x.Name, x => x.Code, x => x.Type, x => x.Sort);
        AddMenuPermissionRules();
    }

    /// <summary>
    /// 添加菜单权限特有验证规则
    /// </summary>
    private void AddMenuPermissionRules() {
        _ = RuleFor(x => x.Path)
            .MaximumLength(300).WithMessage("菜单路径长度不能超过 300 个字符")
            .When(x => !string.IsNullOrEmpty(x.Path));

        _ = RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("菜单图标长度不能超过 100 个字符")
            .When(x => !string.IsNullOrEmpty(x.Icon));

        _ = RuleFor(x => x.Component)
            .MaximumLength(500).WithMessage("组件路径长度不能超过 500 个字符")
            .When(x => !string.IsNullOrEmpty(x.Component));

        _ = RuleFor(x => x.Redirect)
            .MaximumLength(500).WithMessage("重定向地址长度不能超过 500 个字符")
            .When(x => !string.IsNullOrEmpty(x.Redirect));

        _ = RuleFor(x => x.Meta)
            .MaximumLength(2000).WithMessage("元信息长度不能超过 2000 个字符")
            .When(x => !string.IsNullOrEmpty(x.Meta));
    }
}

/// <summary>
/// 菜单权限更新 DTO 验证器
/// </summary>
public class MenuPermissionUpdateDtoValidator : PermissionUpdateDtoValidatorBase<MenuPermissionUpdateDto> {
    /// <summary>
    /// 初始化菜单权限更新 DTO 验证器
    /// </summary>
    public MenuPermissionUpdateDtoValidator() {
        AddPermissionRules(x => x.Name, x => x.Code, x => x.Type, x => x.Sort);
        AddMenuPermissionRules();
    }

    /// <summary>
    /// 添加菜单权限特有验证规则
    /// </summary>
    private void AddMenuPermissionRules() {
        _ = RuleFor(x => x.Path)
            .MaximumLength(300).WithMessage("菜单路径长度不能超过 300 个字符")
            .When(x => !string.IsNullOrEmpty(x.Path));

        _ = RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("菜单图标长度不能超过 100 个字符")
            .When(x => !string.IsNullOrEmpty(x.Icon));

        _ = RuleFor(x => x.Component)
            .MaximumLength(500).WithMessage("组件路径长度不能超过 500 个字符")
            .When(x => !string.IsNullOrEmpty(x.Component));

        _ = RuleFor(x => x.Redirect)
            .MaximumLength(500).WithMessage("重定向地址长度不能超过 500 个字符")
            .When(x => !string.IsNullOrEmpty(x.Redirect));

        _ = RuleFor(x => x.Meta)
            .MaximumLength(2000).WithMessage("元信息长度不能超过 2000 个字符")
            .When(x => !string.IsNullOrEmpty(x.Meta));
    }
}

/// <summary>
/// 菜单权限操作 DTO 验证器
/// </summary>
public class MenuPermissionActionDtoValidator : DtoValidatorBase<MenuPermissionActionDto> {
    /// <summary>
    /// 初始化菜单权限操作 DTO 验证器
    /// </summary>
    public MenuPermissionActionDtoValidator() {
    }
}
