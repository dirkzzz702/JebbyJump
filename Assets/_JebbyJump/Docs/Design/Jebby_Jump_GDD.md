# Jebby Jump｜Game Design Document v1.0

## 1. Overview

**Game Name:** Jebby Jump  
**Genre:** 2D memory platformer / casual action puzzle  
**Primary Platform:** Mobile  
**Secondary Platform:** Desktop  
**Engine:** Unity 6 LTS  
**Render Pipeline:** URP 2D  
**Core Character:** Jebby the Color Knight

## 2. One-line Pitch

Jebby Jump is a colorful mobile-first memory platformer where players memorize a color sequence, then guide Jebby upward through colored platforms in the correct order using real platformer controls, equipped skills, and quick decision-making.

## 3. Core Design Statement

```text
Memory tells the player where to go.
Skill decides whether they can get there.
Equipped skills help the player recover, reposition, or solve harder layouts.
Equipped skills must not bypass the memory sequence.
Time ranking rewards clean, fast execution.
```

## 4. Target Product Direction

Confirmed production direction:

```text
Mobile-first
Desktop second
50 launch levels
Level-based map first
Endless tower later
Rewarded ads + Rainbow Gems + cosmetics
No hard stamina wall at launch
Backend + analytics + ads
Time ranking instead of score
Wardrobe cosmetic only
Equipment skills + consumable skills
```

## 5. Character Direction

Jebby is not a literal dog. Jebby is a Cavalier King Charles Spaniel-inspired humanoid fantasy color knight.

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

## 6. Core Gameplay Loop

1. Player selects a level.
2. Game shows a color sequence.
3. Player memorizes the sequence.
4. Sequence is hidden.
5. Timer starts when the Playing phase begins.
6. Player controls Jebby with real platformer controls.
7. Jebby jumps upward through colored platforms.
8. Landing on the correct color at the current expected row advances the sequence.
9. Landing on the wrong color at the current expected row costs a life.
10. Landing on a future row before the expected step costs a life.
11. Landing on a completed/lower row is ignored.
12. Player may use equipped skills at the right moment.
13. Completing the full sequence clears the level.
14. Result screen shows clear time, best time, and rank.
15. Losing all lives triggers game over.

## 7. Row Progression Rule

```text
Row < CurrentStepIndex  → ignore
Row == CurrentStepIndex → validate platform color
Row > CurrentStepIndex  → wrong landing / lose life
```

This rule must be preserved unless explicitly redesigned.

## 8. Time Ranking System

Replace score with time ranking.

Track:
- Current clear time
- Best clear time per level
- Time rank

Recommended rank model:
- S Rank — excellent clear time
- A Rank — good clear time
- B Rank — normal clear time
- C Rank — completed slowly

Timer starts when the Playing phase begins after the memory sequence.

Result screen should show:
- Clear Time
- Best Time
- Rank

## 9. Controls

Desktop:
```text
A / D or Left / Right = move
Space = jump
Shift or J = use equipped skill
Esc = pause
```

Mobile:
```text
Left virtual button = move left
Right virtual button = move right
Jump virtual button = jump
Equipped skill button = use equipped skill
```

Mobile is primary.

## 10. Level Design

Production launch target:
```text
100 levels
10 themed worlds (10 levels each)
```

World roster (level 10 of each world is a finale):
```text
World 01 — Cloud Meadow:          levels 001-010
World 02 — Enchanted Forest:      levels 011-020
World 03 — Crystal Caves:         levels 021-030
World 04 — Sunshine Desert:       levels 031-040
World 05 — Ocean Sky:             levels 041-050
World 06 — Candy Cloud Kingdom:   levels 051-060
World 07 — Clockwork Heights:     levels 061-070
World 08 — Moonlit Dreamscape:    levels 071-080
World 09 — Stormfire Volcano:     levels 081-090
World 10 — Rainbow Tower Castle:  levels 091-100
```

Authoritative design source:
`Assets/_JebbyJump/Docs/Design/WorldExpansion100/`
(per-level values in `13_100_LEVEL_MASTER_TABLE.csv`).
Existing Levels 1-10 are preserved unchanged as World 1.

Difficulty can increase through:
- longer sequences
- more colors
- smaller platforms
- wider spacing
- cactus obstacles
- more decoys
- same-row Y stagger
- horizontal platform width variation

## 11. Platform System

Each platform has:
```text
PlatformColor color
int rowIndex
```

RowIndex defines the sequence step. Y position does not define the sequence step.

Default colored platforms are one-way platforms.

Platform width can vary horizontally. Gameplay collider height/thickness should stay fixed unless explicitly approved.

## 12. Obstacles

Cactus is a separate generated object with its own trigger collider.

Current rule:
```text
Touching cactus during Playing loses one life.
Cactus spawns only on distractor / incorrect platforms for now.
```

## 13. Equipped Skill System

Items should not be random pickups scattered in levels by default.

The long-term model:
```text
Jebby has a build/loadout.
The player equips skills before a level.
Equipped skills occupy active skill slots.
Start: 1 active skill slot.
Later: 3 active skill slots.
```

Skill types:

```text
1. Equipment Skill
   - granted by gear
   - cooldown-based
   - reusable
   - example: Rocket Boots

2. Consumable Skill Item
   - equipped into skill slot
   - limited use / one-time use
   - cooldown protected
   - example: Health Potion
```

Rocket Boots should be equipment-granted active skill, not a random pickup.

Rocket Boots may assist:
- same-row mobility
- recovery from poor positioning
- wider same-row gaps
- staggered same-row platforms
- cactus avoidance

Rocket Boots must not:
- bypass row validation
- allow intentional row skipping
- ignore wrong-color validation
- function as a shortcut around memory sequence

## 14. Economy and Monetization

Confirmed model:
```text
Free-to-play
Rewarded ads
Rainbow Gems
Cosmetics
Optional convenience purchases
```

Avoid at launch:
- forced ads
- hard stamina wall
- pay-to-win wardrobe
- aggressive monetization

## 15. Wardrobe

Wardrobe/outfits are cosmetic only.

Initial outfit candidates:
- Classic Cavalier
- Forest Cavalier
- Sunshine Knight
- Aqua Knight
- Silver Dreamer

Do not add gameplay stats to outfits unless explicitly redesigned.

## 16. Backend, Analytics, and Ads

Production direction includes:
- backend
- cloud/account capability
- analytics
- ads

Use service abstractions. Gameplay code must not directly depend on vendor SDKs.

## 17. Art Direction

Art direction is defined in:
```text
Assets/_JebbyJump/Docs/Art/Jebby_Jump_Art_Bible.md
```

Style:
- bright
- magical
- friendly
- child-safe
- soft fantasy
- premium indie
- clear gameplay readability

## 18. Current Production Priorities

1. Current screen production polish.
2. Replace score with time ranking.
3. Build scalable 100-level / 10-world structure (see WorldExpansion100).
4. Build production mobile UI shell.
5. Expand skill/economy/wardrobe/backend in planned phases.
