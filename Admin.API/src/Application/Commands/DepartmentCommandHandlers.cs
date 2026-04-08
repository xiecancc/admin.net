/*
 * 文件名称: DepartmentCommandHandlers.cs
 * 功能描述: 部门命令处理器，处理部门相关的命令
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Application.Abstractions.Commands;
using Application.Contracts.Commands;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Shared.Units;
using AutoMapper;
using SqlSugar;

namespace Application.Commands;

/// <summary>
/// 部门创建命令处理器
/// 处理部门的创建操作
/// </summary>
public class DepartmentCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : DomainCreateCommandHandler<DepartmentCreateCommand, Department, IDepartmentRepository, DepartmentCreateDto>(unitOfWork, mapper);

/// <summary>
/// 部门更新命令处理器
/// 处理部门的更新操作
/// </summary>
public class DepartmentUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateUpdateCommandHandler<DepartmentUpdateCommand, Department, IDepartmentRepository, DepartmentUpdateDto>(unitOfWork, mapper);

/// <summary>
/// 部门删除命令处理器
/// 处理部门的删除操作
/// </summary>
public class DepartmentDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateDeleteCommandHandler<DepartmentDeleteCommand, Department, IDepartmentRepository, DepartmentActionDto>(unitOfWork, mapper);

/// <summary>
/// 部门恢复命令处理器
/// 处理部门的恢复操作
/// </summary>
public class DepartmentRestoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateRestoreCommandHandler<DepartmentRestoreCommand, Department, IDepartmentRepository, DepartmentActionDto>(unitOfWork, mapper);

/// <summary>
/// 部门移动命令处理器
/// 处理部门的移动操作
/// </summary>
public class DepartmentMoveCommandHandler(IDepartmentRepository departmentRepository) : RequestHandler<DepartmentMoveCommand, bool> {
    private readonly IDepartmentRepository _departmentRepository = departmentRepository;

    /// <inheritdoc/>
    public override async Task<bool> Handle(DepartmentMoveCommand request, CancellationToken cancellationToken) {
        return await _departmentRepository.MoveNodeAsync(request.DepartmentId, request.NewParentId, cancellationToken);
    }
}

/// <summary>
/// 用户部门角色关联命令处理器
/// 处理用户部门角色的关联操作
/// </summary>
public class UserDepartmentRoleAssignCommandHandler(IUnitOfWork unitOfWork, ISqlSugarClient db) : RequestHandler<UserDepartmentRoleAssignCommand, bool> {
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ISqlSugarClient _db = db;

    /// <inheritdoc/>
    public override async Task<bool> Handle(UserDepartmentRoleAssignCommand request, CancellationToken cancellationToken) {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try {
            foreach (var assignment in request.Assignments) {
                var existing = await _db.Queryable<UserDepartmentRole>()
                    .Where(udr => udr.UserId == assignment.UserId && udr.DepartmentId == assignment.DepartmentId)
                    .FirstAsync(cancellationToken);

                if (existing != null) {
                    existing.RoleId = assignment.RoleId;
                    await _db.Updateable(existing).ExecuteCommandAsync(cancellationToken);
                } else {
                    var newAssignment = new UserDepartmentRole {
                        UserId = assignment.UserId,
                        DepartmentId = assignment.DepartmentId,
                        RoleId = assignment.RoleId
                    };
                    await _db.Insertable(newAssignment).ExecuteCommandAsync(cancellationToken);
                }
            }

            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        } catch (Exception) {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

/// <summary>
/// 用户部门角色移除命令处理器
/// 处理用户部门角色的移除操作
/// </summary>
public class UserDepartmentRoleRemoveCommandHandler(IUnitOfWork unitOfWork, ISqlSugarClient db) : RequestHandler<UserDepartmentRoleRemoveCommand, bool> {
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ISqlSugarClient _db = db;

    /// <inheritdoc/>
    public override async Task<bool> Handle(UserDepartmentRoleRemoveCommand request, CancellationToken cancellationToken) {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try {
            var result = await _db.Deleteable<UserDepartmentRole>()
                .Where(udr => udr.UserId == request.UserId && udr.DepartmentId == request.DepartmentId)
                .ExecuteCommandAsync(cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
            return result > 0;
        } catch (Exception) {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}