<div align="center">

<h1 align="center">Workman</h1>

![.NET](https://img.shields.io/badge/.NET-8-blue) ![WPF](https://img.shields.io/badge/UI-WPF-green) ![License](https://img.shields.io/badge/license-MIT-orange) ![Stars](https://img.shields.io/github/stars/winemonk/workman)

<p align="center">
A concise and efficient desktop work log recording tool
<br />
Built with WPF + .NET 8
<br /><br />
<a href="https://github.com/winemonk/workman/issues">Report Issues</a>
·
<a href="https://github.com/winemonk/workman/issues">Feature Suggestions</a>

</p>

</div>


---

# 📖 Project Introduction

**Workman** is a desktop work log recording tool designed to help developers and office workers record daily work content and enable quick recording and viewing through desktop widgets.

Main Goals:

- Provide lightweight work log management
- Support project-based log recording
- Enable quick recording through desktop widgets
- Deliver a clear and intuitive interface

---

# ✨ Features

### 📝 Log Management
- Create daily work logs
- Edit / Delete logs
- Support project association

### 📁 Project Management
- Create multiple projects
- Organize logs by project

### 🖥 Desktop Widgets
- Always-on-top desktop display
- Quick view of today's work
- Quick add new logs

### ⚙ System Settings
- Customize log behavior
- Configure application parameters

---

# 🖼 Screenshots

## Main Interface

![main](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041132253.png)

![main2](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041132692.png)

---

## Desktop Widgets

![desktop1](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041130635.png)

![desktop2](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041131388.png)

![desktop3](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041130915.png)

---

## Project Management

![project1](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041133264.png)

![project2](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041133342.png)

---

## Create New Log

![log](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041134424.png)

---

## Settings

![setting](https://raw.githubusercontent.com/Winemonk/images/master/blog/post/202603041134696.png)

---

# 🧰 Technology Stack

| Technology | Description       |
| ---------- | ----------------- |
| WPF        | Windows Desktop UI   |
| .NET 8     | Application Runtime |
| Prism      | MVVM + Modular Framework |
| DryIoc     | Dependency Injection Container |
| SQLite     | Local Database     |

---

# 📁 Project Structure

workman
 ├─ Workman.Apps WPF UI & Core Business Logic Implementation
 ├─ Workman.Core Entities
 ├─ Workman.Infrastructure Data Access

---

# 🚀 Quick Start

## Runtime Requirements

- Windows 10 / Windows 11
- .NET 8 Runtime
- Visual Studio 2022

---

## Clone Project

```bash
git clone https://github.com/yourname/workman.git
```

---

## Run Project

```bash
cd workman
```

Open the solution with Visual Studio 2022 and run.

# 🧩 Architecture Overview

The project adopts MVVM + Prism modular architecture

## Core Structure:

View → ViewModel → Service → Repository → SQLite

## Features:

Modular Design

Loose Coupling

Easy to Extend

Easy to Test

# 📌 Roadmap

Future Plans:

 Log Statistics and Analysis

 Data Export

 Markdown Support

 Multi-theme Support

 Cloud Sync

# 🤝 Contribution

Welcome to submit Issues or Pull Requests.

Contribution Process:

1. Fork the project
2. Create a branch
    ```bash
    git checkout -b feature/your-feature
    ```
3. Commit code
    ```bash
    git commit -m "add new feature"
    ```
4. Push branch
    ```bash
    git push origin feature/your-feature
    ```
5. Create a Pull Request

# 📄 License

MIT License

# ⭐ Support the Project

If this project has been helpful to you, please give it a Star ⭐
