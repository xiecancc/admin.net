/*
 * 文件名称: RateLimitOptionTests.cs
 * 功能描述: 限流配置选项测试类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Infrastructure.Shared.Options;

namespace Infrastructure.Test.Options;

/// <summary>
/// 限流配置选项测试类
/// <para>测试限流配置选项的验证逻辑，包括策略验证、端点配置验证等</para>
/// </summary>
public class RateLimitOptionTests {
    #region RateLimitOption 验证测试

    /// <summary>
    /// 测试使用有效配置进行验证
    /// <para>预期结果：验证通过，不抛出异常</para>
    /// </summary>
    [Fact]
    public void Validate_WithValidConfiguration_ShouldNotThrow() {
        var option = new RateLimitOption {
            Enabled = true,
            PartitionType = "Ip",
            DefaultPolicy = new RateLimitPolicyOption {
                Name = "Default",
                Algorithm = "FixedWindow",
                PermitLimit = 100,
                WindowSeconds = 60
            }
        };

        var exception = Record.Exception(() => option.Validate());
        Assert.Null(exception);
    }

    /// <summary>
    /// 测试默认策略为空时的验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Fact]
    public void Validate_WithNullDefaultPolicy_ShouldThrowInvalidOperationException() {
        var option = new RateLimitOption {
            Enabled = true,
            PartitionType = "Ip",
            DefaultPolicy = null!
        };

        var exception = Assert.Throws<InvalidOperationException>(() => option.Validate());
        Assert.Contains("默认限流策略必须配置", exception.Message);
    }

    /// <summary>
    /// 测试使用 Header 分区类型但未配置 Header 名称时的验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Fact]
    public void Validate_WithHeaderPartitionTypeButNoHeaderName_ShouldThrowInvalidOperationException() {
        var option = new RateLimitOption {
            Enabled = true,
            PartitionType = "Header",
            HeaderName = null,
            DefaultPolicy = new RateLimitPolicyOption {
                Name = "Default",
                Algorithm = "FixedWindow",
                PermitLimit = 100,
                WindowSeconds = 60
            }
        };

        var exception = Assert.Throws<InvalidOperationException>(() => option.Validate());
        Assert.Contains("Header 名称必须配置", exception.Message);
    }

    /// <summary>
    /// 测试使用 Header 分区类型并配置了 Header 名称时的验证
    /// <para>预期结果：验证通过，不抛出异常</para>
    /// </summary>
    [Fact]
    public void Validate_WithHeaderPartitionTypeAndHeaderName_ShouldNotThrow() {
        var option = new RateLimitOption {
            Enabled = true,
            PartitionType = "Header",
            HeaderName = "X-Client-Id",
            DefaultPolicy = new RateLimitPolicyOption {
                Name = "Default",
                Algorithm = "FixedWindow",
                PermitLimit = 100,
                WindowSeconds = 60
            }
        };

        var exception = Record.Exception(() => option.Validate());
        Assert.Null(exception);
    }

    /// <summary>
    /// 测试使用有效的端点策略配置时的验证
    /// <para>预期结果：验证通过，不抛出异常</para>
    /// </summary>
    [Fact]
    public void Validate_WithValidEndpointPolicies_ShouldNotThrow() {
        var option = new RateLimitOption {
            Enabled = true,
            PartitionType = "Ip",
            DefaultPolicy = new RateLimitPolicyOption {
                Name = "Default",
                Algorithm = "FixedWindow",
                PermitLimit = 100,
                WindowSeconds = 60
            },
            EndpointPolicies = [
                new EndpointRateLimitOption {
                    PathPattern = "/api/auth/login",
                    Method = "POST",
                    PolicyName = "LoginPolicy",
                    Algorithm = "FixedWindow",
                    PermitLimit = 10,
                    WindowSeconds = 60
                }
            ]
        };

        var exception = Record.Exception(() => option.Validate());
        Assert.Null(exception);
    }

    /// <summary>
    /// 测试端点策略名称重复时的验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Fact]
    public void Validate_WithDuplicatePolicyNames_ShouldThrowInvalidOperationException() {
        var option = new RateLimitOption {
            Enabled = true,
            PartitionType = "Ip",
            DefaultPolicy = new RateLimitPolicyOption {
                Name = "Default",
                Algorithm = "FixedWindow",
                PermitLimit = 100,
                WindowSeconds = 60
            },
            EndpointPolicies = [
                new EndpointRateLimitOption {
                    PathPattern = "/api/auth/login",
                    Method = "POST",
                    PolicyName = "DuplicatePolicy",
                    Algorithm = "FixedWindow",
                    PermitLimit = 10,
                    WindowSeconds = 60
                },
                new EndpointRateLimitOption {
                    PathPattern = "/api/auth/register",
                    Method = "POST",
                    PolicyName = "DuplicatePolicy",
                    Algorithm = "FixedWindow",
                    PermitLimit = 5,
                    WindowSeconds = 60
                }
            ]
        };

        var exception = Assert.Throws<InvalidOperationException>(() => option.Validate());
        Assert.Contains("端点限流策略名称必须唯一", exception.Message);
    }

    /// <summary>
    /// 测试使用有效的拒绝响应配置时的验证
    /// <para>预期结果：验证通过，不抛出异常</para>
    /// </summary>
    [Fact]
    public void Validate_WithValidOnRejectedConfiguration_ShouldNotThrow() {
        var option = new RateLimitOption {
            Enabled = true,
            PartitionType = "Ip",
            DefaultPolicy = new RateLimitPolicyOption {
                Name = "Default",
                Algorithm = "FixedWindow",
                PermitLimit = 100,
                WindowSeconds = 60
            },
            OnRejected = new RateLimitOnRejectedOption {
                StatusCode = 429,
                Message = "请求过于频繁，请稍后再试"
            }
        };

        var exception = Record.Exception(() => option.Validate());
        Assert.Null(exception);
    }

    #endregion

    #region RateLimitPolicyOption 验证测试

    /// <summary>
    /// 测试使用有效的请求数量限制进行验证
    /// <para>预期结果：验证通过，不抛出异常</para>
    /// </summary>
    [Fact]
    public void PolicyValidate_WithValidPermitLimit_ShouldNotThrow() {
        var policy = new RateLimitPolicyOption {
            Name = "Test",
            Algorithm = "FixedWindow",
            PermitLimit = 100,
            WindowSeconds = 60
        };

        var exception = Record.Exception(() => policy.Validate());
        Assert.Null(exception);
    }

    /// <summary>
    /// 测试使用有效范围内的请求数量限制进行验证
    /// <para>预期结果：验证通过，不抛出异常</para>
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(10000)]
    public void PolicyValidate_WithValidPermitLimitRange_ShouldNotThrow(int permitLimit) {
        var policy = new RateLimitPolicyOption {
            Name = "Test",
            Algorithm = "FixedWindow",
            PermitLimit = permitLimit,
            WindowSeconds = 60
        };

        var exception = Record.Exception(() => policy.Validate());
        Assert.Null(exception);
    }

    /// <summary>
    /// 测试请求数量限制低于最小值时的验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Fact]
    public void PolicyValidate_WithPermitLimitBelowMin_ShouldThrowInvalidOperationException() {
        var policy = new RateLimitPolicyOption {
            Name = "Test",
            Algorithm = "FixedWindow",
            PermitLimit = 0,
            WindowSeconds = 60
        };

        var exception = Assert.Throws<InvalidOperationException>(() => policy.Validate());
        Assert.Contains("允许的请求数量必须在 1-10000 之间", exception.Message);
    }

    /// <summary>
    /// 测试请求数量限制超过最大值时的验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Fact]
    public void PolicyValidate_WithPermitLimitExceedingMax_ShouldThrowInvalidOperationException() {
        var policy = new RateLimitPolicyOption {
            Name = "Test",
            Algorithm = "FixedWindow",
            PermitLimit = 10001,
            WindowSeconds = 60
        };

        var exception = Assert.Throws<InvalidOperationException>(() => policy.Validate());
        Assert.Contains("允许的请求数量必须在 1-10000 之间", exception.Message);
    }

    /// <summary>
    /// 测试使用有效的时间窗口进行验证
    /// <para>预期结果：验证通过，不抛出异常</para>
    /// </summary>
    [Fact]
    public void PolicyValidate_WithValidWindowSeconds_ShouldNotThrow() {
        var policy = new RateLimitPolicyOption {
            Name = "Test",
            Algorithm = "FixedWindow",
            PermitLimit = 100,
            WindowSeconds = 60
        };

        var exception = Record.Exception(() => policy.Validate());
        Assert.Null(exception);
    }

    /// <summary>
    /// 测试使用有效范围内的时间窗口进行验证
    /// <para>预期结果：验证通过，不抛出异常</para>
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(60)]
    [InlineData(3600)]
    public void PolicyValidate_WithValidWindowSecondsRange_ShouldNotThrow(int windowSeconds) {
        var policy = new RateLimitPolicyOption {
            Name = "Test",
            Algorithm = "FixedWindow",
            PermitLimit = 100,
            WindowSeconds = windowSeconds
        };

        var exception = Record.Exception(() => policy.Validate());
        Assert.Null(exception);
    }

    /// <summary>
    /// 测试时间窗口低于最小值时的验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Fact]
    public void PolicyValidate_WithWindowSecondsBelowMin_ShouldThrowInvalidOperationException() {
        var policy = new RateLimitPolicyOption {
            Name = "Test",
            Algorithm = "FixedWindow",
            PermitLimit = 100,
            WindowSeconds = 0
        };

        var exception = Assert.Throws<InvalidOperationException>(() => policy.Validate());
        Assert.Contains("时间窗口必须在 1-3600 秒之间", exception.Message);
    }

    /// <summary>
    /// 测试时间窗口超过最大值时的验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Fact]
    public void PolicyValidate_WithWindowSecondsExceedingMax_ShouldThrowInvalidOperationException() {
        var policy = new RateLimitPolicyOption {
            Name = "Test",
            Algorithm = "FixedWindow",
            PermitLimit = 100,
            WindowSeconds = 3601
        };

        var exception = Assert.Throws<InvalidOperationException>(() => policy.Validate());
        Assert.Contains("时间窗口必须在 1-3600 秒之间", exception.Message);
    }

    /// <summary>
    /// 测试使用有效范围内的窗口分段数进行验证
    /// <para>预期结果：验证通过，不抛出异常</para>
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void PolicyValidate_WithValidSegmentsPerWindowRange_ShouldNotThrow(int segmentsPerWindow) {
        var policy = new RateLimitPolicyOption {
            Name = "Test",
            Algorithm = "SlidingWindow",
            PermitLimit = 100,
            WindowSeconds = 60,
            SegmentsPerWindow = segmentsPerWindow
        };

        var exception = Record.Exception(() => policy.Validate());
        Assert.Null(exception);
    }

    /// <summary>
    /// 测试窗口分段数超过最大值时的验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Fact]
    public void PolicyValidate_WithSegmentsPerWindowExceedingMax_ShouldThrowInvalidOperationException() {
        var policy = new RateLimitPolicyOption {
            Name = "Test",
            Algorithm = "SlidingWindow",
            PermitLimit = 100,
            WindowSeconds = 60,
            SegmentsPerWindow = 11
        };

        var exception = Assert.Throws<InvalidOperationException>(() => policy.Validate());
        Assert.Contains("窗口分段数必须在 1-10 之间", exception.Message);
    }

    /// <summary>
    /// 测试使用有效范围内的排队限制进行验证
    /// <para>预期结果：验证通过，不抛出异常</para>
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(1000)]
    public void PolicyValidate_WithValidQueueLimitRange_ShouldNotThrow(int queueLimit) {
        var policy = new RateLimitPolicyOption {
            Name = "Test",
            Algorithm = "FixedWindow",
            PermitLimit = 100,
            WindowSeconds = 60,
            QueueLimit = queueLimit
        };

        var exception = Record.Exception(() => policy.Validate());
        Assert.Null(exception);
    }

    /// <summary>
    /// 测试排队限制超过最大值时的验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Fact]
    public void PolicyValidate_WithQueueLimitExceedingMax_ShouldThrowInvalidOperationException() {
        var policy = new RateLimitPolicyOption {
            Name = "Test",
            Algorithm = "FixedWindow",
            PermitLimit = 100,
            WindowSeconds = 60,
            QueueLimit = 1001
        };

        var exception = Assert.Throws<InvalidOperationException>(() => policy.Validate());
        Assert.Contains("排队限制必须在 0-1000 之间", exception.Message);
    }

    #endregion

    #region EndpointRateLimitOption 验证测试

    /// <summary>
    /// 测试使用有效的端点配置进行验证
    /// <para>预期结果：验证通过，不抛出异常</para>
    /// </summary>
    [Fact]
    public void EndpointValidate_WithValidConfiguration_ShouldNotThrow() {
        var endpoint = new EndpointRateLimitOption {
            PathPattern = "/api/auth/login",
            Method = "POST",
            PolicyName = "LoginPolicy",
            Algorithm = "FixedWindow",
            PermitLimit = 10,
            WindowSeconds = 60
        };

        var exception = Record.Exception(() => endpoint.Validate());
        Assert.Null(exception);
    }

    /// <summary>
    /// 测试路径模式为空时的验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Fact]
    public void EndpointValidate_WithNullPathPattern_ShouldThrowInvalidOperationException() {
        var endpoint = new EndpointRateLimitOption {
            PathPattern = null!,
            Method = "POST",
            Algorithm = "FixedWindow",
            PermitLimit = 10,
            WindowSeconds = 60
        };

        var exception = Assert.Throws<InvalidOperationException>(() => endpoint.Validate());
        Assert.Contains("路径模式必须配置", exception.Message);
    }

    /// <summary>
    /// 测试路径模式为空字符串时的验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Fact]
    public void EndpointValidate_WithEmptyPathPattern_ShouldThrowInvalidOperationException() {
        var endpoint = new EndpointRateLimitOption {
            PathPattern = "",
            Method = "POST",
            Algorithm = "FixedWindow",
            PermitLimit = 10,
            WindowSeconds = 60
        };

        var exception = Assert.Throws<InvalidOperationException>(() => endpoint.Validate());
        Assert.Contains("路径模式必须配置", exception.Message);
    }

    /// <summary>
    /// 测试 HTTP 方法为空时的验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Fact]
    public void EndpointValidate_WithNullMethod_ShouldThrowInvalidOperationException() {
        var endpoint = new EndpointRateLimitOption {
            PathPattern = "/api/auth/login",
            Method = null!,
            Algorithm = "FixedWindow",
            PermitLimit = 10,
            WindowSeconds = 60
        };

        var exception = Assert.Throws<InvalidOperationException>(() => endpoint.Validate());
        Assert.Contains("HTTP 方法必须配置", exception.Message);
    }

    /// <summary>
    /// 测试 HTTP 方法为空字符串时的验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Fact]
    public void EndpointValidate_WithEmptyMethod_ShouldThrowInvalidOperationException() {
        var endpoint = new EndpointRateLimitOption {
            PathPattern = "/api/auth/login",
            Method = "",
            Algorithm = "FixedWindow",
            PermitLimit = 10,
            WindowSeconds = 60
        };

        var exception = Assert.Throws<InvalidOperationException>(() => endpoint.Validate());
        Assert.Contains("HTTP 方法必须配置", exception.Message);
    }

    #endregion

    #region RateLimitOnRejectedOption 验证测试

    /// <summary>
    /// 测试使用有效的拒绝响应配置进行验证
    /// <para>预期结果：验证通过，不抛出异常</para>
    /// </summary>
    [Fact]
    public void OnRejectedValidate_WithValidConfiguration_ShouldNotThrow() {
        var onRejected = new RateLimitOnRejectedOption {
            StatusCode = 429,
            Message = "请求过于频繁，请稍后再试"
        };

        var exception = Record.Exception(() => onRejected.Validate());
        Assert.Null(exception);
    }

    /// <summary>
    /// 测试使用有效范围内的状态码进行验证
    /// <para>预期结果：验证通过，不抛出异常</para>
    /// </summary>
    [Theory]
    [InlineData(100)]
    [InlineData(429)]
    [InlineData(999)]
    public void OnRejectedValidate_WithValidStatusCodeRange_ShouldNotThrow(int statusCode) {
        var onRejected = new RateLimitOnRejectedOption {
            StatusCode = statusCode,
            Message = "请求过于频繁"
        };

        var exception = Record.Exception(() => onRejected.Validate());
        Assert.Null(exception);
    }

    /// <summary>
    /// 测试使用无效状态码进行验证
    /// <para>预期结果：抛出 InvalidOperationException 异常</para>
    /// </summary>
    [Theory]
    [InlineData(99)]
    [InlineData(1000)]
    public void OnRejectedValidate_WithInvalidStatusCode_ShouldThrowInvalidOperationException(int statusCode) {
        var onRejected = new RateLimitOnRejectedOption {
            StatusCode = statusCode,
            Message = "请求过于频繁"
        };

        var exception = Assert.Throws<InvalidOperationException>(() => onRejected.Validate());
        Assert.Contains("HTTP 状态码必须在 100-999 之间", exception.Message);
    }

    #endregion

    #region 枚举默认值测试

    /// <summary>
    /// 测试限流选项的默认值
    /// <para>预期结果：默认值符合预期</para>
    /// </summary>
    [Fact]
    public void RateLimitOption_DefaultValues_ShouldBeCorrect() {
        var option = new RateLimitOption();

        Assert.True(option.Enabled);
        Assert.Null(option.PartitionType);
        Assert.Equal(RateLimitPartitionType.Ip, option.PartitionTypeEnum);
        Assert.Null(option.HeaderName);
        Assert.Null(option.OnRejected);
        Assert.Null(option.EndpointPolicies);
    }

    /// <summary>
    /// 测试限流策略选项的默认值
    /// <para>预期结果：默认值符合预期</para>
    /// </summary>
    [Fact]
    public void RateLimitPolicyOption_DefaultValues_ShouldBeCorrect() {
        var policy = new RateLimitPolicyOption();

        Assert.Null(policy.Algorithm);
        Assert.Equal(RateLimitAlgorithmType.FixedWindow, policy.AlgorithmEnum);
        Assert.Null(policy.QueueProcessingOrder);
        Assert.Equal(RateLimitQueueProcessingOrder.OldestFirst, policy.QueueProcessingOrderEnum);
        Assert.Equal(0, policy.PermitLimit);
        Assert.Equal(0, policy.WindowSeconds);
        Assert.Equal(0, policy.SegmentsPerWindow);
        Assert.Equal(0, policy.QueueLimit);
    }

    /// <summary>
    /// 测试端点限流选项的默认值
    /// <para>预期结果：默认值符合预期</para>
    /// </summary>
    [Fact]
    public void EndpointRateLimitOption_DefaultValues_ShouldBeCorrect() {
        var endpoint = new EndpointRateLimitOption();

        Assert.Null(endpoint.Algorithm);
        Assert.Equal(RateLimitAlgorithmType.FixedWindow, endpoint.AlgorithmEnum);
        Assert.Equal(0, endpoint.PermitLimit);
        Assert.Equal(0, endpoint.WindowSeconds);
        Assert.Equal(0, endpoint.SegmentsPerWindow);
        Assert.Equal(0, endpoint.QueueLimit);
    }

    /// <summary>
    /// 测试拒绝响应选项的默认值
    /// <para>预期结果：默认值符合预期</para>
    /// </summary>
    [Fact]
    public void RateLimitOnRejectedOption_DefaultValues_ShouldBeCorrect() {
        var onRejected = new RateLimitOnRejectedOption();

        Assert.Equal(429, onRejected.StatusCode);
        Assert.Null(onRejected.Message);
    }

    #endregion

    #region 枚举转换测试

    /// <summary>
    /// 测试算法枚举从字符串转换
    /// <para>预期结果：正确转换为对应的枚举值</para>
    /// </summary>
    [Theory]
    [InlineData("FixedWindow", RateLimitAlgorithmType.FixedWindow)]
    [InlineData("SlidingWindow", RateLimitAlgorithmType.SlidingWindow)]
    [InlineData("TokenBucket", RateLimitAlgorithmType.TokenBucket)]
    [InlineData("Concurrency", RateLimitAlgorithmType.Concurrency)]
    public void AlgorithmEnum_ShouldConvertFromString(string algorithmString, RateLimitAlgorithmType expected) {
        var policy = new RateLimitPolicyOption {
            Algorithm = algorithmString,
            PermitLimit = 100,
            WindowSeconds = 60
        };

        Assert.Equal(expected, policy.AlgorithmEnum);
    }

    /// <summary>
    /// 测试分区类型枚举从字符串转换
    /// <para>预期结果：正确转换为对应的枚举值</para>
    /// </summary>
    [Theory]
    [InlineData("User", RateLimitPartitionType.User)]
    [InlineData("Ip", RateLimitPartitionType.Ip)]
    [InlineData("Global", RateLimitPartitionType.Global)]
    [InlineData("Header", RateLimitPartitionType.Header)]
    public void PartitionTypeEnum_ShouldConvertFromString(string partitionString, RateLimitPartitionType expected) {
        var option = new RateLimitOption {
            PartitionType = partitionString,
            DefaultPolicy = new RateLimitPolicyOption {
                PermitLimit = 100,
                WindowSeconds = 60
            }
        };

        Assert.Equal(expected, option.PartitionTypeEnum);
    }

    /// <summary>
    /// 测试队列处理顺序枚举从字符串转换
    /// <para>预期结果：正确转换为对应的枚举值</para>
    /// </summary>
    [Theory]
    [InlineData("OldestFirst", RateLimitQueueProcessingOrder.OldestFirst)]
    [InlineData("NewestFirst", RateLimitQueueProcessingOrder.NewestFirst)]
    public void QueueProcessingOrderEnum_ShouldConvertFromString(string orderString, RateLimitQueueProcessingOrder expected) {
        var policy = new RateLimitPolicyOption {
            QueueProcessingOrder = orderString,
            PermitLimit = 100,
            WindowSeconds = 60
        };

        Assert.Equal(expected, policy.QueueProcessingOrderEnum);
    }

    /// <summary>
    /// 测试算法枚举为空时返回默认值
    /// <para>预期结果：返回默认的 FixedWindow 枚举值</para>
    /// </summary>
    [Fact]
    public void AlgorithmEnum_WithNullValue_ShouldReturnDefault() {
        var policy = new RateLimitPolicyOption {
            Algorithm = null,
            PermitLimit = 100,
            WindowSeconds = 60
        };

        Assert.Equal(RateLimitAlgorithmType.FixedWindow, policy.AlgorithmEnum);
    }

    /// <summary>
    /// 测试分区类型枚举为空时返回默认值
    /// <para>预期结果：返回默认的 Ip 枚举值</para>
    /// </summary>
    [Fact]
    public void PartitionTypeEnum_WithNullValue_ShouldReturnDefault() {
        var option = new RateLimitOption {
            PartitionType = null,
            DefaultPolicy = new RateLimitPolicyOption {
                PermitLimit = 100,
                WindowSeconds = 60
            }
        };

        Assert.Equal(RateLimitPartitionType.Ip, option.PartitionTypeEnum);
    }

    /// <summary>
    /// 测试算法枚举为无效值时返回默认值
    /// <para>预期结果：返回默认的 FixedWindow 枚举值</para>
    /// </summary>
    [Fact]
    public void AlgorithmEnum_WithInvalidValue_ShouldReturnDefault() {
        var policy = new RateLimitPolicyOption {
            Algorithm = "InvalidAlgorithm",
            PermitLimit = 100,
            WindowSeconds = 60
        };

        Assert.Equal(RateLimitAlgorithmType.FixedWindow, policy.AlgorithmEnum);
    }

    #endregion
}
