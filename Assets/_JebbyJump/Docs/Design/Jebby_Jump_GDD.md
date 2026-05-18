# Jebby Jump｜Game Design Document v0.3

## 1. Overview

**Game Name:** Jebby Jump  
**Genre:** 2D memory platformer / casual action puzzle  
**Engine:** Unity 6 LTS  
**Render Pipeline:** URP 2D  
**Core Character:** Jebby the Color Knight  
**Target Platforms:** Windows for development/testing, Android/mobile later, WebGL demo later, iOS later if needed

## 2. One-line Pitch

**Jebby Jump** is a colorful 2D memory platformer where players memorize a sequence of colors, then physically control Jebby to jump upward through colored platforms in the correct order.

## 3. Core Design Statement

```text
Memory tells the player where to go.
Skill decides whether they can get there.
Items give clever ways to survive.
```

## 4. Character Direction

Jebby is **not a literal dog**.

Jebby is a **Cavalier King Charles Spaniel-inspired humanoid fantasy color knight**.

Default Jebby identity:

- Jebby the Color Knight
- Classic Cavalier as default identity
- Brown-and-ivory flowing ear-like hair
- Large warm eyes
- Small blue cape
- Little boots
- Rainbow gem badge
- Brave, gentle, magical, child-friendly
- Rainbow tower / color memory adventure theme

All visual work should follow:

```text
Assets/_JebbyJump/Docs/Art/Jebby_Jump_Art_Bible.md
```

Locked visual references:

```text
Assets/_JebbyJump/Docs/Art/References/jebby_color_knight_character_sheet_v01.png
Assets/_JebbyJump/Docs/Art/References/jebby_outfit_variations_board_v01.png
```

## 5. Target Experience

The player should feel:

- "I remembered the sequence."
- "I made the jump."
- "I climbed higher than before."
- "I want one more try."
- "Jebby feels brave, cute, and worth helping."

The game should be child-safe but not boring. It should be simple enough for kids and casual players, but later deep enough through timing, obstacles, and item choices.

## 6. Core Gameplay Loop

1. Player starts a level.
2. Game shows a color sequence.
3. Player memorizes the sequence.
4. Sequence is hidden.
5. Player controls Jebby with real platformer controls.
6. Jebby jumps upward through colored platforms.
7. Landing on the correct color advances the sequence.
8. Landing on the wrong color costs a life.
9. Completing the full sequence clears the level.
10. Losing all lives triggers game over.
11. Later: player earns rewards and may unlock outfits/items.

## 7. Core Mechanics

### 7.1 Memory Sequence

At the beginning of a level, the game shows a sequence such as:

```text
Red → Blue → Yellow → Green
```

The player must remember the order.

### 7.2 Physical Platforming

Unlike click-to-jump games, Jebby uses real Rigidbody2D platformer movement:

- Horizontal movement
- Jump
- Variable jump height
- Coyote time
- Jump buffering
- Fall gravity multiplier
- Low-jump gravity multiplier
- Max fall speed
- Ground detection
- Landing detection

### 7.3 Colored Platforms

Each platform has identity:

- `PlatformColor`
- `RowIndex`

Example:

```text
Row 0: Red
Row 1: Blue
Row 2: Yellow
```

### 7.4 Landing Detection

When Jebby lands on a platform, the system should know:

- Which platform
- Which color
- Which row index
- Whether landing was from above

Phase 5 introduces this system without correctness validation.

### 7.5 Correctness Validation

Row validation rule:

```text
Row < CurrentStepIndex  → ignore (already-completed row, player still standing on it)
Row == CurrentStepIndex → validate color
Row > CurrentStepIndex  → wrong landing / lose life
```

Correct landing:

- Advance sequence index
- Add score
- Continue upward

Wrong landing:

- Lose life
- Reset sequence progress
- Respawn to start
- If lives reach 0, game over

## 8. Controls

### Desktop

```text
A / D or Left / Right = move
Space = jump
Shift or J = use item later
Esc = pause
```

### Mobile Later

```text
Left virtual button = move left
Right virtual button = move right
Jump virtual button = jump
Item virtual button = use item later
```

Gameplay code must read through `InputReader`, not directly from keyboard or mobile UI.

## 9. MVP Scope

### MVP Must Include

- Boot / MainMenu / Game scenes
- Input System abstraction
- Rigidbody2D player controller
- Cinemachine follow camera
- Platform identity
- Landing detection
- Memory sequence system
- LevelConfig-driven levels
- Platform row generation
- Correct/wrong landing validation
- Lives
- Score
- Respawn/checkpoint
- Level complete / game over
- 3 MVP levels
- Basic UI for memory sequence, lives, score, result screens

### MVP Should Not Include

