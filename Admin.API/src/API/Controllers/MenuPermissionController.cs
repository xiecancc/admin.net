/*
 * 文件名称: MenuPermissionController.cs
 * 功能描述: 菜单权限控制器，处理菜单权限相关的 CRUD 操作
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
/// 菜单权限控制器
/// <para>处理菜单权限相关的 CRUD 操作</para>
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/menu-permission")]
[ApiVersion("1.0")]
[Authorize]
public class MenuPermissionController(IMediator mediator) : ControllerBase {
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// 获取菜单权限列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>菜单权限列表</returns>
    [HttpGet]
    public async Task<ActionResult<List<MenuPermission>>> GetListAsync(CancellationToken cancellationToken = default) {
        var query = new MenuPermissionListQuery(new MenuPermissionQueryParameters());
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取菜单权限详情
    /// </summary>
    /// <param name="id">菜单权限ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>菜单权限详情</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MenuPermission?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var query = new MenuPermissionByIdQuery(id);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 分页获取菜单权限
    /// </summary>
    /// <param name="parameters">查询参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页结果</returns>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResponse<MenuPermission>>> GetPagedAsync([FromQuery] MenuPermissionQueryParameters parameters, CancellationToken cancellationToken = default) {
        var query = new MenuPermissionPagedQuery(parameters);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 创建菜单权限
    /// </summary>
    /// <param name="dto">创建DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    public async Task<ActionResult<bool>> CreateAsync([FromBody] MenuPermissionCreateDto dto, CancellationToken cancellationToken = default) {
        var command = new MenuPermissionCreateCommand([dto]);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 更新菜单权限
    /// </summary>
    /// <param name="dto">更新DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPut]
    public async Task<ActionResult<bool>> UpdateAsync([FromBody] MenuPermissionUpdateDto dto, CancellationToken cancellationToken = default) {
        var command = new MenuPermissionUpdateCommand([dto]);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 删除菜单权限
    /// </summary>
    /// <param name="ids">菜单权限ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpDelete]
    public async Task<ActionResult<bool>> DeleteAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default) {
        var command = new MenuPermissionDeleteCommand(ids);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 恢复菜单权限
    /// </summary>
    /// <param name="ids">菜单权限ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("restore")]
    public async Task<ActionResult<bool>> RestoreAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default) {
        var command = new MenuPermissionRestoreCommand(ids);
        return Ok(await _mediator.Send(command, cancellationToken));
    }
}
