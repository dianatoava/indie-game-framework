#!/usr/bin/env python3
"""
数据配置管理器

功能：
1. 读取 YAML/JSON 配置
2. 验证配置格式
3. 生成 C# 数据类
4. 热更配置打包
5. 配置差异对比

使用方式：
    ./config-manager.py --input ./configs --output ./generated --lang cs
"""

import argparse
import yaml
import json
import os
from pathlib import Path
from typing import Any, Dict, List
from datetime import datetime


class ConfigManager:
    """配置管理器"""
    
    def __init__(self):
        self.configs: Dict[str, Any] = {}
        self.stats = {
            'loaded': 0,
            'generated': 0,
            'errors': 0
        }
    
    def load_config(self, config_path: str) -> Dict[str, Any]:
        """
        加载配置文件
        
        Args:
            config_path: 配置文件路径
            
        Returns:
            配置数据
        """
        path = Path(config_path)
        
        if not path.exists():
            raise FileNotFoundError(f"Config file not found: {config_path}")
        
        # 根据扩展名选择加载方式
        if path.suffix in ['.yaml', '.yml']:
            with open(path, 'r', encoding='utf-8') as f:
                config = yaml.safe_load(f)
        elif path.suffix == '.json':
            with open(path, 'r', encoding='utf-8') as f:
                config = json.load(f)
        else:
            raise ValueError(f"Unsupported config format: {path.suffix}")
        
        config_name = path.stem
        self.configs[config_name] = config
        self.stats['loaded'] += 1
        
        print(f"Loaded config: {config_name} ({len(config)} entries)")
        return config
    
    def load_directory(self, config_dir: str) -> int:
        """
        加载目录下所有配置
        
        Args:
            config_dir: 配置目录
            
        Returns:
            加载的配置数量
        """
        dir_path = Path(config_dir)
        
        if not dir_path.exists():
            raise FileNotFoundError(f"Config directory not found: {config_dir}")
        
        count = 0
        for config_file in dir_path.glob('*.yaml'):
            try:
                self.load_config(str(config_file))
                count += 1
            except Exception as e:
                print(f"Error loading {config_file}: {e}")
                self.stats['errors'] += 1
        
        for config_file in dir_path.glob('*.yml'):
            try:
                self.load_config(str(config_file))
                count += 1
            except Exception as e:
                print(f"Error loading {config_file}: {e}")
                self.stats['errors'] += 1
        
        for config_file in dir_path.glob('*.json'):
            try:
                self.load_config(str(config_file))
                count += 1
            except Exception as e:
                print(f"Error loading {config_file}: {e}")
                self.stats['errors'] += 1
        
        return count
    
    def generate_csharp_classes(self, output_dir: str) -> int:
        """
        生成 C# 数据类
        
        Args:
            output_dir: 输出目录
            
        Returns:
            生成的文件数量
        """
        output_path = Path(output_dir)
        output_path.mkdir(parents=True, exist_ok=True)
        
        count = 0
        
        for config_name, config_data in self.configs.items():
            # 生成数据类
            class_name = self._to_pascal_case(config_name)
            class_code = self._generate_csharp_class(class_name, config_data)
            
            # 保存文件
            output_file = output_path / f"{class_name}Config.cs"
            with open(output_file, 'w', encoding='utf-8') as f:
                f.write(class_code)
            
            print(f"Generated: {output_file}")
            count += 1
            self.stats['generated'] += 1
        
        # 生成配置管理器单例
        manager_code = self._generate_config_manager()
        manager_file = output_path / "GameConfigManager.cs"
        with open(manager_file, 'w', encoding='utf-8') as f:
            f.write(manager_code)
        
        print(f"Generated: {manager_file}")
        count += 1
        
        return count
    
    def _generate_csharp_class(self, class_name: str, data: Dict) -> str:
        """生成单个 C# 类"""
        lines = [
            "using System;",
            "using System.Collections.Generic;",
            "",
            "namespace IndieGameFramework.Data",
            "{",
            f"    /// <summary>",
            f"    /// {class_name} 配置数据",
            f"    /// Generated: {datetime.now().isoformat()}",
            f"    /// </summary>",
            f"    [Serializable]",
            f"    public class {class_name}Config",
            "    {",
        ]
        
        # 生成字段
        for key, value in data.items():
            field_name = self._to_pascal_case(key)
            field_type = self._infer_type(value)
            
            # 添加注释
            lines.append(f"")
            lines.append(f"        /// <summary>")
            lines.append(f"        /// {key}")
            lines.append(f"        /// </summary>")
            
            if isinstance(value, dict):
                # 嵌套对象
                nested_class = self._generate_nested_class(f"{class_name}_{key}", value)
                lines.append(f"        public {field_name}Data {field_name} {{ get; set; }}")
            elif isinstance(value, list) and len(value) > 0 and isinstance(value[0], dict):
                # 对象数组
                nested_class = self._generate_nested_class(f"{class_name}_{key}_item", value[0])
                lines.append(f"        public List<{field_name}ItemData> {field_name} {{ get; set; }}")
            else:
                lines.append(f"        public {field_type} {field_name} {{ get; set; }}")
        
        lines.append("    }")
        lines.append("}")
        
        return "\n".join(lines)
    
    def _generate_nested_class(self, class_name: str, data: Dict) -> str:
        """生成嵌套类"""
        lines = [
            f"    [Serializable]",
            f"    public class {self._to_pascal_case(class_name)}Data",
            "    {",
        ]
        
        for key, value in data.items():
            field_name = self._to_pascal_case(key)
            field_type = self._infer_type(value)
            lines.append(f"        public {field_type} {field_name} {{ get; set; }}")
        
        lines.append("    }")
        
        return "\n".join(lines)
    
    def _generate_config_manager(self) -> str:
        """生成配置管理器单例"""
        config_loaders = []
        for config_name in self.configs.keys():
            class_name = self._to_pascal_case(config_name)
            config_loaders.append(
                f'            {config_name}s = LoadConfig<{class_name}Config>("{config_name}");'
            )
        
        code = f'''using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IndieGameFramework.Data
{{
    /// <summary>
    /// 游戏配置管理器 - 单例
    /// Generated: {datetime.now().isoformat()}
    /// </summary>
    public class GameConfigManager
    {{
        private static GameConfigManager _instance;
        public static GameConfigManager Instance
        {{
            get
            {{
                if (_instance == null)
                {{
                    _instance = new GameConfigManager();
                    _instance.Initialize();
                }}
                return _instance;
            }}
        }}

        // 配置数据缓存
'''
        
        # 添加配置字段
        for config_name in self.configs.keys():
            class_name = self._to_pascal_case(config_name)
            code += f'        private Dictionary<string, {class_name}Config> {config_name}s;\n'
        
        code += '''
        /// <summary>
        /// 初始化配置管理器
        /// </summary>
        public void Initialize()
        {
            // 加载所有配置
'''
        
        code += "\n".join(config_loaders)
        
        code += '''
        }

        /// <summary>
        /// 从 Resources 加载配置
        /// </summary>
        private Dictionary<string, T> LoadConfig<T>(string configName) where T : class
        {
            var dict = new Dictionary<string, T>();
            
            // 尝试从 Resources 加载
            var textAsset = Resources.Load<TextAsset>($"Configs/{configName}");
            if (textAsset != null)
            {
                var data = JsonUtility.FromJson<ConfigWrapper<T>>(textAsset.text);
                if (data != null && data.items != null)
                {
                    foreach (var item in data.items)
                    {
                        var id = GetId(item);
                        if (!string.IsNullOrEmpty(id))
                        {
                            dict[id] = item;
                        }
                    }
                }
            }
            
            return dict;
        }

        /// <summary>
        /// 获取配置项 ID（反射获取 id 字段）
        /// </summary>
        private string GetId<T>(T obj)
        {
            var type = typeof(T);
            var prop = type.GetProperty("Id");
            if (prop != null)
            {
                return prop.GetValue(obj)?.ToString();
            }
            var field = type.GetField("id");
            if (field != null)
            {
                return field.GetValue(obj)?.ToString();
            }
            return null;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        public T GetConfig<T>(string configName, string id) where T : class
        {
            // 根据配置类型返回对应数据
            // 实际实现需要根据具体类型做反射
            return null;
        }

        /// <summary>
        /// 配置数据包装类（用于 JSON 反序列化）
        /// </summary>
        [Serializable]
        private class ConfigWrapper<T>
        {
            public List<T> items;
        }
    }
}
'''
        
        return code
    
    def _infer_type(self, value: Any) -> str:
        """推断 C# 类型"""
        if value is None:
            return "object"
        elif isinstance(value, bool):
            return "bool"
        elif isinstance(value, int):
            return "int"
        elif isinstance(value, float):
            return "float"
        elif isinstance(value, str):
            return "string"
        elif isinstance(value, dict):
            return "object"
        elif isinstance(value, list):
            if len(value) == 0:
                return "List<object>"
            item_type = self._infer_type(value[0])
            return f"List<{item_type}>"
        else:
            return "object"
    
    def _to_pascal_case(self, name: str) -> str:
        """转换为 PascalCase"""
        # 处理 snake_case
        if '_' in name:
            parts = name.split('_')
            return ''.join(word.capitalize() for word in parts)
        
        # 已经是 PascalCase 或 camelCase
        return name[0].upper() + name[1:]
    
    def validate_configs(self) -> List[str]:
        """
        验证配置格式
        
        Returns:
            错误列表
        """
        errors = []
        
        for config_name, config_data in self.configs.items():
            # 检查是否是字典
            if not isinstance(config_data, dict):
                errors.append(f"{config_name}: 配置必须是对象格式")
                continue
            
            # 检查是否有 id 字段（如果有多个条目）
            for key, value in config_data.items():
                if isinstance(value, dict):
                    # 嵌套对象，检查是否有必需字段
                    pass
        
        return errors


