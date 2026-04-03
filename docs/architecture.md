# 架构设计文档

## 设计原则

### 1. 模块化 (Modularity)

每个功能独立成模块，按需取用：

```
┌─────────────────────────────────────────┐
│              游戏项目                     │
├─────────────────────────────────────────┤
│  Core (必须)                             │
│  ├── GameManager                         │
│  ├── EventBus                            │
│  ├── ObjectPool                          │
│  ├── SaveSystem                          │
│  └── SceneLoader                         │
├─────────────────────────────────────────┤
│  Modules (可选)                          │
│  ├── sprite-processor  ← 需要时用        │
│  ├── data-config       ← 需要时用        │
│  ├── cli              ← 需要时用         │
│  └── debug-tools      ← 需要时用         │
└─────────────────────────────────────────┘
```

### 2. 可测试性 (Testability)

每个模块必须：
- 有独立的测试目录
- 提供测试脚本 (`tests/run-tests.sh`)
- 不依赖全局状态（除了单例）
- 支持 mock/stub

### 3. 约定优于配置 (Convention over Configuration)

默认配置能用，高级功能可选：

```yaml
# 简单场景 - 默认配置就够用
default:
  target_size: [64, 64]
  remove_bg: true

# 复杂场景 - 可以自定义
character:
  target_size: [128, 128]
  animation:
    idle_frames: 4
    run_frames: 8
```

### 4. 自动化优先 (Automation First)

能自动的绝不手动：

| 任务 | 自动化方案 |
|------|-----------|
| AI 图片处理 | `sprite-processor` |
| 配置生成 | `config-manager` |
| 项目创建 | `game-cli new` |
| 构建 | `game-cli build` |
| 测试 | `game-cli test` |

---

## 目录结构

```
indie-game-framework/
├── core/                      # 核心系统（必须）
│   ├── src/                   # 源码
│   │   ├── game-manager.cs
│   │   ├── event-bus.cs
│   │   ├── object-pool.cs
│   │   ├── save-system.cs
│   │   └── scene-loader.cs
│   ├── tests/                 # 测试
│   │   ├── run-tests.sh
│   │   └── Core.Tests.cs
│   └── README.md
│
├── modules/                   # 功能模块（可选）
│   ├── sprite-processor/      # Sprite 处理
│   │   ├── src/
│   │   │   └── sprite-processor.py
│   │   ├── config/
│   │   │   └── default.yaml
│   │   ├── tests/
│   │   │   └── run-tests.sh
│   │   └── README.md
│   │
│   ├── data-config/           # 数据配置
│   │   ├── src/
│   │   │   └── config-manager.py
│   │   └── README.md
│   │
│   ├── cli/                   # 命令行工具
│   │   └── src/
│   │       └── game-cli.sh
│   │
│   └── debug-tools/           # 调试工具
│       └── src/
│
├── templates/                 # 项目模板
│   └── project/
│
├── tools/                     # 开发工具
│   └── scripts/
│       └── test-all.sh
│
└── docs/                      # 文档
    ├── architecture.md
    ├── sprite-processor.md
    └── data-config.md
```

---

## 模块接口规范

每个模块必须提供：

### 1. 源码目录 (`src/`)

- 主要功能实现
- 清晰的入口函数/类

### 2. 测试目录 (`tests/`)

- `run-tests.sh` - 测试脚本
- 测试数据
- Mock/Stub

### 3. 配置目录 (`config/`)

- `default.yaml` - 默认配置
- 可选的预设配置

### 4. 文档 (`README.md`)

- 功能说明
- 使用示例
- 配置选项

---

## 数据流

### Sprite 处理流程

```
AI 生成图片
     ↓
auto-import/              (用户放置)
     ↓
sprite-processor.py       (自动处理)
  - 去背景
  - 统一尺寸
  - 切割 Sprite Sheet
     ↓
Assets/Sprites/processed/ (处理结果)
     ↓
Unity Import              (自动导入)
  - 设置 filterMode
  - 生成 Sprite Atlas
     ↓
SpriteRegistry.Get()      (代码使用)
```

### 配置管理流程

```
data/config/
  enemies.yaml
  items.yaml
     ↓
config-manager.py         (生成代码)
  - 验证格式
  - 生成 C# 类
     ↓
Generated/GameConfig.cs   (类型安全访问)
     ↓
GameConfigManager.Instance
  .GetConfig<EnemyConfig>("slime")
```

---

## 扩展点

### 添加新模块

```bash
# 1. 创建目录
mkdir -p modules/my-module/{src,tests,config}

# 2. 实现功能
touch modules/my-module/src/my-module.py

# 3. 编写测试
touch modules/my-module/tests/run-tests.sh

# 4. 添加配置
touch modules/my-module/config/default.yaml

# 5. 写文档
touch modules/my-module/README.md

# 6. 注册到测试脚本
echo './modules/my-module/tests/run-tests.sh' >> tools/scripts/test-all.sh
```

### 扩展现有模块

通过配置文件扩展，不修改源码：

```yaml
# config/custom.yaml
# 自定义配置，覆盖默认值

custom_profile:
  target_size: [256, 256]
  remove_bg: false
  filter_mode: linear
```

---

## 版本管理

### 语义化版本

```
MAJOR.MINOR.PATCH

MAJOR - 不兼容的 API 变更
MINOR - 向后兼容的功能新增
PATCH - 向后兼容的问题修复
```

### 模块版本独立

每个模块可以有自己的版本：

```yaml
# module.yaml
name: sprite-processor
version: 1.2.0
dependencies:
  - pillow>=9.0.0
  - pyyaml>=6.0
```

---

## 测试策略

### 测试金字塔

```
        /\
       /  \      E2E 测试 (少量)
      /----\
     /      \    集成测试 (适量)
    /--------\
   /          \   单元测试 (大量)
  /------------\
```

### 单元测试

- 测试单个函数/类
- 快速执行 (<100ms)
- 无外部依赖

### 集成测试

- 测试模块间协作
- 中等速度 (<1s)
- 可能需要 mock

### E2E 测试

- 测试完整流程
- 较慢 (>1s)
- 真实环境

---

## 性能考虑

### 启动时间

- Core 模块 < 100ms
- 单个 Module < 50ms
- CLI 命令 < 500ms

### 内存占用

- Core 模块 < 10MB
- Sprite 处理按需加载
- 配置缓存 < 5MB

### 优化技巧

1. **懒加载** - 不用的模块不加载
2. **对象池** - 复用频繁创建的对象
3. **异步处理** - 不阻塞主线程
4. **缓存** - 配置/资源缓存

---

## 安全考虑

### 配置文件

- 验证输入格式
- 限制文件大小
- 防止路径遍历

### 存档系统

- 简单加密（防查看）
- 备份机制
- 校验和验证

### CLI 工具

- 命令白名单
- 路径验证
- 权限检查

---

## 未来规划

### Phase 1 (已完成)

- [x] Core 模块
- [x] Sprite Processor
- [x] Data Config
- [x] CLI 基础

### Phase 2 (进行中)

- [ ] Debug Tools
- [ ] 热更系统
- [ ] 自动构建

### Phase 3 (计划)

- [ ] 多设备同步
- [ ] 崩溃日志
- [ ] 性能分析工具
