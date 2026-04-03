# Indie Game Framework

**为独立游戏开发者设计的通用框架** - 模块化、可测试、自动化

---

## 🎯 设计理念

1. **模块化** - 每个功能独立，按需取用
2. **可测试** - 每个模块都有单元测试
3. **自动化** - 能自动的绝不手动
4. **约定优于配置** - 默认配置能用，高级功能可选

---

## 📦 项目结构

```
indie-game-framework/
├── core/                    # 核心系统（必须）
│   ├── src/                 # 核心源码
│   └── tests/               # 核心测试
│
├── modules/                 # 功能模块（可选）
│   ├── sprite-processor/    # Sprite 自动处理
│   ├── data-config/         # 数据驱动配置
│   ├── cli/                 # 命令行工具
│   └── debug-tools/         # 调试工具
│
├── templates/               # 项目模板
│   ├── project/             # 新项目模板
│   ├── sprites/             # Sprite 模板
│   └── configs/             # 配置模板
│
├── tools/                   # 开发工具
│   ├── scripts/             # 构建脚本
│   └── ci/                  # CI/CD 配置
│
└── docs/                    # 文档
```

---

## 🚀 快速开始

### 安装

```bash
# 克隆框架
git clone https://github.com/yourname/indie-game-framework.git

# 运行测试（验证安装）
./tools/scripts/test-all.sh
```

### 创建新项目

```bash
# 使用 CLI 创建新项目
./modules/cli/src/game-cli.sh new MyGame

# 或手动复制模板
cp -r templates/project/* ../MyGame/
```

---

## 📋 模块说明

### Core（核心系统）

| 模块 | 说明 | 必选 |
|------|------|------|
| GameManager | 游戏状态管理 | ✅ |
| EventBus | 事件系统 | ✅ |
| ObjectPool | 对象池 | ✅ |
| SaveSystem | 存档系统 | ✅ |
| SceneLoader | 场景加载 | ✅ |

### Modules（功能模块）

| 模块 | 说明 | 必选 |
|------|------|------|
| sprite-processor | AI 图片自动处理 | ❌ |
| data-config | 数据驱动配置 | ❌ |
| svg-editor | SVG 矢量图编辑器 | ❌ |
| cli | 命令行工具 | ❌ |
| debug-tools | 调试工具 | ❌ |

---

## 🧪 测试

```bash
# 运行所有测试
./tools/scripts/test-all.sh

# 运行单个模块测试
./modules/sprite-processor/tests/run-tests.sh

# 运行核心测试
./core/tests/run-tests.sh
```

---

## 🛠️ 开发指南

### 添加新模块

```bash
# 1. 创建模块目录
mkdir -p modules/my-module/{src,tests,config}

# 2. 创建模块入口
touch modules/my-module/src/my-module.sh

# 3. 创建测试
touch modules/my-module/tests/test-my-module.sh

# 4. 添加到测试脚本
echo "./modules/my-module/tests/run-tests.sh" >> tools/scripts/test-all.sh
```

### 模块接口规范

每个模块必须提供：
- `src/` - 源码
- `tests/run-tests.sh` - 测试脚本
- `config/default.yaml` - 默认配置
- `README.md` - 使用说明

---

## 📖 文档

- [架构设计](docs/architecture.md)
- [Sprite 处理](docs/sprite-processor.md)
- [数据配置](docs/data-config.md)
- [CLI 工具](docs/cli.md)

---

## 🤝 贡献

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

---

## 📄 许可证

MIT License

---

## 🎮 示例项目

查看 [examples/](examples/) 目录中的完整示例。
