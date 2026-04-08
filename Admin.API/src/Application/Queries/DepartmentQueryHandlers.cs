/*
 * 文件名称: DepartmentQueryHandlers.cs
 * 功能描述: 部门查询处理器，处理部门相关的查询
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Application.Abstractions.Queries;
using Application.Contracts.Queries;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;
using AutoMapper;
using SqlSugar;

namespace Application.Queries;

/// <summary>
/// 部门列表查询处理器
/// 处理部门列表查询
/// </summary>
public class DepartmentListQueryHandler(IDepartmentRepository departmentRepository, IMapper mapper) : DomainListQueryHandler<DepartmentListQuery, Department, IDepartmentRepository, DepartmentQueryParameters, DepartmentListDto>(departmentRepository, mapper);

/// <summary>
/// 部门分页查询处理器
/// 处理部门分页查询
/// </summary>
public class DepartmentPagedQueryHandler(IDepartmentRepository departmentRepository, IMapper mapper) : DomainPagedQueryHandler<DepartmentPagedQuery, Department, IDepartmentRepository, DepartmentQueryParameters, DepartmentPagedDto>(departmentRepository, mapper);

/// <summary>
/// 部门详情查询处理器
/// 处理部门详情查询
/// </summary>
public class DepartmentByIdQueryHandler(IDepartmentRepository departmentRepository, IMapper mapper) : AggregateByIdQueryHandler<DepartmentByIdQuery, Department, IDepartmentRepository, DepartmentDetailDto>(departmentRepository, mapper);

/// <summary>
/// 部门树形结构查询处理器
/// 处理部门树形结构查询
/// </summary>
public class DepartmentTreeQueryHandler(IDepartmentRepository departmentRepository, IMapper mapper) : RequestHandler<DepartmentTreeQuery, List<DepartmentDetailDto>> {
    private readonly IDepartmentRepository _departmentRepository = departmentRepository;
    private readonly IMapper _mapper = mapper;

    /// <inheritdoc/>
    public override async Task<List<DepartmentDetailDto>> Handle(DepartmentTreeQuery request, CancellationToken cancellationToken) {
        var departments = await _departmentRepository.GetTreeAsync(cancellationToken);
        return _mapper.Map<List<DepartmentDetailDto>>(departments);
    }
}

/// <summary>
/// 部门子树结构查询处理器
/// 处理部门子树结构查询
/// </summary>
public class DepartmentSubTreeQueryHandler(IDepartmentRepository departmentRepository, IMapper mapper) : RequestHandler<DepartmentSubTreeQuery, DepartmentDetailDto> {
    private readonly IDepartmentRepository _departmentRepository = departmentRepository;
    private readonly IMapper _mapper = mapper;

    /// <inheritdoc/>
    public override async Task<DepartmentDetailDto> Handle(DepartmentSubTreeQuery request, CancellationToken cancellationToken) {
        var department = await _departmentRepository.GetSubTreeAsync(request.DepartmentId, cancellationToken);
        return _mapper.Map<DepartmentDetailDto>(department);
    }
}

/// <summary>
/// 部门用户查询处理器
/// 处理部门用户查询
/// </summary>
public class DepartmentUsersQueryHandler(ISqlSugarClient db, IMapper mapper) : RequestHandler<DepartmentUsersQuery, List<UserListDto>> {
    private readonly ISqlSugarClient _db = db;
    private readonly IMapper _mapper = mapper;

    /// <inheritdoc/>
    public override async Task<List<UserListDto>> Handle(DepartmentUsersQuery request, CancellationToken cancellationToken) {
        var users = await _db.Queryable<User, UserDepartmentRole>(
            (u, udr) => new JoinQueryInfos(
                JoinType.Inner,
                u.Id == udr.UserId
            ))
            .Where((u, udr) => udr.DepartmentId == request.DepartmentId && !u.IsDeleted)
            .Select((u, udr) => u)
            .Distinct()
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<UserListDto>>(users);
    }
}

/// <summary>
/// 用户部门查询处理器
/// 处理用户部门查询
/// </summary>
public class UserDepartmentsQueryHandler(ISqlSugarClient db, IMapper mapper) : RequestHandler<UserDepartmentsQuery, List<DepartmentListDto>> {
    private readonly ISqlSugarClient _db = db;
    private readonly IMapper _mapper = mapper;

    /// <inheritdoc/>
    public override async Task<List<DepartmentListDto>> Handle(UserDepartmentsQuery request, CancellationToken cancellationToken) {
        var departments = await _db.Queryable<Department, UserDepartmentRole>(
            (d, udr) => new JoinQueryInfos(
                JoinType.Inner,
                d.Id == udr.DepartmentId
            ))
            .Where((d, udr) => udr.UserId == request.UserId && !d.IsDeleted)
            .Select((d, udr) => d)
            .Distinct()
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<DepartmentListDto>>(departments);
    }
}