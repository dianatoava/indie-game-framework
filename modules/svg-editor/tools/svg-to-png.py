#!/usr/bin/env python3
"""
SVG 转 PNG 工具

使用 CairoSVG 将 SVG 转换为 PNG

安装依赖：
    pip3 install cairosvg

使用方式：
    ./svg-to-png.py input.svg --output output.png --size 512x512
"""

import argparse
import sys
from pathlib import Path

def convert_svg_to_png(input_path, output_path, width=None, height=None):
    """
    将 SVG 转换为 PNG
    
    Args:
        input_path: SVG 文件路径
        output_path: PNG 输出路径
        width: 输出宽度（可选）
        height: 输出高度（可选）
    """
    try:
        import cairosvg
    except ImportError:
        print("Error: cairosvg not installed")
        print("Run: pip3 install cairosvg")
        return False
    
    try:
        # 读取 SVG
        with open(input_path, 'rb') as f:
            svg_data = f.read()
        
        # 转换
        if width and height:
            cairosvg.svg2png(
                bytestring=svg_data,
                write_to=output_path,
                output_width=width,
                output_height=height
            )
        else:
            cairosvg.svg2png(
                bytestring=svg_data,
                write_to=output_path
            )
        
        print(f"Converted: {input_path} -> {output_path}")
        return True
        
    except Exception as e:
        print(f"Error: {e}")
        return False


def main():
    parser = argparse.ArgumentParser(description='SVG to PNG Converter')
    
    parser.add_argument('input', help='Input SVG file')
    parser.add_argument('--output', '-o', help='Output PNG file')
    parser.add_argument('--size', '-s', help='Output size (e.g., 512x512)')
    parser.add_argument('--width', '-w', type=int, help='Output width')
    parser.add_argument('--height', '-h', type=int, help='Output height')
    
    args = parser.parse_args()
    
    # 检查输入文件
    input_path = Path(args.input)
    if not input_path.exists():
        print(f"Error: File not found: {args.input}")
        return 1
    
    # 确定输出路径
    if args.output:
        output_path = Path(args.output)
    else:
        output_path = input_path.with_suffix('.png')
    
    # 解析尺寸
    width = args.width
    height = args.height
    
    if args.size:
        try:
            size_parts = args.size.split('x')
            width = int(size_parts[0])
            height = int(size_parts[1])
        except:
            print(f"Error: Invalid size format: {args.size}")
            return 1
    
    # 转换
    if convert_svg_to_png(str(input_path), str(output_path), width, height):
        return 0
    else:
        return 1


if __name__ == '__main__':
    sys.exit(main())
