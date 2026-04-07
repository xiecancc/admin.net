using Domain.Shared.Dtos;

namespace Infrastructure.Shared.Contexts;

/// <summary>
/// HTTP 上下文提供者接口，用于获取当前请求的上下文信息
/// </summary>
public interface IHttpContextProvider {
    /// <summary>
    /// 当前用户 ID（已认证用户，未认证返回 null）
    /// </summary>
    Guid? UserId {
        get;
    }

    /// <summary>
    /// 是否已认证
    /// </summary>
    bool IsAuthenticated {
        get;
    }

    /// <summary>
    /// 请求追踪 ID
    /// </summary>
    string? TraceId {
        get;
    }

    /// <summary>
    /// 请求 ID
    /// </summary>
    string? RequestId {
        get;
    }

    /// <summary>
    /// 客户端 IP 地址
    /// </summary>
    string? ClientIp {
        get;
    }

    /// <summary>
    /// 用户代理
    /// </summary>
    string? UserAgent {
        get;
    }

    /// <summary>
    /// 请求路径
    /// </summary>
    string? RequestPath {
        get;
    }

    /// <summary>
    /// 请求方法
    /// </summary>
    string? RequestMethod {
        get;
    }

    /// <summary>
    /// 用户详细信息（从缓存/数据库获取）
    /// </summary>
    /// <value>用户的详细信息，未认证或获取失败返回 null</value>
    UserInfoDTO? UserInfo {
        get;
    }
}