- Wardrobe system
- Outfit fragments
- Shop
- Monetization
- IAP
- Analytics
- Online leaderboard
- Cloud save
- Networking
- Lua / HybridCLR / ILRuntime code hot update
- Full art polish
- Addressables remote content

## 10. Level Design

### MVP Levels

| Level | Sequence Length | Platforms/Row | Memory Time | Obstacles | Jitter |
|---|---:|---:|---:|---|---|
| Level 1 | 4 | 2 | 5 sec | None | 0 (flat) |
| Level 2 | 5 | 2 | 5 sec | Cactus 25% | 0 (flat) |
| Level 3 | 6 | 3 | 5 sec | Cactus 35% | 0.3 (small stagger) |

Level 3 uses `_rowVerticalJitter = 0.3`, giving platforms in the same row a small random Y offset (±0.3u). RowIndex is unaffected — Y position does not define the sequence step.

### Difficulty Growth

Difficulty can increase by:

- Longer sequences
- More colors
- Less memory time
- Smaller platforms
- Wider platform spacing
- Same-row vertical jitter
- Moving platforms
- Obstacles
- Similar color distractions
- Fewer safe landing options

## 11. Platform System

### Platform Data

Each platform should store:

```text
PlatformColor color
int rowIndex
```

### Platform Rules

- Platforms are physical objects with Collider2D.
- Platform identity is separate from gameplay validation.
- Platform does not know whether it is correct.
- Platform does not know about lives, score, or memory sequence.

### One-Way Colored Platforms

Default colored memory platforms are **one-way platforms**.

Jebby can:

```text
jump upward through them from below
land on them from above
```

This creates a smoother vertical climbing experience and keeps the challenge focused on:

```text
color memory
jump timing
landing accuracy
```

Default colored platforms should use:

```text
BoxCollider2D
PlatformEffector2D
BoxCollider2D.usedByEffector = true
PlatformEffector2D.useOneWay = true
PlatformEffector2D.surfaceArc = 180
```

Expected behavior:

```text
Jebby jumps from below → passes through
Jebby falls from above → lands on top
```

Solid blocking platforms may be added later as a separate platform type for puzzle or obstacle levels. They should not be the default behavior for colored memory platforms.

### Obstacles on One-Way Platforms

Obstacles placed on platforms, such as cactus, must be separate child objects with their own colliders.

The platform collider controls the one-way landing surface. The obstacle collider controls hazard or blocking behavior.

Example hierarchy:

```text
Platform_Row1_Blue
  ├── PlatformCollider / BoxCollider2D + PlatformEffector2D
  └── CactusObstacle / separate Collider2D
```

A cactus should not be part of the one-way platform collider. It should have its own collider, usually a trigger collider in the first implementation.

Initial cactus rule:

```text
Touching cactus causes damage or costs one life.
```

Advanced cactus rule later:

```text
Platforms may be divided into left, center, and right landing zones, allowing cactus to make only part of a platform unsafe.
```

## 12. Memory System

Core responsibilities:

- Generate sequence
- Display sequence
- Countdown
- Hide sequence
- Track current expected step
- Expose expected color for validation

Suggested scripts later:

```text
ColorSequenceGenerator
MemoryPhaseController
SequenceValidator
```

## 13. Level Generation

Use `LevelConfig` ScriptableObjects.

A level config may include:

```text
levelId
sequenceLength
availableColors
memoryTimeSeconds
startingLives
platformsPerRowMin
platformsPerRowMax
rowVerticalSpacing
rowHorizontalSpread
rowVerticalJitter
obstaclesEnabled
themeId
```

For MVP, use conservative reachable layouts rather than fully random difficult generation.

## 14. Scoring and Lives

### Suggested MVP Score

```text
Correct landing: +10
Level complete: +50
Remaining life: +20 each
```

### Lives

MVP default:

```text
startingLives = 3
```

Wrong landing:

```text
lives -= 1
```

Lives reach zero:

```text
GameOver
```

## 15. Obstacles

### First Obstacle: Cactus

Cactus must be implemented as a **separate child obstacle object**, not as part of the platform's one-way collider.

Initial simple rule:

```text
Touching cactus causes damage or costs one life.
```

Recommended first implementation:

```text
CactusObstacle.cs
Collider2D with isTrigger = true
Placed as a child object on the platform edge
```

This allows the platform to stay smooth and one-way while the cactus area remains dangerous.

Later advanced rule:

```text
Cactus on one side of platform makes that landing zone unsafe.
```

For example:

```text
Cactus on right edge = right side unsafe
Landing on left or center remains safe
```

### Future Obstacles

- Ice platform
- Cracked platform
- Cloud platform
- Moving platform
- Wind zone
- Spike ball

## 16. Advanced Platform Layout

### Same-Row Vertical Jitter

