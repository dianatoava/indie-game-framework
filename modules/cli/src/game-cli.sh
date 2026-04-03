#!/bin/bash

# Game CLI - 游戏项目管理命令行工具

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
FRAMEWORK_DIR="$(dirname "$(dirname "$SCRIPT_DIR")")"

# 颜色定义
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# 打印横幅
print_banner() {
    echo -e "${BLUE}"
    echo "  ____ ___        .__                 __      __  .__   "
    echo " |    |   \______ |  |   ____   _____/  |    /  |_|__| "
    echo " |    |   /\____ \|  |  /  _ \_/ ___\   __\  /   __\  | "
    echo " |    |  / |  |_> >  |_(  <_> )  \___|  |    |  | |  | "
    echo " |______/  |   __/|____/\____/ \___  >__|    |__| |__| "
    echo "           |__|                    \/                  "
    echo -e "${NC}"
    echo ""
}

# 打印帮助
print_help() {
    print_banner
    echo "用法：game-cli <命令> [选项]"
    echo ""
    echo "命令:"
    echo "  new <project-name>   创建新项目"
    echo "  build                构建游戏"
    echo "  test                 运行测试"
    echo "  hotfix               生成热更包"
    echo "  sync                 同步多设备"
    echo "  clean                清理构建文件"
    echo "  help                 显示帮助"
    echo ""
    echo "示例:"
    echo "  game-cli new MyRPG --template 2d-platformer"
    echo "  game-cli build --platform windows,mac"
    echo "  game-cli test --all"
    echo ""
}

