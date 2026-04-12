/*
 * 文件名称: MenuPermissionController.cs
 * 功能描述: 菜单权限控制器，处理菜单权限相关的 CRUD 操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
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
/// 菜单权限控制器
/// <para>处理菜单权限相关的 CRUD 操作</para>
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/menu-permission")]
[ApiVersion("1.0")]
[Authorize]
[EnableRateLimiting("MenuPermissionPolicy")]
public class MenuPermissionController(IMediator _mediator) : ControllerBase
{

    /// <summary>
    /// 获取菜单权限列表
    /// </summary>
    /// <param name="queryDto">查询参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>菜单权限列表</returns>
    [HttpGet]
    [Permission("permission:menu:view")]
    public async Task<ActionResult<List<MenuPermissionListDto>>> GetListAsync([FromQuery] MenuPermissionQueryDto queryDto, CancellationToken cancellationToken = default) {
        var query = new MenuPermissionListQuery(queryDto);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取菜单权限详情
    /// </summary>
    /// <param name="id">菜单权限ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>菜单权限详情</returns>
    [HttpGet("{id:guid}")]
    [Permission("permission:menu:view")]
    public async Task<ActionResult<MenuPermissionDetailDto?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var query = new MenuPermissionByIdQuery(new MenuPermissionQueryDto()) { Id = id };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 分页获取菜单权限
    /// </summary>
    /// <param name="queryDto">查询参数 DTO</param>
    /// <param name="page">页码</param>
    /// <param name="size">每页大小</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页结果</returns>
    [HttpGet("paged")]
    [Permission("permission:menu:view")]
    public async Task<ActionResult<PagedResponse<MenuPermissionPagedDto>>> GetPagedAsync(
        [FromQuery] MenuPermissionQueryDto queryDto,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        CancellationToken cancellationToken = default) {
        var query = new MenuPermissionPagedQuery(queryDto) { Page = page, Size = size };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 创建菜单权限
    /// </summary>
    /// <param name="dto">创建DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    [Permission("permission:menu:create")]
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
    [Permission("permission:menu:update")]
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
    [Permission("permission:menu:delete")]
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
    [Permission("permission:menu:update")]
    public async Task<ActionResult<bool>> RestoreAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default) {
        var command = new MenuPermissionRestoreCommand(ids);
        return Ok(await _mediator.Send(command, cancellationToken));
    }
}
