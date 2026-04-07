/*
 * 文件名称: ButtonPermissionController.cs
 * 功能描述: 按钮权限控制器，处理按钮权限相关的 CRUD 操作
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

namespace API.Controllers;

/// <summary>
/// 按钮权限控制器
/// <para>处理按钮权限相关的 CRUD 操作</para>
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/button-permission")]
[ApiVersion("1.0")]
[Authorize]
public class ButtonPermissionController(IMediator mediator) : ControllerBase {
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// 获取按钮权限列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>按钮权限列表</returns>
    [HttpGet]
    public async Task<ActionResult<List<ButtonPermission>>> GetListAsync(CancellationToken cancellationToken = default) {
        var query = new ButtonPermissionListQuery(new ButtonPermissionQueryParameters());
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取按钮权限详情
    /// </summary>
    /// <param name="id">按钮权限ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>按钮权限详情</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ButtonPermission?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var query = new ButtonPermissionByIdQuery(id);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 分页获取按钮权限
    /// </summary>
    /// <param name="parameters">查询参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页结果</returns>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResponse<ButtonPermission>>> GetPagedAsync([FromQuery] ButtonPermissionQueryParameters parameters, CancellationToken cancellationToken = default) {
        var query = new ButtonPermissionPagedQuery(parameters);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 创建按钮权限
    /// </summary>
    /// <param name="dto">创建DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    public async Task<ActionResult<bool>> CreateAsync([FromBody] ButtonPermissionCreateDto dto, CancellationToken cancellationToken = default) {
        var command = new ButtonPermissionCreateCommand([dto]);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 更新按钮权限
    /// </summary>
    /// <param name="dto">更新DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPut]
    public async Task<ActionResult<bool>> UpdateAsync([FromBody] ButtonPermissionUpdateDto dto, CancellationToken cancellationToken = default) {
        var command = new ButtonPermissionUpdateCommand([dto]);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 删除按钮权限
    /// </summary>
    /// <param name="ids">按钮权限ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpDelete]
    public async Task<ActionResult<bool>> DeleteAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default) {
        var command = new ButtonPermissionDeleteCommand(ids);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 恢复按钮权限
    /// </summary>
    /// <param name="ids">按钮权限ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("restore")]
    public async Task<ActionResult<bool>> RestoreAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default) {
        var command = new ButtonPermissionRestoreCommand(ids);
        return Ok(await _mediator.Send(command, cancellationToken));
    }
}