# 创建新项目
cmd_new() {
    local project_name=$1
    shift
    
    if [ -z "$project_name" ]; then
        echo -e "${RED}错误：请指定项目名称${NC}"
        exit 1
    fi
    
    # 解析选项
    local template="default"
    while [[ $# -gt 0 ]]; do
        case $1 in
            --template|-t)
                template="$2"
                shift 2
                ;;
            *)
                shift
                ;;
        esac
    done
    
    echo -e "${BLUE}Creating project: ${GREEN}$project_name${NC}"
    echo -e "${BLUE}Template: ${GREEN}$template${NC}"
    echo ""
    
    # 检查项目是否已存在
    if [ -d "$project_name" ]; then
        echo -e "${RED}错误：项目已存在${NC}"
        exit 1
    fi
    
    # 创建项目目录
    mkdir -p "$project_name"
    cd "$project_name"
    
    # 复制模板
    TEMPLATE_DIR="$FRAMEWORK_DIR/templates/project"
    if [ -d "$TEMPLATE_DIR" ]; then
        cp -r "$TEMPLATE_DIR"/* .
        cp -r "$TEMPLATE_DIR"/.[!.]* . 2>/dev/null || true
    else
        # 创建基础结构
        mkdir -p Assets/_Game/Scripts
        mkdir -p Assets/_Game/Prefabs
        mkdir -p Assets/_Game/Art
        mkdir -p Assets/_Game/Audio
        mkdir -p data/config
        mkdir -p auto-import
        mkdir -p docs
        
        # 创建 README
        cat > README.md << 'EOF'
# Game Project

使用 Indie Game Framework 创建的游戏项目。

## 目录结构

```
.
├── Assets/
│   └── _Game/          # 游戏代码和资源
├── data/
│   └── config/         # 游戏配置
├── auto-import/        # AI 图片自动导入
└── docs/               # 文档
```

## 快速开始

1. 在 Unity 中打开此项目
2. 运行 Framework 初始化场景
3. 开始开发！

## 使用框架

- Sprite 处理：将 AI 图片放入 `auto-import/`，运行 `sprite-processor`
- 配置管理：在 `data/config/` 添加 YAML 配置
- 构建：使用 `game-cli build`
EOF
    fi
    
    # 创建 .gitignore
    cat > .gitignore << 'EOF'
# Unity
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Uu]ser[Ss]ettings/
*.pidb.meta
*.pdb.meta
*.mdb.meta
sysinfo.txt
*.apk
*.aab
*.unitypackage
*.app

# OS
.DS_Store
Thumbs.db

# IDE
.vs/
.idea/
*.swp
*.swo
*~

# Framework
auto-import/*
!auto-import/.gitkeep
EOF
    
    # 创建框架 symlink
    ln -sf "$FRAMEWORK_DIR" framework 2>/dev/null || true
    
    echo -e "${GREEN}✓ 项目创建成功！${NC}"
    echo ""
    echo "下一步:"
    echo "  1. cd $project_name"
    echo "  2. 在 Unity Hub 中添加项目"
    echo "  3. 开始开发！"
}

# 构建游戏
cmd_build() {
    echo -e "${BLUE}Building game...${NC}"
    
    # 解析选项
    local platforms=""
    while [[ $# -gt 0 ]]; do
        case $1 in
            --platform|-p)
                platforms="$2"
                shift 2
                ;;
            --output|-o)
                output_dir="$2"
                shift 2
                ;;
            *)
                shift
                ;;
        esac
    done
    
    echo -e "${YELLOW}注意：构建需要在 Unity 中执行${NC}"
    echo ""
    echo "使用 Unity 命令行构建:"
    echo "  unity-editor -batchmode -quit -projectPath ."
    echo "    -executeMethod BuildScript.PerformBuild"
    echo "    -buildTarget $platforms"
    echo ""
    
    # 创建构建脚本
    mkdir -p Editor
    cat > Editor/BuildScript.cs << 'EOF'
using UnityEditor;
using UnityEngine;

public class BuildScript
{
    [MenuItem("Framework/Build/Windows")]
    public static void BuildWindows()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            target = BuildTarget.StandaloneWindows64,
            locationPathName = "Builds/Windows/Game.exe"
        };
        
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
    
    [MenuItem("Framework/Build/Mac")]
    public static void BuildMac()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            target = BuildTarget.StandaloneOSX,
            locationPathName = "Builds/Mac/Game.app"
        };
        
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
    
    [MenuItem("Framework/Build/Android")]
    public static void BuildAndroid()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            target = BuildTarget.Android,
            locationPathName = "Builds/Android/Game.apk"
        };
        
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
    
    private static string[] GetEnabledScenes()
    {
        // 获取所有启用的场景
        var scenes = new System.Collections.Generic.List<string>();
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            if (EditorBuildSettings.scenes[i].enabled)
            {
                scenes.Add(EditorBuildSettings.scenes[i].path);
            }
        }
        return scenes.ToArray();
    }
}
EOF
    
    echo -e "${GREEN}✓ 构建脚本已创建 (Editor/BuildScript.cs)${NC}"
}

# 运行测试
cmd_test() {
    echo -e "${BLUE}Running tests...${NC}"
    
    # 解析选项
    local all=false
    while [[ $# -gt 0 ]]; do
        case $1 in
            --all|-a)
                all=true
                shift
                ;;
            *)
                shift
                ;;
        esac
    done
    
    if [ "$all" = true ]; then
        # 运行所有测试
        echo "Running all framework tests..."
        "$FRAMEWORK_DIR/tools/scripts/test-all.sh"
    else
        # 运行项目测试
        if [ -f "tests/run-tests.sh" ]; then
            ./tests/run-tests.sh
        else
            echo -e "${YELLOW}No tests found${NC}"
        fi
    fi
}

# 生成热更包
cmd_hotfix() {
    echo -e "${BLUE}Generating hotfix package...${NC}"
    
    local version=""
    while [[ $# -gt 0 ]]; do
        case $1 in
            --version|-v)
                version="$2"
                shift 2
                ;;
            *)
                shift
                ;;
        esac
    done
    
    if [ -z "$version" ]; then
        version=$(date +%Y%m%d%H%M%S)
    fi
    
    # 创建热更目录
    HOTFIX_DIR="hotfix_$version"
    mkdir -p "$HOTFIX_DIR"
    
    # 复制配置
    if [ -d "data/config" ]; then
        cp -r data/config "$HOTFIX_DIR/"
    fi
    
    # 生成版本信息
    cat > "$HOTFIX_DIR/version.json" << EOF
{
    "version": "$version",
    "timestamp": "$(date -Iseconds)",
    "files": []
}
EOF
    
    # 打包
    if command -v zip &> /dev/null; then
        zip -r "$HOTFIX_DIR.zip" "$HOTFIX_DIR"
        echo -e "${GREEN}✓ 热更包已生成：${HOTFIX_DIR}.zip${NC}"
    else
        echo -e "${GREEN}✓ 热更文件已生成：$HOTFIX_DIR/${NC}"
    fi
}

# 清理
cmd_clean() {
    echo -e "${BLUE}Cleaning build files...${NC}"
    
    rm -rf Builds/
    rm -rf Library/
    rm -rf Temp/
    rm -rf Obj/
    rm -rf Logs/
    rm -f *.apk *.aab
    
    echo -e "${GREEN}✓ 清理完成${NC}"
}

# 主函数
main() {
    if [ $# -eq 0 ]; then
        print_help
        exit 0
    fi
    
    local command=$1
    shift
    
    case $command in
        new)
            cmd_new "$@"
            ;;
        build)
            cmd_build "$@"
            ;;
        test)
            cmd_test "$@"
            ;;
        hotfix)
            cmd_hotfix "$@"
            ;;
        clean)
            cmd_clean "$@"
            ;;
        help|--help|-h)
            print_help
            ;;
        *)
            echo -e "${RED}未知命令：$command${NC}"
            echo ""
            print_help
            exit 1
            ;;
    esac
}

main "$@"
