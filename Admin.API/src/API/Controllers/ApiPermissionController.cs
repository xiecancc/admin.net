/*
 * 文件名称: ApiPermissionController.cs
 * 功能描述: API权限控制器，处理API权限相关的 CRUD 操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
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

namespace API.Controllers;

/// <summary>
/// API权限控制器
/// <para>处理API权限相关的 CRUD 操作</para>
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/api-permission")]
[ApiVersion("1.0")]
[Authorize]
public class ApiPermissionController(IMediator mediator) : ControllerBase {
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// 获取API权限列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>API权限列表</returns>
    [HttpGet]
    [Permission("permission:api:view")]
    public async Task<ActionResult<List<ApiPermission>>> GetListAsync(CancellationToken cancellationToken = default) {
        var query = new ApiPermissionListQuery(new ApiPermissionQueryParameters());
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 获取API权限详情
    /// </summary>
    /// <param name="id">API权限ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>API权限详情</returns>
    [HttpGet("{id:guid}")]
    [Permission("permission:api:view")]
    public async Task<ActionResult<ApiPermission?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var query = new ApiPermissionByIdQuery(id);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 分页获取API权限
    /// </summary>
    /// <param name="parameters">查询参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页结果</returns>
    [HttpGet("paged")]
    [Permission("permission:api:view")]
    public async Task<ActionResult<PagedResponse<ApiPermission>>> GetPagedAsync([FromQuery] ApiPermissionQueryParameters parameters, CancellationToken cancellationToken = default) {
        var query = new ApiPermissionPagedQuery(parameters);
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// 创建API权限
    /// </summary>
    /// <param name="dto">创建DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    [Permission("permission:api:create")]
    public async Task<ActionResult<bool>> CreateAsync([FromBody] ApiPermissionCreateDto dto, CancellationToken cancellationToken = default) {
        var command = new ApiPermissionCreateCommand([dto]);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 更新API权限
    /// </summary>
    /// <param name="dto">更新DTO</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPut]
    [Permission("permission:api:update")]
    public async Task<ActionResult<bool>> UpdateAsync([FromBody] ApiPermissionUpdateDto dto, CancellationToken cancellationToken = default) {
        var command = new ApiPermissionUpdateCommand([dto]);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 删除API权限
    /// </summary>
    /// <param name="ids">API权限ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpDelete]
    [Permission("permission:api:delete")]
    public async Task<ActionResult<bool>> DeleteAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default) {
        var command = new ApiPermissionDeleteCommand(ids);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 恢复API权限
    /// </summary>
    /// <param name="ids">API权限ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("restore")]
    [Permission("permission:api:update")]
    public async Task<ActionResult<bool>> RestoreAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken = default) {
        var command = new ApiPermissionRestoreCommand(ids);
        return Ok(await _mediator.Send(command, cancellationToken));
    }
}
