# 项目结构概览

```
indie-game-framework/
│
├── 📄 README.md                     # 项目说明
├── 📄 PROJECT_STRUCTURE.md          # 本文件
│
├── 📁 core/                         # 核心系统（必须）
│   ├── 📁 src/
│   │   ├── game-manager.cs          # 游戏状态管理
│   │   ├── event-bus.cs             # 事件系统
│   │   ├── object-pool.cs           # 对象池
│   │   ├── save-system.cs           # 存档系统
│   │   └── scene-loader.cs          # 场景加载
│   ├── 📁 tests/
│   │   ├── run-tests.sh             # 测试脚本
│   │   └── Core.Tests.cs            # 单元测试
│   └── 📄 README.md
│
├── 📁 modules/                      # 功能模块（可选）
│   │
│   ├── 📁 sprite-processor/         # Sprite 自动处理
│   │   ├── 📁 src/
│   │   │   └── sprite-processor.py  # 处理脚本
│   │   ├── 📁 config/
│   │   │   └── default.yaml         # 默认配置
│   │   ├── 📁 tests/
│   │   │   └── run-tests.sh         # 测试脚本
│   │   └── 📄 README.md
│   │
│   ├── 📁 data-config/              # 数据驱动配置
│   │   ├── 📁 src/
│   │   │   └── config-manager.py    # 配置管理器
│   │   └── 📄 README.md
│   │
│   ├── 📁 cli/                      # 命令行工具
│   │   └── 📁 src/
│   │       └── game-cli.sh          # CLI 入口
│   │
│   └── 📁 debug-tools/              # 调试工具（待实现）
│       └── 📁 src/
│
├── 📁 templates/                    # 项目模板
│   └── 📁 project/
│       ├── README.md
│       └── .gitignore
│
├── 📁 tools/                        # 开发工具
│   └── 📁 scripts/
│       └── test-all.sh              # 运行所有测试
│
└── 📁 docs/                         # 文档
    ├── architecture.md              # 架构设计
    ├── sprite-processor.md          # Sprite 处理文档
    └── data-config.md               # 数据配置文档
```

---

## 核心模块 (core/)

### GameManager
游戏状态管理，单例模式。

```csharp
GameManager.Instance.StartGame();
GameManager.Instance.Pause();
GameManager.Instance.GameOver();
```

### EventBus
全局事件系统，解耦模块间通信。

```csharp
EventBus.Instance.Subscribe<GameOverEvent>(OnGameOver);
EventBus.Instance.Publish(new GameOverEvent { Score = 100 });
```

### ObjectPool
对象池，复用频繁创建的对象。

```csharp
ObjectPool.Instance.Register(() => new Bullet(), b => b.Reset(), 10, 100);
var bullet = ObjectPool.Instance.Get<Bullet>();
ObjectPool.Instance.Return(bullet);
```

### SaveSystem
存档系统，支持 JSON 序列化、备份、加密。

```csharp
SaveSystem.Instance.Save("save_1", gameData);
var data = SaveSystem.Instance.Load<GameData>("save_1");
```

### SceneLoader
场景加载器，管理场景切换。

```csharp
SceneLoader.Instance.RegisterScene("Level1", "/scenes/level1");
SceneLoader.Instance.LoadScene("Level1");
```

---

## 功能模块 (modules/)

### Sprite Processor
自动处理 AI 生成的图片。

**功能**：
- 去除白色背景
- 统一尺寸
- 切割 Sprite Sheet
- 生成 Unity 配置

**使用**：
```bash
./modules/sprite-processor/src/sprite-processor.py \
    --input ./auto-import \
    --output ./Assets/Sprites \
    --config ./modules/sprite-processor/config/default.yaml
```

### Data Config
数据驱动配置系统。

**功能**：
- YAML/JSON 配置
- 自动生成 C# 类
- 热更支持

**使用**：
```bash
./modules/data-config/src/config-manager.py \
    --input ./data/config \
    --output ./Assets/Scripts/Config \
    --lang cs
```

### CLI
命令行项目管理工具。

**命令**：
- `new` - 创建新项目
- `build` - 构建游戏
- `test` - 运行测试
- `hotfix` - 生成热更包
- `clean` - 清理构建

**使用**：
```bash
./modules/cli/src/game-cli.sh new MyGame
./modules/cli/src/game-cli.sh build --platform windows,mac
```

---

## 测试

### 运行所有测试
```bash
./tools/scripts/test-all.sh
```

### 运行单个模块测试
```bash
./core/tests/run-tests.sh
./modules/sprite-processor/tests/run-tests.sh
```

---

## 扩展框架

### 添加新模块

```bash
# 创建目录
mkdir -p modules/my-module/{src,tests,config}

# 实现功能
touch modules/my-module/src/my-module.py

# 编写测试
touch modules/my-module/tests/run-tests.sh

# 添加配置
touch modules/my-module/config/default.yaml

# 写文档
touch modules/my-module/README.md

# 注册测试
echo './modules/my-module/tests/run-tests.sh' >> tools/scripts/test-all.sh
```

---

## 项目状态

| 模块 | 状态 | 测试 |
|------|------|------|
| Core | ✅ 完成 | ✅ 通过 |
| Sprite Processor | ✅ 完成 | ✅ 通过 |
| Data Config | ✅ 完成 | ⏳ 待完善 |
| CLI | ✅ 基础功能 | ⏳ 待完善 |
| Debug Tools | ⏳ 计划中 | - |

---

## 下一步

1. **完善测试** - 增加集成测试和 E2E 测试
2. **Debug Tools** - 实现游戏内控制台、性能监控
3. **热更系统** - 完整的配置热更流程
4. **文档** - 补充使用示例和最佳实践
