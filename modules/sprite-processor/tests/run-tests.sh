#!/bin/bash

# Sprite Processor 模块测试脚本

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
MODULE_DIR="$(dirname "$SCRIPT_DIR")"

echo "========================================"
echo "  Running Sprite Processor Tests"
echo "========================================"
echo ""

GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m'

PASSED=0
FAILED=0

# 测试 1: 检查 Python 是否安装
echo -n "Testing: Python available... "
if command -v python3 &> /dev/null; then
    echo -e "${GREEN}PASSED${NC}"
    ((PASSED++))
else
    echo -e "${RED}FAILED${NC}"
    ((FAILED++))
    echo "  Python3 is required but not installed"
    exit 1
fi

# 测试 2: 检查依赖
echo -n "Testing: Python dependencies... "
if python3 -c "import PIL, yaml" 2>/dev/null; then
    echo -e "${GREEN}PASSED${NC}"
    ((PASSED++))
else
    echo -e "${RED}FAILED${NC}"
    ((FAILED++))
    echo "  Installing dependencies..."
    pip3 install pillow pyyaml --quiet
fi

# 测试 3: 检查脚本语法
echo -n "Testing: Script syntax... "
if python3 -m py_compile "$MODULE_DIR/src/sprite-processor.py" 2>/dev/null; then
    echo -e "${GREEN}PASSED${NC}"
    ((PASSED++))
else
    echo -e "${RED}FAILED${NC}"
    ((FAILED++))
fi

# 测试 4: 功能测试 - 创建测试图片并处理
echo -n "Testing: Basic image processing... "

# 创建测试目录
TEST_INPUT="$SCRIPT_DIR/test_input"
TEST_OUTPUT="$SCRIPT_DIR/test_output"
rm -rf "$TEST_INPUT" "$TEST_OUTPUT"
mkdir -p "$TEST_INPUT" "$TEST_OUTPUT"

# 创建测试图片（白色背景 + 黑色方块）
python3 << 'EOF'
from PIL import Image
import os

# 创建 100x100 图片，白色背景
img = Image.new('RGB', (100, 100), (255, 255, 255))

# 画一个黑色方块
for x in range(30, 70):
    for y in range(30, 70):
        img.putpixel((x, y), (0, 0, 0))

img.save('test_input/test_image.png')
EOF

# 运行处理器
if python3 "$MODULE_DIR/src/sprite-processor.py" \
    --input "$TEST_INPUT" \
    --output "$TEST_OUTPUT" \
    --config "$MODULE_DIR/config/default.yaml" > /dev/null 2>&1; then
    
    # 检查输出文件
    if [ -f "$TEST_OUTPUT/test_image_processed.png" ]; then
        echo -e "${GREEN}PASSED${NC}"
        ((PASSED++))
    else
        echo -e "${RED}FAILED${NC} (output file not created)"
        ((FAILED++))
    fi
else
    echo -e "${RED}FAILED${NC} (processor failed)"
    ((FAILED++))
fi

# 清理测试文件
rm -rf "$TEST_INPUT" "$TEST_OUTPUT"

# 测试 5: Sprite Sheet 切割测试
echo -n "Testing: Sprite sheet slicing... "

TEST_INPUT="$SCRIPT_DIR/test_input"
TEST_OUTPUT="$SCRIPT_DIR/test_output"
rm -rf "$TEST_INPUT" "$TEST_OUTPUT"
mkdir -p "$TEST_INPUT" "$TEST_OUTPUT"

# 创建 4x4 Sprite Sheet (256x256, 每个格子 64x64)
python3 << 'EOF'
from PIL import Image

# 创建 256x256 图片
img = Image.new('RGB', (256, 256), (255, 255, 255))

# 画 4x4 网格
for row in range(4):
    for col in range(4):
        # 每个格子画不同颜色
        color = (row * 64, col * 64, 128)
        for x in range(col * 64, (col + 1) * 64):
            for y in range(row * 64, (row + 1) * 64):
                img.putpixel((x, y), color)

img.save('test_input/sprite_sheet.png')
EOF

# 运行切割
if python3 "$MODULE_DIR/src/sprite-processor.py" \
    --input "$TEST_INPUT" \
    --output "$TEST_OUTPUT" \
    --slice \
    --grid-size 64 64 > /dev/null 2>&1; then
    
    # 检查是否生成了 16 个文件 (4x4)
    FILE_COUNT=$(ls -1 "$TEST_OUTPUT"/*.png 2>/dev/null | wc -l)
    if [ "$FILE_COUNT" -eq 16 ]; then
        echo -e "${GREEN}PASSED${NC}"
        ((PASSED++))
    else
        echo -e "${RED}FAILED${NC} (expected 16 files, got $FILE_COUNT)"
        ((FAILED++))
    fi
else
    echo -e "${RED}FAILED${NC} (slicing failed)"
    ((FAILED++))
fi

# 清理
rm -rf "$TEST_INPUT" "$TEST_OUTPUT"

# 总结
echo ""
echo "========================================"
echo "  Test Summary"
echo "========================================"
echo -e "  Passed: ${GREEN}$PASSED${NC}"
echo -e "  Failed: ${RED}$FAILED${NC}"
echo ""

if [ $FAILED -gt 0 ]; then
    exit 1
fi

echo -e "${GREEN}All tests passed!${NC}"