Platforms within the same logical row may have slightly different Y positions (vertical jitter). This creates visual variety and mild horizontal navigation challenges without changing sequence validation.

Key invariant:

```text
RowIndex is assigned from the row loop counter, not from Y position.
All platforms in the same logical row share the same RowIndex.
Validation is based on RowIndex and PlatformColor, not on world Y.
```

Config field:

```text
LevelConfig._rowVerticalJitter (float, default 0, clamped 0–1)
0     = flat rows (Level 1, Level 2 default)
0.3   = small stagger (Level 3)
1.0   = maximum safe jitter
```

Reachability rule:

```text
Max jitter must be < (max jump height − row vertical spacing).
With jump force 10 and gravity 9.81: max safe jitter ≈ 1.0u.
```

Cactus placement uses the actual staggered platform transform position. No changes to cactus spawning logic are needed.

## 17. Item System

Not part of MVP gameplay implementation, but architecture should remain item-ready.

Future item categories:

- Movement
- Time
- Defense
- Memory helper
- Utility
- Economy

### First Future Items

#### Rocket Boots

Rocket Boots is **deferred until Phase 19+ advanced platform layouts create a meaningful same-row mobility challenge**.

Rocket Boots must NOT be a row-skipping power-up.

When implemented, Rocket Boots may assist:
- Same-row mobility
- Wider same-row gaps or staggered same-row platforms requiring horizontal reach
- Cactus avoidance
- Recovery from poor positioning

Rocket Boots must NOT:
- Bypass row validation
- Allow intentional sequence-row skipping
- Ignore wrong-color validation
- Function as a shortcut around the memory sequence

#### Bullet Time

Modify:

```text
timeScale
air-time control window
limited charges
```

#### Bubble Shield

Protects from one mistake or hazard.

#### Color Echo

Briefly reminds the player of the next/remaining colors.

## 18. Wardrobe / Outfit System

Approved as future product direction, **not MVP**.

### Product Direction

```text
MVP: Default Jebby only
V2: Basic outfit preview or 1–2 unlockable cosmetic outfits
V3: Full Jebby's Wardrobe with outfit fragments
```

### Unlock Concept

Players collect outfit fragments. When all fragments are collected, the outfit unlocks.

Initial future outfit candidates:

- Forest Cavalier
- Sunshine Knight
- Aqua Knight
- Silver Dreamer

### Rule

Outfits are cosmetic first. Do not add wardrobe, outfit fragments, cosmetic inventory, save data, or outfit economy until explicitly approved.

## 19. Art Direction

Defined in:

```text
Assets/_JebbyJump/Docs/Art/Jebby_Jump_Art_Bible.md
```

Key style:

- Bright
- Magical
- Friendly
- Child-safe
- Soft fantasy
- Premium indie
- Clear gameplay readability

## 20. Technical Direction

### Engine Stack

```text
Unity 6 LTS
URP 2D
Rigidbody2D
Unity Input System
Cinemachine 3
ScriptableObjects
UGUI for MVP UI
Config-driven architecture
```

### Live Update Strategy

MVP does not implement code hot update.

Use config-driven architecture:

- Level configs
- Difficulty settings
- Item values
- Obstacle rules
- Tutorial/localization text later
- Addressable assets later

No Lua / HybridCLR / ILRuntime for MVP.

## 21. Current Phase Plan

1. Project foundation ✅
2. Packages + Input System setup ✅
3. Rigidbody2D Player Controller ✅
4. Cinemachine + test scene ✅
5. Platform identity + landing detection ✅
6. Memory sequence system ✅
7. LevelConfig + platform row generation ✅
8. Lives / score / respawn / level complete ✅
9. Mobile virtual controls polish ✅
10. Cactus obstacle + item-ready player stats ✅
11. In-game HUD + game over / level complete panels ✅
12. Retry / restart level flow ✅
13. Boot + Main Menu + scene flow foundation ✅
14. Basic audio feedback (SFX) ✅
15. MVP level set + session level progression ✅
16. Playability tuning / MVP balance pass ✅
17. MVP polish — visual readability + UX feedback ✅
18. Basic tutorial / first-time onboarding ✅
19. Advanced platform layout foundation (_rowVerticalJitter) ✅
20. Rocket Boots Prototype — only after Phase 19 creates a meaningful same-row mobility challenge

## 22. MVP Completion Criteria

The MVP is complete when:

- A player can launch the game.
- The player can start a level.
- A color sequence is shown.
- The sequence is hidden after countdown.
- The player controls Jebby with real movement.
- Jebby jumps through colored platforms.
- The game validates correct/wrong color order.
- The player can win or lose.
- There are at least 3 playable levels.
- The codebase supports future obstacles, items, and wardrobe without major rewrites.
