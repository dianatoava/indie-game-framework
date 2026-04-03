#!/bin/bash

# 测试准备脚本
# 创建测试环境并输出提示

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
FRAMEWORK_DIR="$(dirname "$SCRIPT_DIR")"

echo "========================================"
echo "  美术素材测试准备"
echo "========================================"
echo ""

# 颜色定义
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

TEST_DIR="$SCRIPT_DIR/test-art-assets"
INPUT_DIR="$TEST_DIR/input"

# 创建目录结构
echo -e "${BLUE}创建测试目录...${NC}"
mkdir -p "$INPUT_DIR"/{characters,ui,tileset,vector,items}
mkdir -p "$TEST_DIR/output"
mkdir -p "$TEST_DIR/report"

echo -e "${GREEN}✓ 目录创建完成${NC}"
echo ""

# 显示目录结构
echo "目录结构："
tree "$TEST_DIR" 2>/dev/null || find "$TEST_DIR" -type d | head -20
echo ""

# 显示提示词
echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}  请使用以下中文提示词生成素材${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

cat << 'EOF'
【角色提示词】
2D像素艺术游戏角色精灵图，可爱的Q版战士，头大身小的比例，
穿着精致的银色盔甲，镶嵌蓝色宝石装饰，披着飘逸的红色披风，
手持发光魔法剑，带有粒子特效，多个动画帧展示：待机姿势、
奔跑循环、攻击挥砍、跳跃动作，16位复古游戏风格，使用抖动技术
的有限调色板，纯透明背景便于游戏集成，从左上方向的一致光照
创造清晰的阴影，干净的像素边缘无抗锯齿，画面清晰锐利，
参考《星露谷物语》和《蔚蓝》的艺术风格，可直接导入Unity的游戏资源，
按4x4精灵图网格布局排列，每帧精确64x64像素，高对比度剪影
确保清晰可读，奇幻RPG主题带魔法元素，暖色调配合橙色和金色
高光，与冷色调银色盔甲形成对比

【环境提示词】
2D像素艺术游戏环境瓦片集，中世纪村庄场景，鹅卵石小径
和茅草屋顶小屋，多种地形类型包括草地、泥土、石板路面
和木板地面，无缝平铺纹理重复时无可见边缘，16位复古像素艺术
美学与角色精灵匹配，有限的32色调色板，大地色调和秋天色彩，
白天光照带柔和的环境光遮蔽阴影，俯视45度视角适合RPG视图，
包含地面瓦片、建筑墙壁、屋顶部分、围栏，以及装饰元素如
花朵、岩石和招牌，每块瓦片精确32x32像素，物体瓦片透明背景，
参考《星露谷物语》和《牧场物语》环境艺术，温馨的村庄氛围
带温暖的橙色和棕色配色方案，所有瓦片一致的像素密度和风格，
可直接导入Unity瓦片地图系统的游戏资源

【UI提示词】
2D像素艺术游戏UI元素套装，RPG风格界面组件，包括带斜面边框的
矩形按钮，带金属框架的圆形技能图标，带渐变填充的生命值和
法力值条，带微妙深度的背包槽位背景，带装饰角饰的窗口面板，
16位像素艺术风格与游戏美学匹配，有限调色板，金色装饰和
深色木色调，叠加元素的透明背景，一致的1像素轮廓厚度，
参考《最终幻想》和《时空之轮》UI设计，游戏资源精灵图格式，
各种尺寸从32x32图标到256x64面板，高对比度确保可读性，
奇幻RPG主题带中世纪工艺细节，所有UI元素统一的视觉语言
EOF

echo ""
echo -e "${YELLOW}========================================${NC}"
echo -e "${YELLOW}  完整提示词文档${NC}"
echo -e "${YELLOW}========================================${NC}"
echo ""
echo "查看完整提示词："
echo "  cat $FRAMEWORK_DIR/docs/ai-prompts-zh.md"
echo ""

# 显示操作步骤
echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}  操作步骤${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""
echo "1. 使用AI工具（Midjourney/Stable Diffusion/文心一格等）"
echo "   用上面的提示词生成素材"
echo ""
echo "2. 下载生成的JPG/PNG图片到对应目录："
echo "   - 角色 → $INPUT_DIR/characters/"
echo "   - UI   → $INPUT_DIR/ui/"
echo "   - 瓦片 → $INPUT_DIR/tileset/"
echo "   - 道具 → $INPUT_DIR/items/"
echo ""
echo "3. 运行测试："
echo "   $SCRIPT_DIR/art-assets-integration-test.sh"
echo ""
echo "4. 查看测试报告："
echo "   $TEST_DIR/report/test-report.md"
echo ""
echo -e "${GREEN}准备完成！${NC}"
