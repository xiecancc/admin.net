/*
 * 文件名称: DepartmentCommands.cs
 * 功能描述: 部门相关命令，包含创建、更新、删除等操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Application.Contracts.Dtos;
using Application.Contracts.Abstractions.Commands;

namespace Application.Contracts.Commands;

/// <summary>
/// 部门创建命令
/// <para>用于创建部门</para>
/// </summary>
public class DepartmentCreateCommand : DomainCreateCommand<DepartmentCreateDto> {
    /// <summary>
    /// 初始化部门创建命令
    /// </summary>
    /// <param name="data">部门创建数据</param>
    public DepartmentCreateCommand(DepartmentCreateDto data) : base(data) { }
}

/// <summary>
/// 部门更新命令
/// <para>用于更新部门</para>
/// </summary>
public class DepartmentUpdateCommand : AggregateUpdateCommand<DepartmentUpdateDto> {
    /// <summary>
    /// 初始化部门更新命令
    /// </summary>
    /// <param name="id">部门 ID</param>
    /// <param name="data">部门更新数据</param>
    public DepartmentUpdateCommand(Guid id, DepartmentUpdateDto data) : base(id, data) { }
}

/// <summary>
/// 部门删除命令
/// <para>用于删除部门</para>
/// </summary>
public class DepartmentDeleteCommand : AggregateDeleteCommand<DepartmentActionDto> {
    /// <summary>
    /// 初始化部门删除命令
    /// </summary>
    /// <param name="id">部门 ID</param>
    /// <param name="data">部门操作数据</param>
    public DepartmentDeleteCommand(Guid id, DepartmentActionDto data) : base(id, data) { }
}

/// <summary>
/// 部门恢复命令
/// <para>用于恢复部门</para>
/// </summary>
public class DepartmentRestoreCommand : AggregateRestoreCommand<DepartmentActionDto> {
    /// <summary>
    /// 初始化部门恢复命令
    /// </summary>
    /// <param name="id">部门 ID</param>
    /// <param name="data">部门操作数据</param>
    public DepartmentRestoreCommand(Guid id, DepartmentActionDto data) : base(id, data) { }
}

/// <summary>
/// 部门移动命令
/// <para>用于移动部门到新的父部门</para>
/// </summary>
public class DepartmentMoveCommand : DomainCommand {
    /// <summary>
    /// 部门 ID
    /// </summary>
    /// <value>要移动的部门 ID</value>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// 新的父部门 ID
    /// </summary>
    /// <value>新的父部门 ID，可以为空</value>
    public Guid? NewParentId { get; set; }
}

/// <summary>
/// 用户部门角色关联命令
/// <para>用于关联用户、部门和角色</para>
/// </summary>
public class UserDepartmentRoleAssignCommand : DomainCommand {
    /// <summary>
    /// 用户部门角色关联数据
    /// </summary>
    /// <value>用户部门角色关联数据列表</value>
    public List<UserDepartmentRoleDto> Assignments { get; set; } = [];
}

/// <summary>
/// 用户部门角色移除命令
/// <para>用于移除用户、部门和角色的关联</para>
/// </summary>
public class UserDepartmentRoleRemoveCommand : DomainCommand {
    /// <summary>
    /// 用户 ID
    /// </summary>
    /// <value>用户的 ID</value>
    public Guid UserId { get; set; }

    /// <summary>
    /// 部门 ID
    /// </summary>
    /// <value>部门的 ID</value>
    public Guid DepartmentId { get; set; }
}