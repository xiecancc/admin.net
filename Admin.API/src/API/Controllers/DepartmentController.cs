/*
 * 文件名称: DepartmentController.cs
 * 功能描述: 部门控制器，处理部门相关的CRUD操作和树形结构管理
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using API.Filters;
using Application.Contracts.Commands;
using Application.Contracts.Dtos;
using Application.Contracts.Queries;
using Asp.Versioning;
using Domain.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// 部门控制器
/// <para>处理部门相关的CRUD操作和树形结构管理</para>
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/department")]
[ApiVersion("1.0")]
[Authorize]
[EnableRateLimiting("UserPolicy")]
public class DepartmentController(IMediator mediator) : ControllerBase {
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// 获取部门列表
    /// </summary>
    /// <param name="parameters">查询参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部门列表</returns>
    [HttpGet]
    [Permission("department:view")]
    public async Task<ActionResult<List<DepartmentListDto>>> GetListAsync([FromQuery] DepartmentQueryParameters parameters, CancellationToken cancellationToken = default) {
        var query = new DepartmentListQuery(parameters);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取部门详情
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部门详情</returns>
    [HttpGet("{id:guid}")]
    [Permission("department:view")]
    public async Task<ActionResult<DepartmentDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var query = new DepartmentByIdQuery(id);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 分页获取部门
    /// </summary>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="parameters">查询参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页结果</returns>
    [HttpGet("paged")]
    [Permission("department:view")]
    public async Task<ActionResult<PagedResponse<DepartmentPagedDto>>> GetPagedAsync(int page, int pageSize, [FromQuery] DepartmentQueryParameters parameters, CancellationToken cancellationToken = default) {
        var query = new DepartmentPagedQuery(page, pageSize, parameters);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取部门树形结构
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部门树形结构</returns>
    [HttpGet("tree")]
    [Permission("department:view")]
    public async Task<ActionResult<List<DepartmentDetailDto>>> GetTreeAsync(CancellationToken cancellationToken = default) {
        var query = new DepartmentTreeQuery();
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取部门子树结构
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部门子树结构</returns>
    [HttpGet("{id:guid}/subtree")]
    [Permission("department:view")]
    public async Task<ActionResult<DepartmentDetailDto>> GetSubTreeAsync(Guid id, CancellationToken cancellationToken = default) {
        var query = new DepartmentSubTreeQuery { DepartmentId = id };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 创建部门
    /// </summary>
    /// <param name="dto">创建DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    [Permission("department:create")]
    public async Task<ActionResult<bool>> CreateAsync([FromBody] DepartmentCreateDto dto, CancellationToken cancellationToken = default) {
        var command = new DepartmentCreateCommand(dto);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="dto">更新DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id:guid}")]
    [Permission("department:update")]
    [DepartmentPermission]
    public async Task<ActionResult<bool>> UpdateAsync(Guid id, [FromBody] DepartmentUpdateDto dto, CancellationToken cancellationToken = default) {
        var command = new DepartmentUpdateCommand(id, dto);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id:guid}")]
    [Permission("department:delete")]
    [DepartmentPermission]
    public async Task<ActionResult<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default) {
        var command = new DepartmentDeleteCommand(id, new DepartmentActionDto());
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 恢复部门
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id:guid}/restore")]
    [Permission("department:update")]
    [DepartmentPermission]
    public async Task<ActionResult<bool>> RestoreAsync(Guid id, CancellationToken cancellationToken = default) {
        var command = new DepartmentRestoreCommand(id, new DepartmentActionDto());
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 移动部门
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="newParentId">新的父部门ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id:guid}/move")]
    [Permission("department:update")]
    [DepartmentPermission]
    public async Task<ActionResult<bool>> MoveAsync(Guid id, [FromBody] Guid? newParentId, CancellationToken cancellationToken = default) {
        var command = new DepartmentMoveCommand { DepartmentId = id, NewParentId = newParentId };
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    #region 部门用户管理

    /// <summary>
    /// 获取部门下的用户
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户列表</returns>
    [HttpGet("{id:guid}/users")]
    [Permission("department:view")]
    [DepartmentPermission]
    public async Task<ActionResult<List<UserListDto>>> GetDepartmentUsersAsync(Guid id, CancellationToken cancellationToken = default) {
        var query = new DepartmentUsersQuery { DepartmentId = id };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取用户所属的部门
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部门列表</returns>
    [HttpGet("user/{userId:guid}")]
    [Permission("department:view")]
    public async Task<ActionResult<List<DepartmentListDto>>> GetUserDepartmentsAsync(Guid userId, CancellationToken cancellationToken = default) {
        var query = new UserDepartmentsQuery { UserId = userId };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 为用户分配部门角色
    /// </summary>
    /// <param name="assignments">用户部门角色关联数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("user-roles")]
    [Permission("department:update")]
    public async Task<ActionResult<bool>> AssignUserDepartmentRolesAsync([FromBody] List<UserDepartmentRoleDto> assignments, CancellationToken cancellationToken = default) {
        var command = new UserDepartmentRoleAssignCommand { Assignments = assignments };
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 移除用户部门角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="departmentId">部门ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpDelete("user-roles/{userId:guid}/{departmentId:guid}")]
    [Permission("department:update")]
    [DepartmentPermission(DepartmentIdParamName = "departmentId")]
    public async Task<ActionResult<bool>> RemoveUserDepartmentRoleAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default) {
        var command = new UserDepartmentRoleRemoveCommand { UserId = userId, DepartmentId = departmentId };
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    #endregion
}