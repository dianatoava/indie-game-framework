#!/bin/bash

# SVG 编辑器启动脚本

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
EDITOR_DIR="$SCRIPT_DIR/svg-editor"

echo "========================================"
echo "  SVG Editor Launcher"
echo "========================================"
echo ""

# 检查 Java
if ! command -v java &> /dev/null; then
    echo "Error: Java not found. Please install Java 17+."
    exit 1
fi

JAVA_VERSION=$(java -version 2>&1 | head -n 1 | cut -d'"' -f2)
echo "Java version: $JAVA_VERSION"

# 检查编辑器是否已克隆
if [ ! -d "$EDITOR_DIR" ]; then
    echo "SVG Editor not found. Cloning..."
    git clone https://github.com/chengenzhao/svg-editor.git "$EDITOR_DIR"
    
    if [ $? -ne 0 ]; then
        echo "Error: Failed to clone SVG Editor"
        exit 1
    fi
fi

# 检查是否已编译
if [ ! -f "$EDITOR_DIR/target/svgeditor-1.0-SNAPSHOT.jar" ]; then
    echo "Building SVG Editor..."
    cd "$EDITOR_DIR"
    
    # 检查 Maven
    if ! command -v mvn &> /dev/null; then
        echo "Error: Maven not found. Please install Maven."
        exit 1
    fi
    
    mvn clean package -DskipTests
    
    if [ $? -ne 0 ]; then
        echo "Error: Build failed"
        exit 1
    fi
fi

# 启动编辑器
echo "Starting SVG Editor..."
cd "$EDITOR_DIR"

# 运行
java -jar target/svgeditor-1.0-SNAPSHOT.jar "$@"
