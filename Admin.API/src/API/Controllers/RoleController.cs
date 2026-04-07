/*
 * 文件名称: RoleController.cs
 * 功能描述: 角色控制器，处理角色相关的CRUD操作
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
/// 角色控制器
/// <para>处理角色相关的CRUD操作</para>
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/role")]
[ApiVersion("1.0")]
[Authorize]
[EnableRateLimiting("RolePolicy")]
public class RoleController(IMediator mediator) : ControllerBase {
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// 获取角色列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色列表</returns>
    [HttpGet]
    public async Task<ActionResult<List<Role>>> GetListAsync(CancellationToken cancellationToken = default) {
        var query = new RoleListQuery(new RoleQueryParameters());
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色详情</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Role?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var query = new RoleByIdQuery(id);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 分页获取角色
    /// </summary>
    /// <param name="parameters">查询参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页结果</returns>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResponse<Role>>> GetPagedAsync([FromQuery] RoleQueryParameters parameters, CancellationToken cancellationToken = default) {
        var query = new RolePagedQuery(parameters);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="dto">创建DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    public async Task<ActionResult<bool>> CreateAsync([FromBody] RoleCreateDto dto, CancellationToken cancellationToken = default) {
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
    public async Task<ActionResult<bool>> UpdateAsync([FromBody] RoleUpdateDto dto, CancellationToken cancellationToken = default) {
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
    public async Task<ActionResult<bool>> DeleteAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default) {
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
    public async Task<ActionResult<bool>> RestoreAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default) {
        var command = new RoleRestoreCommand(ids);
        return Ok(await _mediator.Send(command, cancellationToken));
    }
}
