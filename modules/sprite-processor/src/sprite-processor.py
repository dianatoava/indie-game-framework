#!/usr/bin/env python3
"""
Sprite 自动处理器

功能：
1. 去除 AI 生成图片的白色背景
2. 统一尺寸
3. 自动切割 Sprite Sheet
4. 设置像素完美过滤
5. 生成 Sprite Atlas 配置

使用方式：
    ./sprite-processor.py --input ./input --output ./output --config config.yaml
"""

import argparse
import yaml
import os
import sys
from pathlib import Path
from PIL import Image, ImageOps
import json

class SpriteProcessor:
    """Sprite 处理器"""
    
    def __init__(self, config_path=None):
        """
        初始化处理器
        
        Args:
            config_path: 配置文件路径
        """
        self.config = self._load_config(config_path)
        self.stats = {
            'processed': 0,
            'skipped': 0,
            'errors': 0
        }
    
    def _load_config(self, config_path):
        """加载配置文件"""
        if config_path and os.path.exists(config_path):
            with open(config_path, 'r', encoding='utf-8') as f:
                return yaml.safe_load(f)
        
        # 默认配置
        return {
            'default': {
                'target_size': [64, 64],
                'remove_bg': True,
                'bg_threshold': 240,
                'center_anchor': True,
                'filter_mode': 'point',
                'format': 'png'
            }
        }
    
    def process_directory(self, input_dir, output_dir, profile='default'):
        """
        处理目录中的所有图片
        
        Args:
            input_dir: 输入目录
            output_dir: 输出目录
            profile: 配置 profile 名称
        """
        input_path = Path(input_dir)
        output_path = Path(output_dir)
        
        # 创建输出目录
        output_path.mkdir(parents=True, exist_ok=True)
        
        # 获取配置
        config = self.config.get(profile, self.config['default'])
        
        # 处理所有图片
        image_extensions = {'.png', '.jpg', '.jpeg', '.webp'}
        image_files = [f for f in input_path.iterdir() 
                      if f.suffix.lower() in image_extensions]
        
        print(f"Found {len(image_files)} images in {input_dir}")
        
        for image_file in image_files:
            try:
                self._process_image(image_file, output_path, config)
                self.stats['processed'] += 1
            except Exception as e:
                print(f"Error processing {image_file}: {e}")
                self.stats['errors'] += 1
        
        # 生成处理报告
        self._generate_report(output_path)
        
        return self.stats
    
    def _process_image(self, input_path, output_path, config):
        """
        处理单张图片
        
        Args:
            input_path: 输入文件路径
            output_path: 输出目录路径
            config: 处理配置
        """
        print(f"Processing: {input_path.name}")
        
        # 打开图片
        img = Image.open(input_path)
        
        # 1. 去除白色背景
        if config.get('remove_bg', True):
            img = self._remove_background(img, config.get('bg_threshold', 240))
        
        # 2. 统一尺寸
        target_size = tuple(config.get('target_size', [64, 64]))
        img = self._resize_to_target(img, target_size, config.get('center_anchor', True))
        
        # 3. 保存
        output_file = output_path / f"{input_path.stem}_processed{input_path.suffix}"
        
        # 确保 RGBA 模式（PNG 需要）
        if img.mode != 'RGBA':
            img = img.convert('RGBA')
        
        img.save(output_file, 'PNG')
        print(f"  Saved: {output_file}")
    
    def _remove_background(self, img, threshold=240):
        """
        去除白色/浅色背景
        
        Args:
            img: PIL Image
            threshold: 亮度阈值，高于此值视为背景
            
        Returns:
            去背后的图片
        """
        # 转换为 RGBA
        if img.mode != 'RGBA':
            img = img.convert('RGBA')
        
        # 获取像素数据
        data = img.getdata()
        new_data = []
        
        for item in data:
            # 如果 RGB 都高于阈值，设为透明
            if item[0] > threshold and item[1] > threshold and item[2] > threshold:
                new_data.append((255, 255, 255, 0))
            else:
                new_data.append(item)
        
        img.putdata(new_data)
        return img
    
    def _resize_to_target(self, img, target_size, center_anchor=True):
        """
        调整到目标尺寸
        
        Args:
            img: PIL Image
            target_size: 目标尺寸 (width, height)
            center_anchor: 是否居中对齐
            
        Returns:
            调整后的图片
        """
        # 获取非透明区域边界
        bbox = img.getbbox()
        
        if bbox is None:
            # 完全透明的图片
            return Image.new('RGBA', target_size, (0, 0, 0, 0))
        
        # 裁剪到内容区域
        cropped = img.crop(bbox)
        
        # 创建新画布
        result = Image.new('RGBA', target_size, (0, 0, 0, 0))
        
        # 计算粘贴位置（居中）
        if center_anchor:
            paste_x = (target_size[0] - cropped.width) // 2
            paste_y = (target_size[1] - cropped.height) // 2
        else:
            paste_x = 0
            paste_y = 0
        
        # 粘贴
        result.paste(cropped, (paste_x, paste_y), cropped)
        
        return result
    
    def slice_sprite_sheet(self, input_path, output_dir, grid_size, config=None):
        """
        切割 Sprite Sheet
        
        Args:
            input_path: Sprite Sheet 路径
            output_dir: 输出目录
            grid_size: 网格大小 (cell_width, cell_height)
            config: 处理配置
        """
        if config is None:
            config = self.config.get('default', {})
        
        input_path = Path(input_path)
        output_path = Path(output_dir)
        output_path.mkdir(parents=True, exist_ok=True)
        
        # 打开 Sprite Sheet
        sheet = Image.open(input_path)
        
        # 计算行列数
        cols = sheet.width // grid_size[0]
        rows = sheet.height // grid_size[1]
        
        print(f"Slicing {input_path.name}: {cols}x{rows} grid")
        
        # 切割每个单元格
        for row in range(rows):
            for col in range(cols):
                # 计算裁剪区域
                left = col * grid_size[0]
                upper = row * grid_size[1]
                right = left + grid_size[0]
                lower = upper + grid_size[1]
                
                # 裁剪
                cell = sheet.crop((left, upper, right, lower))
                
                # 保存
                output_file = output_path / f"{input_path.stem}_{row:02d}_{col:02d}.png"
                cell.save(output_file, 'PNG')
        
        # 生成 Unity Sprite Atlas 配置
        self._generate_atlas_config(output_path, grid_size)
    
    def _generate_atlas_config(self, output_path, grid_size):
        """生成 Unity Sprite Atlas 配置"""
        config = {
            'spriteAtlas': {
                'gridSize': grid_size,
                'filterMode': 'Point',
                'pixelsPerUnit': 100
            }
        }
        
        config_file = output_path / 'atlas-config.json'
        with open(config_file, 'w', encoding='utf-8') as f:
            json.dump(config, f, indent=2)
        
        print(f"Generated atlas config: {config_file}")
    
    def _generate_report(self, output_path):
        """生成处理报告"""
        report = {
            'stats': self.stats,
            'timestamp': str(Path(output_path).stat().st_mtime)
        }
        
        report_file = output_path / 'processing-report.json'
        with open(report_file, 'w', encoding='utf-8') as f:
            json.dump(report, f, indent=2)
        
        print(f"\nProcessing Report:")
        print(f"  Processed: {self.stats['processed']}")
        print(f"  Skipped: {self.stats['skipped']}")
        print(f"  Errors: {self.stats['errors']}")


def main():
    """命令行入口"""
    parser = argparse.ArgumentParser(description='Sprite 自动处理器')
    
    parser.add_argument('--input', '-i', required=True, 
                       help='输入目录')
    parser.add_argument('--output', '-o', required=True,
                       help='输出目录')
    parser.add_argument('--config', '-c', default=None,
                       help='配置文件路径')
    parser.add_argument('--profile', '-p', default='default',
                       help='配置 profile 名称')
    parser.add_argument('--slice', action='store_true',
                       help='切割 Sprite Sheet 模式')
    parser.add_argument('--grid-size', type=int, nargs=2, default=[64, 64],
                       help='Sprite Sheet 网格大小')
    
    args = parser.parse_args()
    
    # 创建处理器
    processor = SpriteProcessor(args.config)
    
    if args.slice:
        # Sprite Sheet 切割模式
        input_files = list(Path(args.input).glob('*.png'))
        output_path = Path(args.output)
        
        for input_file in input_files:
            processor.slice_sprite_sheet(
                input_file,
                output_path,
                tuple(args.grid_size)
            )
    else:
        # 普通处理模式
        processor.process_directory(
            args.input,
            args.output,
            args.profile
        )


if __name__ == '__main__':
    main()
