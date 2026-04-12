/*
 * 文件名称: RoleController.cs
 * 功能描述: 角色控制器，处理角色相关的CRUD操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using API.Filters;
using Application.Contracts.Commands;
using Application.Contracts.Dtos;
using Application.Contracts.Queries;
using Asp.Versioning;
using Domain.Entities;
using Domain.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// 角色控制器
/// <para>处理角色相关的CRUD操作</para>
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/role")]
[ApiVersion("1.0")]
[Authorize]
[EnableRateLimiting("RolePolicy")]
public class RoleController(IMediator _mediator) : ControllerBase
{

    /// <summary>
    /// 获取角色列表
    /// </summary>
    /// <param name="queryDto">查询参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色列表</returns>
    [HttpGet]
    [Permission("role:view")]
    public async Task<ActionResult<List<RoleListDto>>> GetListAsync([FromQuery] RoleQueryDto queryDto, CancellationToken cancellationToken = default)
    {
        var query = new RoleListQuery(queryDto);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色详情</returns>
    [HttpGet("{id:guid}")]
    [Permission("role:view")]
    public async Task<ActionResult<RoleDetailDto?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new RoleByIdQuery(new RoleQueryDto()) { Id = id };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 分页获取角色
    /// </summary>
    /// <param name="queryDto">查询参数</param>
    /// <param name="page">当前页面</param>
    /// <param name="size">分页大小</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页结果</returns>
    [HttpGet("paged")]
    [Permission("role:view")]
    public async Task<ActionResult<PagedResponse<RolePagedDto>>> GetPagedAsync([FromQuery] RoleQueryDto queryDto, [FromQuery] int page = 1, [FromQuery] int size = 10, CancellationToken cancellationToken = default)
    {
        var query = new RolePagedQuery(queryDto) { Page = page, Size = size };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="dto">创建DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    [Permission("role:create")]
    public async Task<ActionResult<bool>> CreateAsync([FromBody] RoleCreateDto dto, CancellationToken cancellationToken = default)
    {
        var command = new RoleCreateCommand([dto]);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="dto">更新DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPut]
    [Permission("role:update")]
    public async Task<ActionResult<bool>> UpdateAsync([FromBody] RoleUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var command = new RoleUpdateCommand([dto]);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="ids">角色ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpDelete]
    [Permission("role:delete")]
    public async Task<ActionResult<bool>> DeleteAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var command = new RoleDeleteCommand(ids);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 恢复角色
    /// </summary>
    /// <param name="ids">角色ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("restore")]
    [Permission("role:update")]
    public async Task<ActionResult<bool>> RestoreAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var command = new RoleRestoreCommand(ids);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    #region 角色权限关联操作

    /// <summary>
    /// 为角色分配权限
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="permissionIds">权限ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("{roleId:guid}/permissions")]
    [Permission("role:update")]
    public async Task<ActionResult<bool>> AssignPermissionsAsync(Guid roleId, [FromBody] List<Guid> permissionIds, CancellationToken cancellationToken = default)
    {
        var command = new AssignPermissionsToRoleCommand { RoleId = roleId, PermissionIds = permissionIds };
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 移除角色权限
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="permissionIds">权限ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{roleId:guid}/permissions")]
    [Permission("role:update")]
    public async Task<ActionResult<bool>> RemovePermissionsAsync(Guid roleId, [FromBody] List<Guid> permissionIds, CancellationToken cancellationToken = default)
    {
        var command = new RemovePermissionsFromRoleCommand { RoleId = roleId, PermissionIds = permissionIds };
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 获取角色的权限列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限列表</returns>
    [HttpGet("{roleId:guid}/permissions")]
    [Permission("role:view")]
    public async Task<ActionResult<List<PermissionListDto>>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var query = new RolePermissionsQuery(new RolePermissionQueryDto()) { RoleId = roleId };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取角色的权限ID列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限ID列表</returns>
    [HttpGet("{roleId:guid}/permission-ids")]
    [Permission("role:view")]
    public async Task<ActionResult<List<Guid>>> GetRolePermissionIdsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var query = new RolePermissionIdsQuery(new RolePermissionQueryDto()) { RoleId = roleId };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    #endregion

    #region 角色用户关联操作

    /// <summary>
    /// 为角色分配用户
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="userIds">用户ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("{roleId:guid}/users")]
    [Permission("role:update")]
    public async Task<ActionResult<bool>> AssignUsersAsync(Guid roleId, [FromBody] List<Guid> userIds, CancellationToken cancellationToken = default)
    {
        var command = new AssignUsersToRoleCommand { RoleId = roleId, UserIds = userIds };
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 移除角色用户
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="userIds">用户ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{roleId:guid}/users")]
    [Permission("role:update")]
    public async Task<ActionResult<bool>> RemoveUsersAsync(Guid roleId, [FromBody] List<Guid> userIds, CancellationToken cancellationToken = default)
    {
        var command = new RemoveUsersFromRoleCommand { RoleId = roleId, UserIds = userIds };
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 获取角色的用户列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户列表</returns>
    [HttpGet("{roleId:guid}/users")]
    [Permission("role:view")]
    public async Task<ActionResult<List<UserListDto>>> GetRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var query = new RoleUsersQuery(new UserRoleQueryDto()) { RoleId = roleId };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    #endregion
}
