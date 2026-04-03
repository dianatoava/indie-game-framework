# {{PROJECT_NAME}}

使用 **Indie Game Framework** 创建的游戏项目。

---

## 🚀 快速开始

### 1. 打开项目

在 Unity Hub 中添加此项目并打开。

### 2. 初始化框架

运行场景：`Assets/_Framework/Scenes/Initializer.unity`

### 3. 开始开发

- 游戏代码放在 `Assets/_Game/Scripts/`
- 配置放在 `data/config/`
- AI 图片放在 `auto-import/`

---

## 📁 目录结构

```
.
├── Assets/
│   ├── _Framework/         # 框架代码（symlink）
│   └── _Game/              # 你的游戏代码
│       ├── Scripts/
│       ├── Prefabs/
│       ├── Art/
│       └── Audio/
│
├── data/
│   └── config/             # 游戏配置 (YAML)
│       ├── enemies.yaml
│       └── items.yaml
│
├── auto-import/            # AI 图片自动导入
│   └── characters/
│
├── framework/              # 框架 symlink
│
└── README.md
```

---

## 🛠️ 使用框架

### Sprite 自动处理

```bash
# 1. 把 AI 生成的图片放入 auto-import/
# 2. 运行处理器
./framework/modules/sprite-processor/src/sprite-processor.py \
    --input ./auto-import/characters \
    --output ./Assets/_Game/Art/Characters \
    --config ./framework/modules/sprite-processor/config/default.yaml
```

### 配置管理

```bash
# 1. 在 data/config/ 添加 YAML 配置
# 2. 生成 C# 类
./framework/modules/data-config/src/config-manager.py \
    --input ./data/config \
    --output ./Assets/_Game/Scripts/Config \
    --lang cs
```

### CLI 工具

```bash
# 构建游戏
./framework/modules/cli/src/game-cli.sh build --platform windows

# 生成热更包
./framework/modules/cli/src/game-cli.sh hotfix --version 1.0.1

# 运行测试
./framework/modules/cli/src/game-cli.sh test --all
```

---

## 📖 文档

- [框架架构](framework/docs/architecture.md)
- [Sprite 处理](framework/docs/sprite-processor.md)
- [数据配置](framework/docs/data-config.md)

---

## 🎮 开发指南

### 添加新敌人

1. 编辑 `data/config/enemies.yaml`
2. 运行 `config-manager.py` 生成代码
3. 在游戏中使用：
   ```csharp
   var slime = GameConfigManager.Instance.GetConfig<EnemyConfig>("slime");
   ```

### 添加新角色

1. AI 生成图片 → `auto-import/characters/`
2. 运行 `sprite-processor.py`
3. 在 Unity 中创建 Prefab

---

## 🤝 贡献

查看框架文档了解如何扩展模块。

---

## 📄 许可证

MIT License
