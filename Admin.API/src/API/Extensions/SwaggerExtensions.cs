/*
 * 文件名称: SwaggerExtensions.cs
 * 功能描述: Swagger 扩展方法，配置 Swagger 文档服务
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace API.Extensions;

/// <summary>
/// Swagger 扩展方法
/// <para>配置 Swagger 文档服务</para>
/// </summary>
public static class SwaggerExtensions {
    /// <summary>
    /// 添加 Swagger 服务到依赖注入容器
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services) {
        services.AddSwaggerGen(options => {
            options.SwaggerDoc("v1", new OpenApiInfo {
                Title = "Admin.NET API",
                Version = "v1.0.0",
                Description = "Admin.NET RBAC 权限管理系统 API 接口文档\n\n" +
                              "## 功能特性\n" +
                              "- 用户管理：用户的增删改查、状态管理\n" +
                              "- 角色管理：角色的增删改查、权限分配\n" +
                              "- 权限管理：菜单权限、API 权限、按钮权限\n" +
                              "- 认证授权：JWT 令牌认证、基于角色的访问控制",
                Contact = new OpenApiContact {
                    Name = "谢灿软件",
                    Email = "492384481@qq.com"
                },
                License = new OpenApiLicense {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                },
                TermsOfService = new Uri("https://example.com/terms")
            });

            options.SwaggerDoc("v2", new OpenApiInfo {
                Title = "Admin.NET API",
                Version = "v2.0.0",
                Description = "Admin.NET RBAC 权限管理系统 - API 接口文档 v2.0 (预留)\n\n" +
                              "## 功能特性\n" +
                              "- 用户管理：用户的增删改查、状态管理\n" +
                              "- 角色管理：角色的增删改查、权限分配\n" +
                              "- 权限管理：菜单权限、API 权限、按钮权限\n" +
                              "- 认证授权：JWT 令牌认证、基于角色的访问控制",
                Contact = new OpenApiContact {
                    Name = "谢灿软件",
                    Email = "492384481@qq.com"
                },
                License = new OpenApiLicense {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            AddXmlComments(options);

            options.TagActionsBy(apiDesc => {
                if (apiDesc.ActionDescriptor is ControllerActionDescriptor cad) {
                    var controllerName = cad.ControllerName.Replace("Controller", string.Empty, StringComparison.Ordinal);
                    var kebabCaseName = ToKebabCase(controllerName);
                    return [kebabCaseName];
                }
                return ["/"];
            });

            options.CustomOperationIds(apiDesc => {
                if (apiDesc.ActionDescriptor is ControllerActionDescriptor cad) {
                    var controllerName = cad.ControllerName.Replace("Controller", string.Empty, StringComparison.Ordinal);
                    var actionName = cad.ActionName;
                    var version = apiDesc.GroupName ?? "v1";
                    var kebabCaseController = ToKebabCase(controllerName);
                    var kebabCaseAction = ToKebabCase(actionName);
                    return $"{version}-{kebabCaseController}-{kebabCaseAction}";
                }
                return null;
            });

            options.IgnoreObsoleteActions();
            options.OrderActionsBy(apiDesc => $"{apiDesc.RelativePath}");
            options.DescribeAllParametersInCamelCase();
            AddJwtAuthorization(options);
            options.SupportNonNullableReferenceTypes();
            options.UseAllOfToExtendReferenceSchemas();

            options.DocInclusionPredicate((version, apiDesc) => {
                if (!apiDesc.TryGetMethodInfo(out var methodInfo)) {
                    return false;
                }

                var controllerVersion = apiDesc.GroupName ?? "v1";
                return version == controllerVersion;
            });

            options.OperationFilter<ReplaceApiVersionFilter>();
            options.DocumentFilter<KebabCaseDocumentFilter>();
        });

        return services;
    }

    /// <summary>
    /// 添加 JWT 授权支持
    /// </summary>
    /// <param name="options">Swagger 生成选项</param>
    private static void AddJwtAuthorization(SwaggerGenOptions options) {
        options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT 授权头，使用 Bearer 方案。格式：Bearer {token}"
        });

        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement {
            [new OpenApiSecuritySchemeReference("bearer", document)] = []
        });
    }

    /// <summary>
    /// 加载 XML 文档注释
    /// </summary>
    /// <param name="options">Swagger 生成选项</param>
    private static void AddXmlComments(SwaggerGenOptions options) {
        var baseDir = AppContext.BaseDirectory;
        var xmlFiles = Directory.GetFiles(baseDir, "*.xml", SearchOption.TopDirectoryOnly);
        foreach (var xml in xmlFiles) {
            try {
                options.IncludeXmlComments(xml, includeControllerXmlComments: true);
            }
#pragma warning disable CA1031 // XML 注释加载失败不应影响 Swagger 启动
            catch (Exception ex) {
                Trace.TraceWarning($"[SwaggerExtensions] 加载 XML 注释失败：{xml}. {ex.Message}");
            }
#pragma warning restore CA1031
        }
    }

    /// <summary>
    /// 将 PascalCase 或 camelCase 转换为 kebab-case
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>kebab-case 格式的字符串</returns>
    private static string ToKebabCase(string input) {
        if (string.IsNullOrEmpty(input)) {
            return input;
        }

        if (input.StartsWith("api", StringComparison.OrdinalIgnoreCase)) {
            input = input[3..];
        }

        var result = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++) {
            var c = input[i];
            if (char.IsUpper(c)) {
                if (i > 0) {
                    result.Append('-');
                }
                result.Append(char.ToLower(c));
            }
            else {
                result.Append(c);
            }
        }
        return result.ToString();
    }

    /// <summary>
    /// 使用 Swagger 中间件
    /// </summary>
    /// <param name="app">Web 应用构建器</param>
    /// <returns>Web 应用构建器</returns>
    public static WebApplication UseSwaggerMiddlewares(this WebApplication app) {
        app.UseSwagger();

        app.UseSwaggerUI(options => {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin.NET API v1");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "Admin.NET API v2 (预留");
            options.RoutePrefix = "swagger";
            options.DocumentTitle = "Admin.NET API 文档";
            options.DisplayOperationId();
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.DefaultModelsExpandDepth(-1);
            options.DefaultModelExpandDepth(2);
            options.DocExpansion(DocExpansion.List);
            options.EnableFilter();
            options.EnableValidator();
            options.ShowCommonExtensions();
            options.ShowExtensions();
            options.InjectJavascript("/swagger-initializer.js");
        });

        app.MapGet("/", () => Results.Redirect("/swagger"));

        return app;
    }
}

/// <summary>
/// 替换 API 版本占位符的过滤器
/// </summary>
public class ReplaceApiVersionFilter : IOperationFilter {
    /// <summary>
    /// 应用操作过滤器
    /// </summary>
    /// <param name="operation">OpenAPI 操作</param>
    /// <param name="context">操作过滤器上下文</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        var apiDescription = context.ApiDescription;
        var groupName = apiDescription.GroupName ?? "v1";

        if (operation.OperationId != null) {
            var versionNumber = groupName.Replace("v", string.Empty);
            operation.OperationId = operation.OperationId.Replace("{version:apiVersion}", versionNumber);
        }
    }
}

/// <summary>
/// 将路径转换为小写 + 连字符格式的过滤器
/// </summary>
public class KebabCaseDocumentFilter : IDocumentFilter {
    /// <summary>
    /// 应用文档过滤器
    /// </summary>
    /// <param name="swaggerDoc">Swagger 文档</param>
    /// <param name="context">文档过滤器上下文</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) {
        var pathsToRename = new Dictionary<string, string>();

        foreach (var path in swaggerDoc.Paths) {
            var oldKey = path.Key;
            var newPath = ConvertPathToKebabCase(oldKey);

            if (oldKey != newPath) {
                pathsToRename[oldKey] = newPath;
            }
        }

        foreach (var kvp in pathsToRename) {
            var oldPath = swaggerDoc.Paths[kvp.Key];
            swaggerDoc.Paths.Remove(kvp.Key);
            swaggerDoc.Paths[kvp.Value] = oldPath;
        }
    }

    /// <summary>
    /// 将路径转换为 kebab-case 格式
    /// </summary>
    /// <param name="path">原始路径</param>
    /// <returns>转换后的路径</returns>
    private static string ConvertPathToKebabCase(string path) {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var newSegments = new List<string>();

        foreach (var segment in segments) {
            if (segment.StartsWith('{') || segment.StartsWith("v")) {
                newSegments.Add(segment);
                continue;
            }

            newSegments.Add(ToKebabCase(segment));
        }

        return "/" + string.Join("/", newSegments);
    }

    /// <summary>
    /// 将 PascalCase 转换为 kebab-case
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>转换后的字符串</returns>
    private static string ToKebabCase(string input) {
        if (string.IsNullOrEmpty(input)) {
            return input;
        }

        var result = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++) {
            var c = input[i];
            if (char.IsUpper(c)) {
                if (i > 0) {
                    result.Append('-');
                }
                result.Append(char.ToLower(c));
            }
            else {
                result.Append(c);
            }
        }
        return result.ToString();
    }
}
