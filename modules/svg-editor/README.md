# SVG 编辑器模块

**可视化 SVG 矢量图编辑器** - 基于 [chengenzhao/svg-editor](https://github.com/chengenzhao/svg-editor)

---

## 功能

- **可视化编辑** - 拖拽节点编辑 SVG 路径
- **多种路径类型** - 支持 Line、Curve、Quadratic、Smooth 等
- **实时预览** - 即时查看编辑效果
- **导出格式** - 导出为 SVG、PNG、或 Unity 可用格式

---

## 安装

### 前提条件

- Java 17+
- Maven

### 安装步骤

```bash
# 1. 克隆 SVG 编辑器
cd modules/svg-editor/tools
git clone https://github.com/chengenzhao/svg-editor.git

# 2. 编译
cd svg-editor
mvn clean package

# 3. 运行
mvn javafx:run
```

---

## 使用方式

### 方式 1：独立运行（推荐）

```bash
# 启动编辑器
./modules/svg-editor/tools/launch-editor.sh

# 编辑完成后，导出 SVG 到项目
# 然后使用 Sprite Processor 处理
```

### 方式 2：框架集成

```bash
# 在框架中启动
./modules/cli/src/game-cli.sh svg-editor

# 编辑并导出到 auto-import/
# 自动触发 Sprite Processor 处理
```

---

## 工作流

```
1. 启动 SVG Editor
   ↓
2. 绘制/编辑矢量图形
   ↓
3. 导出为 SVG/PNG
   ↓
4. 放入 auto-import/
   ↓
5. Sprite Processor 自动处理
   ↓
6. Unity 中使用
```

---

## 配置

```yaml
# config/default.yaml
svg-editor:
  # 导出设置
  export:
    format: svg          # svg | png | both
    size: [512, 512]     # 导出尺寸
    background: transparent
  
  # 自动处理设置
  auto-process: true     # 导出后自动调用 Sprite Processor
  output-dir: ../../auto-import/svg/
```

---

## 与 Sprite Processor 集成

编辑好的 SVG 可以：

1. **直接导出为 PNG** - 然后走 Sprite Processor 流程
2. **保持为 SVG** - Unity 使用 SVG 插件直接导入
3. **导出为 Sprite Sheet** - 适合动画

---

## 示例

### 创建游戏 Logo

```bash
# 1. 启动编辑器
./tools/launch-editor.sh

# 2. 绘制 Logo 路径
# 3. 导出为 logo.svg

# 4. 转换为 PNG（自动）
./tools/svg-to-png.sh logo.svg --size 256x256

# 5. 放入 auto-import
mv logo.png ../../auto-import/ui/

# 6. Sprite Processor 自动处理
```

---

## 依赖

- Java 17+
- JavaFX
- Maven

---

## 许可证

与原项目相同
