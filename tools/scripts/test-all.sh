#!/bin/bash

# 运行所有框架测试

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
FRAMEWORK_DIR="$(dirname "$(dirname "$SCRIPT_DIR")")"

echo "========================================"
echo "  Indie Game Framework - All Tests"
echo "========================================"
echo ""

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

TOTAL_PASSED=0
TOTAL_FAILED=0

# 运行测试函数
run_test_suite() {
    local name=$1
    local test_script=$2
    
    echo ""
    echo -e "${YELLOW}Running: $name${NC}"
    echo "----------------------------------------"
    
    if [ -x "$test_script" ]; then
        if "$test_script"; then
            ((TOTAL_PASSED++))
        else
            ((TOTAL_FAILED++))
        fi
    else
        echo -e "${RED}Test script not found or not executable: $test_script${NC}"
        ((TOTAL_FAILED++))
    fi
}

# Core 模块测试
run_test_suite "Core Module" "$FRAMEWORK_DIR/core/tests/run-tests.sh"

# Sprite Processor 测试
run_test_suite "Sprite Processor" "$FRAMEWORK_DIR/modules/sprite-processor/tests/run-tests.sh"

# Data Config 测试
if [ -f "$FRAMEWORK_DIR/modules/data-config/tests/run-tests.sh" ]; then
    run_test_suite "Data Config" "$FRAMEWORK_DIR/modules/data-config/tests/run-tests.sh"
else
    echo ""
    echo -e "${YELLOW}Skipping: Data Config tests (not implemented)${NC}"
fi

# CLI 测试
if [ -f "$FRAMEWORK_DIR/modules/cli/tests/run-tests.sh" ]; then
    run_test_suite "CLI" "$FRAMEWORK_DIR/modules/cli/tests/run-tests.sh"
else
    echo ""
    echo -e "${YELLOW}Skipping: CLI tests (not implemented)${NC}"
fi

# 总结
echo ""
echo "========================================"
echo "  Final Summary"
echo "========================================"
echo -e "  Test Suites Passed: ${GREEN}$TOTAL_PASSED${NC}"
echo -e "  Test Suites Failed: ${RED}$TOTAL_FAILED${NC}"
echo ""

if [ $TOTAL_FAILED -gt 0 ]; then
    echo -e "${RED}Some tests failed!${NC}"
    exit 1
fi

echo -e "${GREEN}All tests passed! ✓${NC}"
