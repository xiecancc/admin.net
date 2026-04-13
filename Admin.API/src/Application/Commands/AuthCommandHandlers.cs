/*
 * 文件名称: AuthCommandHandlers.cs
 * 功能描述: 认证相关的命令处理器，包含登录、注册、刷新令牌、登出等命令的处理逻辑
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using Application.Contracts.Commands;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Domain.Shared.Services;
using Domain.Shared.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Commands;

/// <summary>
/// 用户登录命令处理器
/// <para>处理用户登录请求，验证用户身份并生成访问令牌</para>
/// </summary>
public class LoginCommandHandler(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IJwtService jwtService,
    IPermissionDomainService permissionDomainService,
    IPermissionCacheService permissionCacheService,
    ILogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, LoginResponseDto> {
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
    private readonly IJwtService _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    private readonly IPermissionDomainService _permissionDomainService = permissionDomainService ?? throw new ArgumentNullException(nameof(permissionDomainService));
    private readonly IPermissionCacheService _permissionCacheService = permissionCacheService ?? throw new ArgumentNullException(nameof(permissionCacheService));
    private readonly ILogger<LoginCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// 处理登录命令
    /// </summary>
    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken) {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Email, "邮箱");
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Password, "密码");

        var user = await _userRepository.FindByEmailAsync(request.Email, cancellationToken);

        if (user == null || !PasswordUtil.VerifyPassword(user.PasswordHash, request.Password)) {
            _logger.LogWarning("登录失败: 邮箱或密码错误 | Email: {Email}", request.Email);
            throw new ArgumentException("邮箱或密码错误");
        }

        var roleCodes = await _userRoleRepository.GetUserRoleCodesAsync(user.Id, cancellationToken);

        var permissionCodes = await _permissionDomainService.GetUserPermissionCodesAsync(user.Id, cancellationToken);
        if (permissionCodes.Count > 0) {
            await _permissionCacheService.SetUserPermissionsAsync(user.Id, new HashSet<string>(permissionCodes));
            _logger.LogInformation("用户权限预加载成功 | UserId: {UserId} | PermissionCount: {Count}", user.Id, permissionCodes.Count);
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, GuidExtensions.NewShortGuid())
        };

        var accessToken = _jwtService.GenerateToken(claims);
        var refreshToken = _jwtService.GenerateRefreshToken();

        _logger.LogInformation("用户登录成功 | UserId: {UserId} | Email: {Email} | Roles: {Roles}",
            user.Id, user.Email, string.Join(",", roleCodes));

        return new LoginResponseDto {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600,
            User = new LoginUserInfoDto {
                Id = user.Id,
                Email = user.Email,
                NickName = user.NickName,
                Avatar = user.AvatarUrl,
                Roles = roleCodes
            }
        };
    }
}

/// <summary>
/// 用户注册命令处理器
/// <para>处理用户注册请求，创建新用户账号</para>
/// </summary>
public class RegisterCommandHandler(
    IUserRepository userRepository,
    ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommand, bool> {
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly ILogger<RegisterCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// 处理注册命令
    /// </summary>
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken) {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Email, "邮箱");
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Password, "密码");

        if (await _userRepository.IsEmailExistsAsync(request.Email, cancellationToken)) {
            _logger.LogWarning("注册失败: 邮箱已被注册 | Email: {Email}", request.Email);
            throw new ArgumentException("邮箱已被注册");
        }

        var user = new User {
            Email = request.Email,
            PasswordHash = PasswordUtil.HashPassword(request.Password),
            NickName = request.NickName,
            Phone = request.Phone
        };

        var result = await _userRepository.InsertAsync([user], cancellationToken);

        if (result) {
            _logger.LogInformation("用户注册成功 | UserId: {UserId} | Email: {Email} | NickName: {NickName}",
                user.Id, user.Email, user.NickName);
        }

        return result;
    }
}

/// <summary>
/// 刷新令牌命令处理器
/// <para>处理刷新令牌请求，生成新的访问令牌</para>
/// </summary>
public class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IJwtService jwtService,
    ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommand, LoginResponseDto> {
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
    private readonly IJwtService _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    private readonly ILogger<RefreshTokenCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// 处理刷新令牌命令
    /// </summary>
    public async Task<LoginResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.AccessToken, "访问令牌");
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RefreshToken, "刷新令牌");

        var claims = _jwtService.GetClaimsFromToken(request.AccessToken);
        if (claims == null) {
            _logger.LogWarning("刷新令牌失败: 无效的访问令牌");
            throw new ArgumentException("无效的访问令牌");
        }

        var userIdClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId)) {
            _logger.LogWarning("刷新令牌失败: 无效的用户标识");
            throw new ArgumentException("无效的访问令牌");
        }

        var user = await _userRepository.GetAsync(userId, cancellationToken);

        if (user == null) {
            _logger.LogWarning("刷新令牌失败: 用户不存在 | UserId: {UserId}", userId);
            throw new ArgumentException("用户不存在");
        }

        var roleCodes = await _userRoleRepository.GetUserRoleCodesAsync(user.Id, cancellationToken);

        var newClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, GuidExtensions.NewShortGuid())
        };

        var accessToken = _jwtService.GenerateToken(newClaims);
        var refreshToken = _jwtService.GenerateRefreshToken();

        _logger.LogInformation("令牌刷新成功 | UserId: {UserId} | Email: {Email}", user.Id, user.Email);

        return new LoginResponseDto {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600,
            User = new LoginUserInfoDto {
                Id = user.Id,
                Email = user.Email,
                NickName = user.NickName,
                Avatar = user.AvatarUrl,
                Roles = roleCodes
            }
        };
    }
}

/// <summary>
/// 用户登出命令处理器
/// <para>处理用户登出请求，将令牌加入黑名单</para>
/// </summary>
public class LogoutCommandHandler(IJwtService jwtService, ILogger<LogoutCommandHandler> logger) : IRequestHandler<LogoutCommand, bool> {
    private readonly IJwtService _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    private readonly ILogger<LogoutCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// 处理登出命令
    /// </summary>
    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken) {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Token, "令牌");

        var claims = _jwtService.GetClaimsFromToken(request.Token);
        var expClaim = claims?.FirstOrDefault(c => c.Type == "exp");

        if (expClaim == null) {
            _logger.LogWarning("登出失败: 无效的令牌");
            throw new ArgumentException("无效的令牌");
        }

        var userIdClaim = claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        var userId = userIdClaim?.Value ?? "unknown";

        var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));
        var result = await _jwtService.AddTokenToBlacklistAsync(request.Token, expTime.UtcDateTime);

        if (result) {
            _logger.LogInformation("用户登出成功 | UserId: {UserId}", userId);
        }

        return result;
    }
}
