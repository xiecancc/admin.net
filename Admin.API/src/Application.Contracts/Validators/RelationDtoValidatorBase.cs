/*
 * 文件名称：RelationDtoValidatorBase.cs
 * 功能描述：关联表 DTO 验证器基类，提供关联表相关 DTO 的通用验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

using System.Linq.Expressions;
using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// 关联表 DTO 验证器基类
/// <para>提供关联表相关 DTO 的通用验证规则</para>
/// </summary>
/// <typeparam name="T">要验证的 DTO 类型</typeparam>
/// <remarks>
/// <para>包含关联表实体的公共字段验证：两个关联 ID</para>
/// <para>适用于：UserRole, RolePermission</para>
/// </remarks>
public abstract class RelationDtoValidatorBase<T> : DtoValidatorBase<T> {
    /// <summary>
    /// 初始化关联表验证规则
    /// </summary>
    protected RelationDtoValidatorBase() {
    }

    /// <summary>
    /// 添加关联表字段验证规则
    /// </summary>
    /// <typeparam name="TId1">第一个 ID 类型</typeparam>
    /// <typeparam name="TId2">第二个 ID 类型</typeparam>
    /// <param name="id1Selector">第一个 ID 字段选择器</param>
    /// <param name="id2Selector">第二个 ID 字段选择器</param>
    /// <param name="id1Name">第一个 ID 名称</param>
    /// <param name="id2Name">第二个 ID 名称</param>
    protected void AddRelationRules<TId1, TId2>(
        Expression<Func<T, TId1>> id1Selector,
        Expression<Func<T, TId2>> id2Selector,
        string id1Name = "关联 ID 1",
        string id2Name = "关联 ID 2")
        where TId1 : struct
        where TId2 : struct {
        RuleFor(id1Selector)
            .NotEmpty().WithMessage($"{id1Name}不能为空");

        RuleFor(id2Selector)
            .NotEmpty().WithMessage($"{id2Name}不能为空");
    }
}

/// <summary>
/// 关联表创建 DTO 验证器基类
/// </summary>
/// <typeparam name="T">要验证的 DTO 类型</typeparam>
public abstract class RelationCreateDtoValidatorBase<T> : RelationDtoValidatorBase<T> {
    /// <summary>
    /// 初始化关联表创建验证规则
    /// </summary>
    protected RelationCreateDtoValidatorBase() : base() {
    }
}

/// <summary>
/// 关联表操作 DTO 验证器基类
/// </summary>
/// <typeparam name="T">要验证的 DTO 类型</typeparam>
public abstract class RelationActionDtoValidatorBase<T> : RelationDtoValidatorBase<T> {
    /// <summary>
    /// 初始化关联表操作验证规则
    /// </summary>
    protected RelationActionDtoValidatorBase() : base() {
    }
}
