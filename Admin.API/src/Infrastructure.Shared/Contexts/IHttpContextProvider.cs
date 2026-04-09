namespace Infrastructure.Shared.Contexts;

/// <summary>
/// HTTP 上下文提供者接口，用于获取当前请求的上下文信息
/// </summary>
public interface IHttpContextProvider {
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
}
