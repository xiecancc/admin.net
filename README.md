# Admin.NET

一个功能完整、架构清晰的后台管理系统，基于 .NET 8.0 和 Vue 3 开发。

## 项目特点

- **完整的权限管理系统**：支持菜单权限、按钮权限、API 权限的细粒度控制
- **现代化技术栈**：.NET 8.0 + Vue 3 + TypeScript + Element Plus
- **模块化设计**：清晰的分层架构，易于扩展和维护
- **前后端分离**：API 与前端完全分离，便于独立开发和部署
- **丰富的功能**：用户管理、角色管理、权限管理等核心功能

## 技术栈

### 后端
- .NET 8.0
- ASP.NET Core Web API
- Entity Framework Core
- JWT 认证
- Swagger 文档

### 前端
- Vue 3
- TypeScript
- Element Plus
- Pinia
- Vue Router
- Vite

## 快速开始

### 后端
1. 克隆仓库：`git clone https://github.com/xiecancc/admin.net.git`
2. 进入后端目录：`cd Admin.API`
3. 恢复依赖：`dotnet restore`
4. 运行迁移：`dotnet ef database update`
5. 启动服务：`dotnet run`

### 前端
1. 进入前端目录：`cd Admin.Vue`
2. 安装依赖：`npm install`
3. 启动开发服务器：`npm run dev`

## 项目结构

```
Admin.NET/
├── Admin.API/          # 后端 API
│   ├── src/            # 源代码
│   └── test/           # 测试代码
├── Admin.Vue/          # 前端项目
│   ├── src/            # 源代码
│   └── public/         # 静态资源
├── .gitignore          # Git 忽略文件
└── README.md           # 项目说明
```

## 许可证

本项目采用 MIT 许可证 - 详情请参阅 [LICENSE](LICENSE) 文件

## 贡献

欢迎提交 Issue 和 Pull Request！

## 联系方式

- 作者：谢灿软件
- 邮箱：492384481@qq.com
