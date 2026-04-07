/*
 * 文件名称: HttpContextProvider.cs
 * 功能描述: HTTP 上下文提供者实现，从 HttpContext 中获取上下文信息和用户信息
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-04
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Dtos;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Contexts;
using Infrastructure.Shared.Units;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Infrastructure.Contexts;

/// <summary>
/// HTTP 上下文提供者实现，从 HttpContext 中获取上下文信息
/// </summary>
/// <remarks>
/// <para>职责：</para>
/// <list type="bullet">
///   <item>提供当前请求的上下文信息</item>
///   <item>提供当前用户的信息（含缓存）</item>
/// </list>
/// <para>依赖：</para>
/// <list type="bullet">
///   <item>IHttpContextAccessor - HTTP 上下文访问器</item>
///   <item>ICacheProvider - 缓存提供者</item>
///   <item>IUnitOfWork - 工作单元</item>
/// </list>
/// </remarks>
/// <param name="httpContextAccessor">HTTP 上下文访问器</param>
/// <param name="cacheProvider">缓存提供者</param>
/// <param name="unitOfWork">工作单元</param>
public class HttpContextProvider(
    IHttpContextAccessor httpContextAccessor,
    ICacheProvider cacheProvider,
    IUnitOfWork unitOfWork) : IHttpContextProvider {
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private bool _userInfoLoaded;

    private HttpContext? HttpContext => _httpContextAccessor.HttpContext;

    /// <inheritdoc/>
    public Guid? UserId {
        get {
            var userIdClaim = HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
        }
    }

    /// <inheritdoc/>
    public bool IsAuthenticated => HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc/>
    public string? TraceId => Activity.Current?.TraceId.ToString() ?? HttpContext?.TraceIdentifier;

    /// <inheritdoc/>
    public string? RequestId => HttpContext?.TraceIdentifier;

    /// <inheritdoc/>
    public string? ClientIp => HttpContext?.Connection?.RemoteIpAddress?.ToString();

    /// <inheritdoc/>
    public string? UserAgent => HttpContext?.Request.Headers["User-Agent"].ToString();

    /// <inheritdoc/>
    public string? RequestPath => HttpContext?.Request.Path.Value;

    /// <inheritdoc/>
    public string? RequestMethod => HttpContext?.Request.Method;

    /// <inheritdoc/>
    public UserInfoDTO? UserInfo {
        get {
            if (!_userInfoLoaded) {
                field = LoadUserInfoAsync().GetAwaiter().GetResult();
                _userInfoLoaded = true;
            }
            return field;
        }
    }

    /// <summary>
    /// 从缓存或数据库加载用户信息
    /// </summary>
    /// <returns>用户信息 DTO，用户不存在则返回 null</returns>
    /// <remarks>
    /// <para>加载流程：</para>
    /// <list type="number">
    ///   <item>检查 UserId 是否存在</item>
    ///   <item>尝试从缓存获取用户信息</item>
    ///   <item>缓存未命中时从数据库加载</item>
    ///   <item>将用户信息缓存 30 分钟</item>
    /// </list>
    /// </remarks>
    private async Task<UserInfoDTO?> LoadUserInfoAsync() {
        if (UserId == null) {
            return null;
        }

        var cacheKey = $"user_info:{UserId.Value}";
        var cached = await _cacheProvider.GetAsync<UserInfoDTO>(cacheKey);
        if (cached != null) {
            return cached;
        }

        var userRepository = _unitOfWork.GetRepository<IUserRepository, User>();
        var user = await userRepository.GetAsync(UserId.Value);
        if (user == null) {
            return null;
        }

        var userRoleRepository = _unitOfWork.GetRepository<IUserRoleRepository, UserRole>();
        var roleIds = await userRoleRepository.GetRoleIdsByUserIdAsync(UserId.Value);

        var roleRepository = _unitOfWork.GetRepository<IRoleRepository, Role>();
        List<Role> roles = [];
        if (roleIds.Count > 0) {
            var predicates = new List<Expression<Func<Role, bool>>> { r => roleIds.Contains(r.Id) };
            roles = await roleRepository.GetListAsync(predicates: predicates);
        }

        var userInfo = new UserInfoDTO {
            Id = user.Id,
            Email = user.Email,
            NickName = user.NickName,
            Status = user.Status,
            Roles = roles.Select(r => r.Code).ToList().AsReadOnly()
        };

        await _cacheProvider.SetAsync(cacheKey, userInfo, TimeSpan.FromMinutes(30));
        return userInfo;
    }
}
