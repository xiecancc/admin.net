/*
 * 文件名称: DomainDtos.cs
 * 功能描述: 领域模型数据传输对象基类，包含基础模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Domain.Shared.Entities;
using SqlSugar;
using System.Linq.Expressions;

namespace Application.Contracts.Dtos;

/// <summary>
/// 领域模型创建 DTO
/// <para>用于所有创建操作的数据传输对象</para>
/// </summary>
public abstract class DomainCreateDto {

}

/// <summary>
/// 领域模型更新 DTO
/// <para>用于所有更新操作的数据传输对象</para>
/// </summary>
public abstract class DomainUpdateDto {

}

/// <summary>
/// 领域模型列表 DTO
/// <para>用于列表展示的数据传输对象</para>
/// </summary>
public abstract class DomainListDto {

}

/// <summary>
/// 领域模型详情 DTO
/// <para>用于详细信息展示的数据传输对象</para>
/// </summary>
public abstract class DomainDetailDto {

}

/// <summary>
/// 领域模型分页 DTO
/// <para>用于分页响应中的数据传输对象</para>
/// </summary>
public abstract class DomainPagedDto {

}

/// <summary>
/// 领域模型操作 DTO
/// <para>用于简单操作的数据传输对象</para>
/// </summary>
public class DomainActionDto {
}

/// <summary>
/// 领域模型查询参数 DTO
/// <para>用于所有领域模型的查询参数</para>
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
public class DomainQueryParameters<TDomain> where TDomain : DomainBase, new() {
    /// <summary>
    /// 查询条件列表
    /// </summary>
    public virtual List<Expression<Func<TDomain, bool>>> Predicates() {
        return [];
    }

    /// <summary>
    /// 排序字段列表
    /// </summary>
    public virtual IDictionary<Expression<Func<TDomain, object>>, OrderByType> Orders() {
        return new Dictionary<Expression<Func<TDomain, object>>, OrderByType>();
    }

    /// <summary>
    /// 分页参数
    /// </summary>
    /// <param name="page">页码</param>
    /// <param name="size">每页大小</param>
    /// <returns>分页参数</returns>
    public virtual (int page, int size) Pagination(int page, int size) {
        page = page > 0 ? page : 1;
        size = size > 0 ? size : 10;
        return (page, size);
    }

    /// <summary>
    /// 生成缓存键的参数部分
    /// <para>子类可重写此方法添加自定义参数</para>
    /// </summary>
    /// <returns>缓存键参数部分，基类返回空字符串</returns>
    public virtual string CacheKey() {
        return string.Empty;
    }
}
