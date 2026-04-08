/*
 * 文件名称: DepartmentQueries.cs
 * 功能描述: 部门相关查询，包含列表、详情、树形结构等操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Application.Contracts.Dtos;
using Application.Contracts.Abstractions.Queries;

namespace Application.Contracts.Queries;

/// <summary>
/// 部门列表查询
/// <para>用于查询部门列表</para>
/// </summary>
public class DepartmentListQuery : DomainListQuery<DepartmentQueryParameters, DepartmentListDto> {
    /// <summary>
    /// 初始化部门列表查询
    /// </summary>
    /// <param name="parameters">查询参数</param>
    public DepartmentListQuery(DepartmentQueryParameters parameters) : base(parameters) { }
}

/// <summary>
/// 部门分页查询
/// <para>用于分页查询部门</para>
/// </summary>
public class DepartmentPagedQuery : DomainPagedQuery<DepartmentQueryParameters, DepartmentPagedDto> {
    /// <summary>
    /// 初始化部门分页查询
    /// </summary>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="parameters">查询参数</param>
    public DepartmentPagedQuery(int page, int pageSize, DepartmentQueryParameters parameters) : base(page, pageSize, parameters) { }
}

/// <summary>
/// 部门详情查询
/// <para>用于查询部门详情</para>
/// </summary>
public class DepartmentByIdQuery : AggregateByIdQuery<DepartmentDetailDto> {
    /// <summary>
    /// 初始化部门详情查询
    /// </summary>
    /// <param name="id">部门 ID</param>
    public DepartmentByIdQuery(Guid id) : base(id) { }
}

/// <summary>
/// 部门树形结构查询
/// <para>用于查询部门树形结构</para>
/// </summary>
public class DepartmentTreeQuery : DomainQuery<List<DepartmentDetailDto>> {
}

/// <summary>
/// 部门子树结构查询
/// <para>用于查询指定部门的子树结构</para>
/// </summary>
public class DepartmentSubTreeQuery : DomainQuery<DepartmentDetailDto> {
    /// <summary>
    /// 部门 ID
    /// </summary>
    /// <value>部门的 ID</value>
    public Guid DepartmentId { get; set; }
}

/// <summary>
/// 部门用户查询
/// <para>用于查询部门下的用户</para>
/// </summary>
public class DepartmentUsersQuery : DomainQuery<List<UserListDto>> {
    /// <summary>
    /// 部门 ID
    /// </summary>
    /// <value>部门的 ID</value>
    public Guid DepartmentId { get; set; }
}

/// <summary>
/// 用户部门查询
/// <para>用于查询用户所属的部门</para>
/// </summary>
public class UserDepartmentsQuery : DomainQuery<List<DepartmentListDto>> {
    /// <summary>
    /// 用户 ID
    /// </summary>
    /// <value>用户的 ID</value>
    public Guid UserId { get; set; }
}