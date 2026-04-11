/*
 * 文件名称: JwtServiceTests.cs
 * 功能描述: JWT 服务测试类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Infrastructure.Services;
using Infrastructure.Shared.Options;
using Infrastructure.Shared.Caches;

namespace API.Test.Infrastructure;

/// <summary>
/// JWT 服务测试类
/// <para>测试 JWT 服务的各项功能，包括令牌生成、验证、黑名单等</para>
/// </summary>
public class JwtServiceTests {
    private readonly Mock<IOptions<JwtOption>> _mockJwtOptions;
    private readonly Mock<ICacheProvider> _mockCacheProvider;
    private readonly JwtOption _jwtOption;

    public JwtServiceTests() {
        _jwtOption = new JwtOption {
            SecretKey = "ThisIsAVeryLongSecretKeyForTesting1234567890",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiresInMinutes = 60
        };

        _mockJwtOptions = new Mock<IOptions<JwtOption>>();
        _mockJwtOptions.Setup(x => x.Value).Returns(_jwtOption);

        _mockCacheProvider = new Mock<ICacheProvider>();
    }

    /// <summary>
    /// 测试使用有效声明生成令牌
    /// <para>预期结果：返回非空且非空的令牌字符串</para>
    /// </summary>
    [Fact]
    public void GenerateToken_WithValidClaims_ShouldReturnToken() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = jwtService.GenerateToken(claims);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    /// <summary>
    /// 测试使用有效声明生成有效的 JWT 令牌
    /// <para>预期结果：返回包含正确颁发者、受众和声明的有效 JWT 令牌</para>
    /// </summary>
    [Fact]
    public void GenerateToken_WithValidClaims_ShouldReturnValidJwtToken() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = jwtService.GenerateToken(claims);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.NotNull(jwtToken);
        Assert.Equal(_jwtOption.Issuer, jwtToken.Issuer);
        Assert.Equal(_jwtOption.Audience, jwtToken.Audiences.First());
        Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    /// <summary>
    /// 测试生成的令牌不包含可变数据
    /// <para>预期结果：令牌中不包含邮箱、姓名、角色等可变声明</para>
    /// </summary>
    [Fact]
    public void GenerateToken_ShouldNotContainMutableData() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = jwtService.GenerateToken(claims);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.DoesNotContain(jwtToken.Claims, c => c.Type == ClaimTypes.Email);
        Assert.DoesNotContain(jwtToken.Claims, c => c.Type == ClaimTypes.Name);
        Assert.DoesNotContain(jwtToken.Claims, c => c.Type == ClaimTypes.Role);
        Assert.DoesNotContain(jwtToken.Claims, c => c.Type == "email");
        Assert.DoesNotContain(jwtToken.Claims, c => c.Type == "name");
        Assert.DoesNotContain(jwtToken.Claims, c => c.Type == "role");
    }

    /// <summary>
    /// 测试使用有效令牌进行异步验证
    /// <para>预期结果：返回已认证的 ClaimsPrincipal 对象</para>
    /// </summary>
    [Fact]
    public async Task ValidateTokenAsync_WithValidToken_ShouldReturnClaimsPrincipal() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var token = jwtService.GenerateToken(claims);

        _mockCacheProvider.Setup(x => x.GetAsync<string>(It.IsAny<string>()))
            .ReturnsAsync((string?)null);

        var principal = await jwtService.ValidateTokenAsync(token);

        Assert.NotNull(principal);
        Assert.NotNull(principal.Identity);
        Assert.True(principal.Identity.IsAuthenticated);
    }

    /// <summary>
    /// 测试使用黑名单令牌进行异步验证
    /// <para>预期结果：返回 null，表示令牌无效</para>
    /// </summary>
    [Fact]
    public async Task ValidateTokenAsync_WithBlacklistedToken_ShouldReturnNull() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var token = jwtService.GenerateToken(claims);

        _mockCacheProvider.Setup(x => x.GetAsync<bool>(It.IsAny<string>()))
            .ReturnsAsync(true);

        var principal = await jwtService.ValidateTokenAsync(token);

        Assert.Null(principal);
    }

    /// <summary>
    /// 测试使用无效令牌进行验证
    /// <para>预期结果：返回 null，表示令牌无效</para>
    /// </summary>
    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnNull() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);

        var principal = jwtService.ValidateToken("invalid_token");

        Assert.Null(principal);
    }

    /// <summary>
    /// 测试从有效令牌中提取声明
    /// <para>预期结果：返回包含用户 ID 声明的声明集合</para>
    /// </summary>
    [Fact]
    public void GetClaimsFromToken_WithValidToken_ShouldReturnClaims() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var token = jwtService.GenerateToken(claims);

        var tokenClaims = jwtService.GetClaimsFromToken(token);

        Assert.NotNull(tokenClaims);
        Assert.Contains(tokenClaims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
    }

    /// <summary>
    /// 测试从有效令牌中获取特定声明值
    /// <para>预期结果：返回正确的用户 ID 值</para>
    /// </summary>
    [Fact]
    public void GetClaimValue_WithValidToken_ShouldReturnValue() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString())
        };
        var token = jwtService.GenerateToken(claims);

        var claimValue = jwtService.GetClaimValue(token, JwtRegisteredClaimNames.Sub);

        Assert.Equal(userId.ToString(), claimValue);
    }

    /// <summary>
    /// 测试检查有效令牌是否过期
    /// <para>预期结果：返回 false，表示令牌未过期</para>
    /// </summary>
    [Fact]
    public void IsTokenExpired_WithValidToken_ShouldReturnFalse() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString())
        };
        var token = jwtService.GenerateToken(claims);

        var isExpired = jwtService.IsTokenExpired(token);

        Assert.False(isExpired);
    }

    /// <summary>
    /// 测试生成刷新令牌
    /// <para>预期结果：返回非空且非空的刷新令牌字符串</para>
    /// </summary>
    [Fact]
    public void GenerateRefreshToken_ShouldReturnNonEmptyString() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);

        var refreshToken = jwtService.GenerateRefreshToken();

        Assert.NotNull(refreshToken);
        Assert.NotEmpty(refreshToken);
    }

    /// <summary>
    /// 测试将令牌添加到黑名单
    /// <para>预期结果：返回 true，表示添加成功</para>
    /// </summary>
    [Fact]
    public async Task AddTokenToBlacklistAsync_ShouldReturnTrue() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);
        var token = "test_token";
        var expiration = DateTime.UtcNow.AddHours(1);

        _mockCacheProvider.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .Returns(Task.FromResult(true));

        var result = await jwtService.AddTokenToBlacklistAsync(token, expiration);

        Assert.True(result);
    }

    /// <summary>
    /// 测试缓存失败时将令牌添加到黑名单
    /// <para>预期结果：返回 false，表示添加失败</para>
    /// </summary>
    [Fact]
    public async Task AddTokenToBlacklistAsync_ShouldReturnFalse_WhenCacheFails() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);
        var token = "test_token";
        var expiration = DateTime.UtcNow.AddHours(1);

        _mockCacheProvider.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<TimeSpan>()))
            .ThrowsAsync(new Exception("Cache failed"));

        var result = await jwtService.AddTokenToBlacklistAsync(token, expiration);

        Assert.False(result);
    }

    /// <summary>
    /// 测试检查令牌是否在黑名单中（令牌已被列入黑名单）
    /// <para>预期结果：返回 true，表示令牌在黑名单中</para>
    /// </summary>
    [Fact]
    public async Task IsTokenInBlacklistAsync_ShouldReturnTrue_WhenTokenIsBlacklisted() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);
        var token = "test_token";

        _mockCacheProvider.Setup(x => x.GetAsync<bool>(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await jwtService.IsTokenInBlacklistAsync(token);

        Assert.True(result);
    }

    /// <summary>
    /// 测试检查令牌是否在黑名单中（令牌未被列入黑名单）
    /// <para>预期结果：返回 false，表示令牌不在黑名单中</para>
    /// </summary>
    [Fact]
    public async Task IsTokenInBlacklistAsync_ShouldReturnFalse_WhenTokenIsNotBlacklisted() {
        var jwtService = new JwtService(_mockJwtOptions.Object, _mockCacheProvider.Object);
        var token = "test_token";

        _mockCacheProvider.Setup(x => x.GetAsync<string>(It.IsAny<string>()))
            .ReturnsAsync((string?)null);

        var result = await jwtService.IsTokenInBlacklistAsync(token);

        Assert.False(result);
    }
}
