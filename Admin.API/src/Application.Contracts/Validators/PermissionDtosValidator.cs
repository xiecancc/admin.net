/*
 * 文件名称：PermissionDtosValidator.cs
 * 功能描述：权限基础 DTO 验证器，提供权限相关 DTO 的通用验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

using Application.Contracts.Dtos;

namespace Application.Contracts.Validators;

/// <summary>
/// 权限创建 DTO 验证器
/// </summary>
public class PermissionCreateDtoValidator : PermissionCreateDtoValidatorBase<PermissionCreateDto> {
    /// <summary>
    /// 初始化权限创建 DTO 验证器
    /// </summary>
    public PermissionCreateDtoValidator() {
        AddPermissionRules(x => x.Name, x => x.Code, x => x.Type, x => x.Sort);
    }
}

/// <summary>
/// 权限更新 DTO 验证器
/// </summary>
public class PermissionUpdateDtoValidator : PermissionUpdateDtoValidatorBase<PermissionUpdateDto> {
    /// <summary>
    /// 初始化权限更新 DTO 验证器
    /// </summary>
    public PermissionUpdateDtoValidator() {
        AddPermissionRules(x => x.Name, x => x.Code, x => x.Type, x => x.Sort);
    }
}

/// <summary>
/// 权限操作 DTO 验证器
/// </summary>
public class PermissionActionDtoValidator : DtoValidatorBase<PermissionActionDto> {
    /// <summary>
    /// 初始化权限操作 DTO 验证器
    /// </summary>
    public PermissionActionDtoValidator() {
    }
}
