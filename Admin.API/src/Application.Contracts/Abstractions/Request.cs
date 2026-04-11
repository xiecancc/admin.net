/*
 * 文件名称: Request.cs
 * 功能描述: 请求基类，所有命令和查询的基础类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using MediatR;

namespace Application.Contracts.Abstractions;

/// <summary>
/// 领域请求基类
/// 所有领域相关请求的基础类
/// </summary>
/// <typeparam name="TResponse">响应类型</typeparam>
public abstract class Request<TResponse> : IRequest<TResponse>;
