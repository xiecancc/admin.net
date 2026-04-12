/*
 * 文件名称: AuthController.cs
 * 功能描述: 认证控制器，处理用户登录、注册、令牌刷新等认证相关操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Application.Contracts.Commands;
using Application.Contracts.Dtos;
using Application.Contracts.Queries;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace API.Controllers;

/// <summary>
/// 认证控制器
/// <para>处理用户登录、注册、令牌刷新等认证相关操作</para>
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController(ILogger<AuthController> logger, IMediator mediator) : ControllerBase {
    private readonly ILogger<AuthController> _logger = logger;
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// 用户登录
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(request, "登录请求");
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, cancellationToken);
        _logger.LogInformation("User {Email} logged in successfully", request.Email);
        return Ok(result);
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<bool>> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(request, "注册请求");
        var command = new RegisterCommand(request.Email, request.Password, request.NickName, request.Phone);
        var result = await _mediator.Send(command, cancellationToken);
        _logger.LogInformation("User {Email} registered successfully", request.Email);
        return Ok(result);
    }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(request, "刷新令牌请求");
        var command = new RefreshTokenCommand(request.AccessToken, request.RefreshToken);
        var result = await _mediator.Send(command, cancellationToken);
        _logger.LogInformation("Token refreshed for user {Email}", result.User?.Email);
        return Ok(result);
    }

    /// <summary>
    /// 用户登出
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<bool>> Logout(CancellationToken cancellationToken = default) {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("User {UserId} logout", userId);

        var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        ArgumentException.ThrowIfNullOrWhiteSpace(token, "令牌");

        var command = new LogoutCommand(token);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<LoginUserInfoDto>> GetCurrentUser(CancellationToken cancellationToken = default) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new ArgumentException("用户信息不完整");

        var userId = Guid.Parse(userIdClaim.Value);
        var query = new GetProfileQuery { UserId = userId };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
