# Jebby Jump｜Game Design Document v0.4

## 1. Overview

**Game Name:** Jebby Jump  
**Genre:** 2D memory platformer / casual action puzzle  
**Engine:** Unity 6 LTS  
**Render Pipeline:** URP 2D  
**Core Character:** Jebby the Color Knight  
**Target Platforms:** Windows for development/testing, Android/mobile later, WebGL demo later, iOS later if needed

## 2. One-line Pitch

**Jebby Jump** is a colorful 2D memory platformer where players memorize a color sequence, then physically control Jebby to jump upward through colored platforms in the correct order.

## 3. Core Design Statement

```text
Memory tells the player where to go.
Skill decides whether they can get there.
Equipment and active skills help the player recover, reposition, or solve harder layouts.
Equipment and active skills must not bypass the memory sequence.
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

- “I remembered the sequence.”
- “I made the jump.”
- “I climbed higher than before.”
- “I used Jebby’s gear at the right moment.”
- “I want one more try.”
- “Jebby feels brave, cute, and worth helping.”

The game should be child-safe but not boring. It should be simple enough for kids and casual players, but later deep enough through timing, layout variation, obstacles, equipment, and active skill choices.

## 6. Core Gameplay Loop

1. Player starts a level.
2. Game shows a color sequence.
3. Player memorizes the sequence.
4. Sequence is hidden.
5. Player controls Jebby with real platformer controls.
6. Jebby jumps upward through colored platforms.
7. Landing on the correct color at the current expected row advances the sequence.
8. Landing on the wrong color at the current expected row costs a life.
9. Landing on a future row before the expected step costs a life.
10. Landing on a completed / lower row is ignored.
11. Completing the full sequence clears the level.
12. Losing all lives triggers game over.
13. Later: player may equip gear / active skills before a level and unlock cosmetics.

## 7. Core Mechanics

### 7.1 Memory Sequence

At the beginning of a level, the game shows a sequence such as:

```text
Red → Blue → Yellow → Green
```

The player must remember the order.

### 7.2 Physical Platforming

Jebby uses Rigidbody2D platformer movement:

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

```text
PlatformColor color
int rowIndex
```

`RowIndex` defines the logical sequence step. It does not have to be identical to the platform’s exact Y position in advanced layouts.

### 7.4 Row Progression Rule

Current gameplay rule:

```text
Row < CurrentStepIndex  → ignore
Row == CurrentStepIndex → validate platform color
Row > CurrentStepIndex  → wrong landing / lose life
```

This rule must be preserved unless explicitly redesigned.

### 7.5 Correctness Validation

When landing on the current expected row, the game validates:

```text
landed platform color == expected sequence color
```

Correct landing:

- Add score
- Advance sequence index
- Continue upward

Wrong landing:

- Lose one life
- Reset sequence progress to step 0
- Respawn to start/floor
- If lives reach 0, game over

## 8. Controls

### Desktop

```text
A / D or Left / Right = move
Space = jump
Shift or J = use equipped active skill
Esc = pause
```

### Mobile

```text
Left virtual button = move left
Right virtual button = move right
Jump virtual button = jump
Active skill button = use equipped active skill later
```

Gameplay code must read through the input abstraction. Gameplay scripts must not directly depend on keyboard-only input.

## 9. MVP Scope

### MVP Must Include

- Boot / MainMenu / Game scenes
- Input abstraction
- Rigidbody2D player controller
- Cinemachine follow camera
- Platform identity and one-way collision
- Landing detection
- Memory sequence system
- LevelConfig-driven levels
- Platform row generation
- Correct/wrong landing validation
- Lives
- Score
- Respawn
- Level complete / game over
- 3 MVP levels
- Basic HUD and result panels
- Retry / Main Menu / simple scene flow
- Basic audio feedback
- Basic UX feedback and tutorial hints
- Cactus obstacle
- Item-ready player stats architecture
- Advanced same-row platform layout foundation

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
- Full inventory / equipment screen
- Persistent active skill loadout save

## 10. Level Design

### MVP Levels

| Level | Sequence Length | Platform Count | Memory Time | Obstacles |
|---|---:|---:|---:|---|
| Level 1 | 4 | 2 per row | 5 sec | No cactus |
| Level 2 | 5 | 2 per row | 5 sec | Light cactus |
| Level 3 | 6 | 3 per row | 5 sec | Moderate cactus + light same-row vertical jitter |

### Difficulty Growth

Difficulty can increase by:

- Longer sequences
- More colors
- Smaller platforms
- Wider platform spacing
- Cactus obstacles
- More decoys per row
- Staggered platform Y positions within the same row
- Later: moving / disappearing / slippery platforms

## 11. Platform System

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

Default colored platforms should use:

```text
BoxCollider2D
PlatformEffector2D
BoxCollider2D.usedByEffector = true
PlatformEffector2D.useOneWay = true
PlatformEffector2D.surfaceArc = 180
```

### Advanced Platform Layouts

Levels may support staggered platforms within the same logical row.

Example:

```text
RowIndex 3:
  Red platform:   y = 11.2
  Blue platform:  y = 11.7
  Green platform: y = 10.9
