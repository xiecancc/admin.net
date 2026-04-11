/*
 * 文件名称: UserDtos.cs
 * 功能描述: 用户相关数据传输对象，包含创建、更新、列表等模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

namespace Application.Contracts.Dtos;

/// <summary>
/// 用户创建 DTO
/// <para>用于创建用户</para>
/// </summary>
public class UserCreateDto : AggregateCreateDto {
    /// <summary>
    /// 邮箱（登录账号）
    /// </summary>
    /// <value>用户的邮箱地址，不能为空</value>
    public required string Email { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    /// <value>用户的密码，不能为空</value>
    public required string Password { get; set; } = string.Empty;

    /// <summary>
    /// 昵称
    /// </summary>
    /// <value>用户的昵称，可以为空</value>
    public string? NickName {
        get; set;
    }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <value>用户的手机号码，可以为空</value>
    public string? Phone {
        get; set;
    }

    /// <summary>
    /// 头像 URL
    /// </summary>
    /// <value>用户的头像地址，可以为空</value>
    public string? Avatar {
        get; set;
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>用户的状态，默认为 true</value>
    public bool Status { get; set; } = true;
}

/// <summary>
/// 用户更新 DTO
/// <para>用于更新用户</para>
/// </summary>
public class UserUpdateDto : AggregateUpdateDto {
    /// <summary>
    /// 昵称
    /// </summary>
    /// <value>用户的昵称，可以为空</value>
    public string? NickName {
        get; set;
    }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <value>用户的手机号码，可以为空</value>
    public string? Phone {
        get; set;
    }

    /// <summary>
    /// 头像 URL
    /// </summary>
    /// <value>用户的头像地址，可以为空</value>
    public string? Avatar {
        get; set;
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>用户的状态</value>
    public bool Status {
        get; set;
    }
}

/// <summary>
/// 用户列表 DTO
/// <para>用于列表展示</para>
/// </summary>
public class UserListDto : AggregateListDto {
    /// <summary>
    /// 邮箱
    /// </summary>
    /// <value>用户的邮箱地址</value>
    public required string Email { get; set; } = string.Empty;

    /// <summary>
    /// 昵称
    /// </summary>
    /// <value>用户的昵称，可以为空</value>
    public string? NickName {
        get; set;
    }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <value>用户的手机号码，可以为空</value>
    public string? Phone {
        get; set;
    }

    /// <summary>
    /// 头像 URL
    /// </summary>
    /// <value>用户的头像地址，可以为空</value>
    public string? Avatar {
        get; set;
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>用户的状态</value>
    public bool Status {
        get; set;
    }
}

/// <summary>
/// 用户详情 DTO
/// <para>用于详细信息展示</para>
/// </summary>
public class UserDetailDto : AggregateDetailDto {
    /// <summary>
    /// 邮箱
    /// </summary>
    /// <value>用户的邮箱地址</value>
    public required string Email { get; set; } = string.Empty;

    /// <summary>
    /// 昵称
    /// </summary>
    /// <value>用户的昵称，可以为空</value>
    public string? NickName {
        get; set;
    }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <value>用户的手机号码，可以为空</value>
    public string? Phone {
        get; set;
    }

    /// <summary>
    /// 头像 URL
    /// </summary>
    /// <value>用户的头像地址，可以为空</value>
    public string? Avatar {
        get; set;
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>用户的状态</value>
    public bool Status {
        get; set;
    }

    /// <summary>
    /// 角色列表
    /// </summary>
    /// <value>用户拥有的角色 ID 列表</value>
    public List<Guid> RoleIds { get; set; } = [];
}

/// <summary>
/// 用户分页 DTO
/// <para>用于分页响应中的列表数据</para>
/// </summary>
public class UserPagedDto : AggregatePagedDto {
    /// <summary>
    /// 邮箱
    /// </summary>
    /// <value>用户的邮箱地址</value>
    public required string Email { get; set; } = string.Empty;

    /// <summary>
    /// 昵称
    /// </summary>
    /// <value>用户的昵称，可以为空</value>
    public string? NickName {
        get; set;
    }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <value>用户的手机号码，可以为空</value>
    public string? Phone {
        get; set;
    }

    /// <summary>
    /// 头像 URL
    /// </summary>
    /// <value>用户的头像地址，可以为空</value>
    public string? Avatar {
        get; set;
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>用户的状态</value>
    public bool Status {
        get; set;
    }
}

/// <summary>
/// 用户操作 DTO
/// <para>用于简单操作的数据传输对象</para>
/// </summary>
public class UserActionDto : AggregateActionDto {
}
