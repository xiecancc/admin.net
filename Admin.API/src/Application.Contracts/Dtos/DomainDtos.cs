/*
 * 文件名称: DomainDtos.cs
 * 功能描述: 领域模型数据传输对象基类，包含基础模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

namespace Application.Contracts.Dtos;

/// <summary>
/// 领域模型创建 DTO
/// <para>用于所有创建操作的数据传输对象</para>
/// </summary>
public abstract class CreateDto;

/// <summary>
/// 领域模型更新 DTO
/// <para>用于所有更新操作的数据传输对象</para>
/// </summary>
public abstract class UpdateDto;

/// <summary>
/// 领域模型列表 DTO
/// <para>用于列表展示的数据传输对象</para>
/// </summary>
public abstract class ListDto;

/// <summary>
/// 领域模型详情 DTO
/// <para>用于详细信息展示的数据传输对象</para>
/// </summary>
public abstract class DetailDto;

/// <summary>
/// 领域模型分页 DTO
/// <para>用于分页响应中的数据传输对象</para>
/// </summary>
public abstract class PagedDto;

/// <summary>
/// 领域模型操作 DTO
/// <para>用于简单操作的数据传输对象</para>
/// </summary>
public abstract class ActionDto;