```

Important rule:

```text
RowIndex defines the sequence step.
Y position does not define the sequence step.
```

This enables same-row mobility challenges without allowing sequence-row skipping.

Current Phase 19 foundation:

```text
LevelConfig._rowVerticalJitter
Level 1/2: 0
Level 3: small value such as 0.3
```

Future layout fields may include:

```text
maxPlatformYDifferenceWithinRow
minHorizontalGap
maxHorizontalGap
layoutDifficulty
```

## 12. Obstacles

### First Obstacle: Cactus

Cactus is a separate generated object with its own trigger collider. It is not part of the platform’s one-way collider.

Current rule:

```text
Touching cactus during Playing loses one life.
Cactus spawns only on distractor / incorrect platforms for now.
```

Future advanced rule:

```text
Correct platforms may become partially unsafe after landing zones exist.
```

Do not add landing zones until explicitly approved.

## 13. Equipment and Active Skill System

Items should not be random pickups scattered in levels by default.

The long-term model is:

```text
Jebby has a build / loadout.
Some items are equipment.
Some items are active skills placed into limited active skill slots.
```

### Item Categories

#### Equipment

Equipment can be passive or always equipped.

Possible future equipment slots:

```text
Boots
Cape
Badge / charm
```

Examples:

- Mobility boots
- Protective cape
- Score badge
- Memory charm

#### Active Skills

Active skills are manually triggered during gameplay.

Future model:

```text
3 active skill slots
Slot 1: Rocket Boots
Slot 2: Bubble Shield
Slot 3: Color Echo
```

Active skills may be balanced as:

```text
one-time use per level
limited charges per level
cooldown-based
```

For MVP prototypes, prefer **one use per level** unless cooldown is explicitly approved.

### Core Item Rule

Equipment and active skills must help the player solve harder movement or memory situations. They must not bypass the sequence.

Items may support:

- Same-row mobility
- Recovery from poor positioning
- Reaching wider same-row gaps
- Reaching slightly staggered same-row platforms
- Cactus avoidance
- Defensive recovery from one mistake
- Memory assistance

Items must not support:

```text
Skipping required sequence rows
Ignoring color validation
Bypassing lives / score rules
Breaking row progression
Random scene pickup dependency for core strategy
```

### Rocket Boots Design Direction

Rocket Boots should be implemented as:

```text
equipped gear / active skill
```

not as:

```text
random scene pickup
```

Rocket Boots are intended as a same-row mobility assist.

Rocket Boots should eventually help with:

- Staggered platforms within the same RowIndex
- Larger horizontal gaps within the same row
- Reaching higher/farther same-step platforms
- Avoiding cactus side hazards
- Recovery from bad positioning

Rocket Boots must not allow intentional row skipping.

Validation remains:

```text
Row > CurrentStepIndex = wrong landing / lose life
```

Recommended Rocket Boots prototype direction:

```text
equipped by default for prototype
activated by Use Item input
one use per level
short duration
small jump assist
small move speed / air-control assist
no pickup object
no inventory UI
no shop
no save data
```

Suggested future tuning:

```text
jumpMultiplier: about 1.10–1.20
moveSpeed or air-control assist: about 1.10–1.25
duration: about 5 seconds
```

### Future Active Skills

Potential future active skills:

- Rocket Boots: mobility assist for same-row layout challenges
- Bullet Time: temporary air-control / reaction-time assist
- Bubble Shield: protects from one mistake or hazard
- Color Echo: briefly reminds next/remaining colors

## 14. Scoring and Lives

Suggested MVP score:

```text
Correct landing: +10
Level complete: +50
Remaining life: +20 each
```

MVP default:

```text
startingLives = 3
```

## 15. Wardrobe / Outfit System

Wardrobe is approved as a future product direction, but **not MVP**.

Product direction:

```text
MVP: Default Jebby only
V2: Basic outfit preview or 1–2 unlockable cosmetic outfits
V3: Full Jebby’s Wardrobe system with outfit fragments
```

Outfits are cosmetic first. Do not add gameplay advantages, wardrobe UI, skins, outfit fragments, cosmetic inventory, outfit economy, or outfit-related save data until explicitly approved.

## 16. Art Direction

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

## 17. Technical Direction

Engine stack:

```text
Unity 6 LTS
URP 2D
Rigidbody2D
Unity Input System
Cinemachine 3
ScriptableObjects
UGUI / TMP for MVP UI
Config-driven architecture
```

MVP does not implement code hot update. Use config-driven architecture for values that may be changed later:

- Level configs
- Difficulty settings
- Equipment / active skill values later
- Obstacle rules
- Tutorial/localization text later
- Addressable assets later

No Lua / HybridCLR / ILRuntime for MVP.

## 18. Current Phase Direction

The next recommended gameplay phase is:

```text
Phase 20: Rocket Boots Equipped Active Skill Prototype
```

Purpose:

```text
Prototype the first active skill as character equipment, not as a random scene pickup.
```

Do not add inventory, shop, equipment UI, save data, or additional skills yet.

## 19. MVP Completion Criteria

The MVP is complete when:

- A player can launch the game.
- The player can start from Main Menu.
- The player can play 3 levels.
- A color sequence is shown and hidden.
- The player controls Jebby with real movement.
- Jebby jumps through one-way colored platforms.
- The game validates correct/wrong color order.
- The player can win, lose, retry, and return to menu.
- Basic HUD, audio, feedback, and tutorial hints exist.
- The codebase supports future obstacles, platform layouts, equipment, active skills, and wardrobe without major rewrites.
