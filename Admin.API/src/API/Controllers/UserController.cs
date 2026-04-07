/*
 * 文件名称: UserController.cs
 * 功能描述: 用户控制器，处理用户相关的CRUD操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

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
public class UserController(IMediator mediator) : ControllerBase {
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户列表</returns>
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetListAsync(CancellationToken cancellationToken = default) {
        var query = new UserListQuery(new UserQueryParameters());
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取用户详情
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户详情</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<User?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var query = new UserByIdQuery(id);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 分页获取用户
    /// </summary>
    /// <param name="parameters">查询参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页结果</returns>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResponse<User>>> GetPagedAsync([FromQuery] UserQueryParameters parameters, CancellationToken cancellationToken = default) {
        var query = new UserPagedQuery(parameters);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="dto">创建DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    public async Task<ActionResult<bool>> CreateAsync([FromBody] UserCreateDto dto, CancellationToken cancellationToken = default) {
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
    public async Task<ActionResult<bool>> UpdateAsync([FromBody] UserUpdateDto dto, CancellationToken cancellationToken = default) {
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
    public async Task<ActionResult<bool>> DeleteAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default) {
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
    public async Task<ActionResult<bool>> RestoreAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default) {
        var command = new UserRestoreCommand(ids);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 启用用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id:guid}/enable")]
    public async Task<ActionResult<bool>> EnableAsync(Guid id, CancellationToken cancellationToken = default) {
        var command = new UserEnableCommand([id]);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 禁用用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id:guid}/disable")]
    public async Task<ActionResult<bool>> DisableAsync(Guid id, CancellationToken cancellationToken = default) {
        var command = new UserDisableCommand([id]);
        return Ok(await _mediator.Send(command, cancellationToken));
    }
}
