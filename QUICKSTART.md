# 快速入门指南

## 5 分钟上手

### 1. 查看框架结构

```bash
cd /home/node/indie-game-framework
cat PROJECT_STRUCTURE.md
```

### 2. 运行测试（验证安装）

```bash
./tools/scripts/test-all.sh
```

### 3. 创建你的第一个游戏项目

```bash
# 使用 CLI 创建项目
./modules/cli/src/game-cli.sh new MyFirstGame

# 进入项目目录
cd MyFirstGame

# 查看项目结构
ls -la
```

### 4. 在 Unity 中打开

1. 打开 Unity Hub
2. 点击 "Add" → 选择 `MyFirstGame` 目录
3. 等待导入完成

---

## 核心功能演示

### Sprite 自动处理

```bash
# 1. 准备 AI 生成的图片
mkdir -p MyFirstGame/auto-import/characters

# 假设你有 AI 生成的图片，放入目录
# cp your-ai-image.png MyFirstGame/auto-import/characters/

# 2. 运行处理器
./framework/modules/sprite-processor/src/sprite-processor.py \
    --input ./auto-import/characters \
    --output ./Assets/_Game/Art/Characters \
    --config ./framework/modules/sprite-processor/config/default.yaml

# 3. Unity 会自动导入处理好的图片
```

### 数据配置

```bash
# 1. 创建配置文件
cat > MyFirstGame/data/config/enemies.yaml << 'EOF'
slime:
  id: 1001
  name: "史莱姆"
  hp: 50
  attack: 10
  speed: 2.5

goblin:
  id: 1002
  name: "哥布林"
  hp: 80
  attack: 15
  speed: 3.0
EOF

# 2. 生成 C# 类
./framework/modules/data-config/src/config-manager.py \
    --input ./MyFirstGame/data/config \
    --output ./MyFirstGame/Assets/_Game/Scripts/Config \
    --lang cs

# 3. 在代码中使用
# var slime = GameConfigManager.Instance.GetConfig<EnemyConfig>("slime");
```

### 使用核心系统

```csharp
// GameManager - 游戏状态
GameManager.Instance.StartGame();
GameManager.Instance.Pause();

// EventBus - 事件系统
EventBus.Instance.Subscribe<GameOverEvent>(OnGameOver);
EventBus.Instance.Publish(new GameOverEvent { Score = 100 });

// ObjectPool - 对象池
ObjectPool.Instance.Register(() => new Bullet(), b => b.Reset());
var bullet = ObjectPool.Instance.Get<Bullet>();

// SaveSystem - 存档
SaveSystem.Instance.Save("save_1", gameData);
var data = SaveSystem.Instance.Load<GameData>("save_1");
```

---

## 常用命令

```bash
# 创建新项目
./modules/cli/src/game-cli.sh new MyRPG --template 2d-platformer

# 构建游戏
./modules/cli/src/game-cli.sh build --platform windows,mac

# 运行测试
./modules/cli/src/game-cli.sh test --all

# 生成热更包
./modules/cli/src/game-cli.sh hotfix --version 1.0.1

# 清理构建
./modules/cli/src/game-cli.sh clean
```

---

## 下一步

### 学习资源

1. [架构设计](docs/architecture.md) - 了解框架设计理念
2. [项目结构](PROJECT_STRUCTURE.md) - 查看完整目录结构
3. [Sprite 处理](docs/sprite-processor.md) - 学习自动处理图片
4. [数据配置](docs/data-config.md) - 学习配置管理

### 扩展框架

```bash
# 添加你自己的模块
mkdir -p modules/my-module/{src,tests,config}
touch modules/my-module/src/my-module.py
touch modules/my-module/tests/run-tests.sh
```

### 贡献代码

1. Fork 项目
2. 创建功能分支
3. 编写测试
4. 提交 PR

---

## 获取帮助

- 查看 `README.md` 了解项目概览
- 查看 `docs/` 目录了解详细文档
- 运行 `./modules/cli/src/game-cli.sh help` 查看 CLI 帮助

---

**祝你开发顺利！** 🎮
