# AI 美术素材生成提示词

用于生成统一风格的 2D 游戏美术素材。

---

## 角色 Sprite 提示词（英文，适合 Midjourney/Stable Diffusion）

### 基础角色风格（300字）

```
A 2D pixel art game character sprite sheet, cute chibi warrior with 
big head and small body proportions, wearing detailed silver armor 
with blue gem accents and flowing red cape, holding a glowing magical 
sword with particle effects, multiple animation frames showing idle 
stance, running cycle, attack swing, and jump poses, 16-bit retro 
gaming aesthetic with limited color palette using dithering techniques, 
pure transparent background for game integration, consistent top-left 
lighting direction creating readable shadows, clean pixel edges without 
anti-aliasing for crisp appearance, inspired by Stardew Valley and 
Celeste art styles, game asset ready for Unity import, organized in 
4x4 sprite sheet grid layout, each frame precisely 64x64 pixels, 
high contrast silhouette for clear readability, fantasy RPG theme 
with magical elements, warm color temperature with orange and gold 
highlights complementing the cool silver armor tones
```

### 敌人角色（280字）

```
2D pixel art game enemy sprite, menacing dark knight with spiked 
black armor and glowing red eyes, hunched aggressive posture suggesting 
hostility, holding massive cleaver weapon with blood stains, idle 
animation frame with heavy breathing motion implied, attack animation 
frame showing weapon raised high, death animation frame with collapsing 
pose, 16-bit pixel art style matching hero character, limited 16-color 
palette with dark purples and crimson reds, transparent background, 
consistent lighting from above, sharp angular pixel edges, inspired 
by classic Castlevania enemy designs, game asset sprite sheet format, 
64x64 pixel dimensions per frame, high contrast for visibility against 
various backgrounds, horror fantasy theme with gothic elements, 
cohesive art style matching player character for visual consistency
```

---

## 环境/场景提示词

### 瓦片地图风格（320字）

```
2D pixel art game environment tileset, medieval village setting with 
cobblestone paths and thatched roof cottages, multiple terrain types 
including grass, dirt, stone pavement, and wooden plank flooring, 
seamless tiling textures with no visible edges when repeated, 16-bit 
retro pixel art aesthetic matching character sprites, limited 32-color 
palette with earth tones and autumn colors, daytime lighting with soft 
ambient occlusion shadows, top-down 45-degree perspective for RPG view, 
includes ground tiles, building walls, roof sections, fences, and 
decorative elements like flowers, rocks, and signs, each tile precisely 
32x32 pixels, transparent background for object tiles, inspired by 
Stardew Valley and Harvest Moon environmental art, cozy village 
atmosphere with warm orange and brown color scheme, consistent pixel 
density and style across all tiles, game-ready asset for Unity tilemap 
system
```

### 背景场景（290字）

```
2D pixel art game background, fantasy forest with towering ancient 
trees and mystical glowing mushrooms, layered parallax scrolling 
design with foreground, midground, and distant elements, atmospheric 
perspective with desaturated distant colors, 16-bit pixel art style 
matching game aesthetic, limited color palette with deep greens and 
magical cyan highlights, soft lighting filtering through canopy, 
seamless horizontal tiling for infinite scrolling, 512x512 pixel 
dimensions, no transparent areas as it's a full background, inspired 
by Ori and the Blind Forest backgrounds, enchanted forest theme with 
fairy tale elements, depth created through color temperature shifts 
from warm foreground to cool background, game asset ready for Unity 
parallax implementation
```

---

## UI 元素提示词

### 游戏 UI 套装（260字）

```
2D pixel art game UI elements set, RPG-style interface components 
including rectangular buttons with beveled borders, circular skill 
icons with metallic frames, health and mana bars with gradient fills, 
inventory slot backgrounds with subtle depth, panel windows with 
decorative corner ornaments, 16-bit pixel art style matching game 
aesthetic, limited color palette with gold accents and dark wood 
tones, transparent backgrounds for overlay elements, consistent 1-pixel 
outline thickness, inspired by Final Fantasy and Chrono Trigger UI 
designs, game asset sprite sheet format, various sizes from 32x32 
icons to 256x64 panels, high contrast for readability, fantasy RPG 
theme with medieval craftsmanship details, cohesive visual language 
across all UI elements
```

---

## 特效/粒子提示词

### 魔法特效（240字）

```
2D pixel art game magic effect animation frames, swirling blue energy 
orb with electric sparks and particle trails, multiple animation frames 
showing charge-up, burst, and dissipate phases, 16-bit pixel art style 
with glow effects and transparency, limited color palette with cyan 
and white highlights on dark blue base, transparent background for 
overlay on game world, additive blend mode suitable for glow effects, 
64x64 pixel frame size, 8 frames of animation, inspired by classic 
RPG spell effects, magical fantasy theme with elemental energy 
visualization, game asset ready for Unity particle system integration
```

---

## 使用建议

### 风格统一技巧

1. **固定艺术家参考** - 始终引用相同的游戏（如 Stardew Valley）
2. **固定技术参数** - 始终指定 "16-bit pixel art"、"transparent background"
3. **固定尺寸** - 始终指定像素尺寸（如 "64x64 pixels"）
4. **固定调色板** - 始终描述颜色范围（如 "limited 32-color palette"）

### 批量生成工作流

```
1. 使用基础提示词生成角色概念
2. 选择满意的风格
3. 固定该风格的关键描述词
4. 为不同角色/场景复用相同风格词
5. 在 Sprite Processor 中统一后处理
```

### 与框架集成

```
AI 生成图片
     ↓
auto-import/ai-generated/
     ↓
Sprite Processor（统一处理）
     ↓
Assets/Sprites/processed/
     ↓
Unity 中使用
```

---

## 推荐 AI 工具

| 工具 | 适用场景 | 备注 |
|------|----------|------|
| Midjourney | 概念设计 | 风格统一性好 |
| Stable Diffusion | 批量生成 | 可训练 LoRA 保持风格 |
| DALL-E 3 | 快速原型 | 理解力强 |
| Leonardo.ai | 游戏素材 | 有游戏素材模型 |

---

**提示**：生成后务必使用框架的 Sprite Processor 统一处理，确保风格一致！
