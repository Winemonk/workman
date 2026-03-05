![.NET](https://img.shields.io/badge/.NET-8-blue) ![WPF](https://img.shields.io/badge/UI-WPF-green) ![License](https://img.shields.io/badge/license-MIT-orange) ![Stars](https://img.shields.io/github/stars/winemonk/workman)

<div align="center">

<h1 align="center">Workman</h1>

<p align="center">
一个简洁高效的桌面工作日志记录工具
<br />
基于 WPF + .NET 8 构建
<br /><br />
<a href="https://github.com/winemonk/workman/issues">报告问题</a>
·
<a href="https://github.com/winemonk/workman/issues">功能建议</a>

</p>

</div>


---

# 📖 项目介绍

**Workman** 是一个桌面工作日志记录工具，用于帮助开发者或办公人员记录每日工作内容，并通过桌面组件实现快速记录与查看。

主要目标：

- 提供轻量级工作日志管理
- 支持项目维度记录日志
- 支持桌面组件快速记录
- 提供清晰直观的界面

---

# ✨ 功能特性

### 📝 日志管理
- 创建每日工作日志
- 编辑 / 删除日志
- 支持项目关联

### 📁 项目管理
- 创建多个项目
- 日志按项目分类

### 🖥 桌面组件
- 桌面常驻显示
- 快速查看今日工作
- 快速新增日志

### ⚙ 系统设置
- 自定义日志行为
- 应用参数配置

---

# 🖼 应用截图

## 主界面

![main](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041132253.png)

![main2](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041132692.png)

---

## 桌面组件

![desktop1](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041130635.png)

![desktop2](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041131388.png)

![desktop3](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041130915.png)

---

## 项目管理

![project1](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041133264.png)

![project2](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041133342.png)

---

## 新建日志

![log](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041134424.png)

---

## 设置

![setting](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041134696.png)

---

# 🧰 技术栈

| 技术   | 说明              |
| ------ | ----------------- |
| WPF    | Windows 桌面 UI   |
| .NET 8 | 应用运行时        |
| Prism  | MVVM + 模块化框架 |
| DryIoc | 依赖注入容器      |
| SQLite | 本地数据库        |

---

# 📁 项目结构

workman
 ├─ Workman.Apps WPF界面&核心业务逻辑实现
 ├─ Workman.Core 实体
 ├─ Workman.Infrastructure 数据访问

---

# 🚀 快速开始

## 运行环境

- Windows 10 / Windows 11
- .NET 8 Runtime
- Visual Studio 2022

---

## 克隆项目

```bash
git clone https://github.com/yourname/workman.git
```

---

## 运行项目

```bash
cd workman
```

使用 Visual Studio 2022 打开解决方案并运行。

# 🧩 架构说明

项目采用 MVVM + Prism 模块化架构

## 核心结构：

View
 ↓
ViewModel
 ↓
Service
 ↓
Repository
 ↓
SQLite

## 特点：

模块化设计

松耦合

易扩展

易测试

# 📌 Roadmap

未来计划：

 日志统计分析

 数据导出

 Markdown 支持

 多主题支持

 云同步

# 🤝 贡献

欢迎提交 Issue 或 Pull Request。

贡献流程：

1 Fork 项目
2 创建分支
    ```bash
    git checkout -b feature/your-feature
    ```
3 提交代码
    ```bash
    git commit -m "add new feature"
    ```
4 推送分支
    ```bash
    git push origin feature/your-feature
    ```
5 创建 Pull Request

# 📄 License

MIT License

# ⭐ 支持项目

如果这个项目对你有帮助，欢迎点个 Star ⭐
