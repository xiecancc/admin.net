/*
 * 文件名称: UserController.cs
 * 功能描述: 用户控制器，处理用户相关的CRUD操作
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
/// 用户控制器
/// <para>处理用户相关的CRUD操作</para>
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/user")]
[ApiVersion("1.0")]
[Authorize]
[EnableRateLimiting("UserPolicy")]
public class UserController(IMediator _mediator) : ControllerBase
{

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="queryDto">查询参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户列表</returns>
    [HttpGet]
    [Permission("user:view")]
    public async Task<ActionResult<List<UserListDto>>> GetListAsync([FromQuery] UserQueryDto queryDto, CancellationToken cancellationToken = default)
    {
        var query = new UserListQuery(queryDto);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取用户详情
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户详情</returns>
    [HttpGet("{id:guid}")]
    [Permission("user:view")]
    public async Task<ActionResult<UserDetailDto?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new UserByIdQuery(new UserQueryDto()) { Id = id };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 分页获取用户
    /// </summary>
    /// <param name="queryDto">查询参数</param>
    /// <param name="page">当前页面</param>
    /// <param name="size">分页大小</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页结果</returns>
    [HttpGet("paged")]
    [Permission("user:view")]
    public async Task<ActionResult<PagedResponse<UserPagedDto>>> GetPagedAsync([FromQuery] UserQueryDto queryDto, [FromQuery] int page = 1, [FromQuery] int size = 10, CancellationToken cancellationToken = default)
    {
        var query = new UserPagedQuery(queryDto) { Page = page, Size = size };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="dto">创建DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    [Permission("user:create")]
    public async Task<ActionResult<bool>> CreateAsync([FromBody] UserCreateDto dto, CancellationToken cancellationToken = default)
    {
        var command = new UserCreateCommand([dto]);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="dto">更新DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPut]
    [Permission("user:update")]
    public async Task<ActionResult<bool>> UpdateAsync([FromBody] UserUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var command = new UserUpdateCommand([dto]);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="ids">用户ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpDelete]
    [Permission("user:delete")]
    public async Task<ActionResult<bool>> DeleteAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var command = new UserDeleteCommand(ids);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 恢复用户
    /// </summary>
    /// <param name="ids">用户ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("restore")]
    [Permission("user:update")]
    public async Task<ActionResult<bool>> RestoreAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var command = new UserRestoreCommand(ids);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    #region 用户角色关联操作

    /// <summary>
    /// 为用户分配角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="roleIds">角色ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("{userId:guid}/roles")]
    [Permission("user:update")]
    public async Task<ActionResult<bool>> AssignRolesAsync(Guid userId, [FromBody] List<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        var command = new AssignRolesToUserCommand { UserId = userId, RoleIds = roleIds };
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 移除用户角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="roleIds">角色ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{userId:guid}/roles")]
    [Permission("user:update")]
    public async Task<ActionResult<bool>> RemoveRolesAsync(Guid userId, [FromBody] List<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        var command = new RemoveRolesFromUserCommand { UserId = userId, RoleIds = roleIds };
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 获取用户的角色列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色列表</returns>
    [HttpGet("{userId:guid}/roles")]
    [Permission("user:view")]
    public async Task<ActionResult<List<RoleListDto>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = new UserRolesQuery(new UserRoleQueryDto()) { UserId = userId };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取用户的角色ID列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色ID列表</returns>
    [HttpGet("{userId:guid}/role-ids")]
    [Permission("user:view")]
    public async Task<ActionResult<List<Guid>>> GetUserRoleIdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = new UserRoleIdsQuery(new UserRoleQueryDto()) { UserId = userId };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    #endregion
}
