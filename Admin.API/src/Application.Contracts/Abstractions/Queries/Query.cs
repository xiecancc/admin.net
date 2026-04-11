/*
 * 文件名称: DomainQuery.cs
 * 功能描述: 请求基类，所有命令和查询的基础类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

namespace Application.Contracts.Abstractions.Queries;

/// <summary>
/// 查询基类
/// 所有查询的基础类，用于领域实体（包括关系表）的查询操作
/// </summary>
/// <typeparam name="TResponse">响应类型</typeparam>
public abstract class Query<TResponse> : Request<TResponse>;
