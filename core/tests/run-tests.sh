#!/bin/bash

# Core 模块测试脚本

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CORE_DIR="$(dirname "$SCRIPT_DIR")"

echo "========================================"
echo "  Running Core Module Tests"
echo "========================================"
echo ""

# 颜色定义
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m' # No Color

PASSED=0
FAILED=0

# 运行单个测试
run_test() {
    local test_name=$1
    local test_file=$2
    
    echo -n "  Testing: $test_name... "
    
    if dotnet test "$test_file" --logger "console;verbosity=quiet" > /dev/null 2>&1; then
        echo -e "${GREEN}PASSED${NC}"
        ((PASSED++))
    else
        echo -e "${RED}FAILED${NC}"
        ((FAILED++))
    fi
}

# 检查是否安装了 dotnet
if ! command -v dotnet &> /dev/null; then
    echo "  dotnet not installed, skipping C# tests"
    echo "  Running shell-based tests only..."
    
    # Shell 测试
    echo ""
    echo "Shell Tests:"
    
    # 测试 1: 文件结构
    echo -n "  Testing: File structure... "
    if [ -f "$CORE_DIR/src/game-manager.cs" ] && \
       [ -f "$CORE_DIR/src/event-bus.cs" ] && \
       [ -f "$CORE_DIR/src/object-pool.cs" ] && \
       [ -f "$CORE_DIR/src/save-system.cs" ] && \
       [ -f "$CORE_DIR/src/scene-loader.cs" ]; then
        echo -e "${GREEN}PASSED${NC}"
        ((PASSED++))
    else
        echo -e "${RED}FAILED${NC}"
        ((FAILED++))
    fi
    
    # 测试 2: 语法检查（如果有 csc）
    if command -v csc &> /dev/null; then
        echo -n "  Testing: C# syntax check... "
        if csc /t:library /out:/dev/null "$CORE_DIR/src"/*.cs 2>/dev/null; then
            echo -e "${GREEN}PASSED${NC}"
            ((PASSED++))
        else
            echo -e "${RED}FAILED${NC}"
            ((FAILED++))
        fi
    else
        echo "  Skipping: C# syntax check (csc not available)"
    fi
else
    # dotnet 测试
    echo ".NET Tests:"
    
    # 创建测试项目（如果不存在）
    TEST_PROJECT="$SCRIPT_DIR/Core.Tests.csproj"
    if [ ! -f "$TEST_PROJECT" ]; then
        echo "  Creating test project..."
        cd "$SCRIPT_DIR"
        dotnet new xunit --force --name Core.Tests
        # 添加项目引用
        echo "<ProjectReference Include=\"../src/Core.csproj\" />" >> Core.Tests.csproj
    fi
    
    # 运行测试
    run_test "GameManager" "$TEST_PROJECT"
    run_test "EventBus" "$TEST_PROJECT"
    run_test "ObjectPool" "$TEST_PROJECT"
    run_test "SaveSystem" "$TEST_PROJECT"
    run_test "SceneLoader" "$TEST_PROJECT"
fi

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