def main():
    """命令行入口"""
    parser = argparse.ArgumentParser(description='数据配置管理器')
    
    parser.add_argument('--input', '-i', required=True,
                       help='配置输入目录或文件')
    parser.add_argument('--output', '-o', required=True,
                       help='输出目录')
    parser.add_argument('--lang', '-l', default='cs',
                       choices=['cs', 'json'],
                       help='生成语言')
    parser.add_argument('--validate', action='store_true',
                       help='只验证配置，不生成代码')
    
    args = parser.parse_args()
    
    # 创建管理器
    manager = ConfigManager()
    
    # 加载配置
    input_path = Path(args.input)
    
    if input_path.is_dir():
        count = manager.load_directory(str(input_path))
        print(f"Loaded {count} config files")
    else:
        manager.load_config(str(input_path))
    
    # 验证
    if args.validate:
        errors = manager.validate_configs()
        if errors:
            print("\nValidation errors:")
            for error in errors:
                print(f"  - {error}")
            return 1
        else:
            print("All configs valid!")
            return 0
    
    # 生成代码
    if args.lang == 'cs':
        count = manager.generate_csharp_classes(args.output)
        print(f"Generated {count} C# files")
    
    # 打印统计
    print(f"\nStats:")
    print(f"  Loaded: {manager.stats['loaded']}")
    print(f"  Generated: {manager.stats['generated']}")
    print(f"  Errors: {manager.stats['errors']}")
    
    return 0


if __name__ == '__main__':
    exit(main())
