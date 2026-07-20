# Jebby Jump — Full Production Plan v1.0

> NOTE: superseded by `Jebby_Jump_Full_Production_Plan_v1.0.md` (same folder)
> — phase status and all P5+ updates are maintained there, not here.

## Confirmed Product Direction

Jebby Jump is now moving from MVP to full production.

Primary platform:

```text
Mobile first
Desktop second
```

Production target:

```text
A polished free-to-play mobile memory-platformer with 50 launch levels, time ranking, equipped skills, cosmetic wardrobe, rewarded ads, Rainbow Gems, backend, analytics, and cloud/account capability.
```

---

## Confirmed Core Decisions

### Monetization

Confirmed:

```text
Rewarded ads + Rainbow Gems + cosmetic monetization + optional convenience purchases
```

Avoid at launch:

```text
forced ads
hard stamina wall
pay-to-win wardrobe
aggressive monetization
```

Recommended monetization:

```text
rewarded ad revive
rewarded ad double gems
daily bonus ad
Rainbow Gem packs
starter pack
no-ads/supporter pack
cosmetic outfit bundles
seasonal wardrobe packs
```

### Audience

Confirmed:

```text
Parent-child friendly casual mobile game
Secondary audience: general casual mobile players
```

### Game Mode

Confirmed:

```text
Level-based map first
Endless tower later
```

### Stamina / 体力

Confirmed:

```text
No hard stamina wall at launch
Possible soft stamina / event tickets later
```

### Skill Slots

Confirmed:

```text
Start with 1 active skill slot
Expand to 3 active skill slots later
```

### Wardrobe vs Gameplay Equipment

Confirmed:

```text
Wardrobe / outfits are cosmetic only.
Gameplay skills/equipment are separate.
```

### Performance Metric

Confirmed:

```text
Remove score as the main metric.
Use time ranking.
```

Recommended:

```text
Current Time
Best Time
Time Rank
```

Suggested rank model:

```text
S Rank — excellent clear time
A Rank — good clear time
B Rank — normal clear time
C Rank — completed slowly
```

Optional:

```text
Use 1–3 stars visually, but calculate from time rank.
```

The source of truth should be time, not score.

### Launch Content Size

Confirmed:

```text
50 production levels
```

Suggested split:

```text
World 1 — Rainbow Garden: 15 levels
World 2 — Cloud Valley: 15 levels
World 3 — Cactus Desert: 20 levels
```

### Backend / Analytics / Ads

Confirmed:

```text
Backend
Cloud/account capability
Analytics
Ads
```

Important:

```text
Use service abstractions.
Do not let gameplay code directly depend on vendor SDKs.
Choose vendors in a dedicated integration phase.
```

---

## Production Phase Roadmap

### P1 — Current Screen Production Polish

Fix current UI/presentation issues before adding more content.

Includes:

```text
replace transparent mobile buttons
fix background full-screen coverage
make level/time/skill HUD readable
move tutorial messages away from center
polish start/origin platform
fix Jebby standing position
fix Jebby animation transitions
polish result panels
add mobile skill button
decide fall-off behavior
```

### P2 — Replace Score With Time Ranking

Add:

```text
LevelTimer
BestTime per level
TimeRankConfig
result panel time display
best time display
S/A/B/C rank
```

Recommended timer rule:

```text
Timer starts when Playing phase begins after the memory sequence.
```

### P3 — 100-Level / 10-World Data Foundation

Add:

```text
WorldConfig
LevelSetConfig
LevelConfig expansion
rank thresholds per level
world metadata
level unlock data model
```

### P4 — Mobile-first UI Shell

Add:

```text
polished main menu
level map
world selection
pause menu
settings
safe area support
mobile controls
result screen
skill button
```

### P5 — Equipped Skill Foundation v2

Add:

```text
ActiveSkillSlot model
SkillDefinition ScriptableObject
EquipmentSkillDefinition
ConsumableSkillDefinition
Cooldown system
skill availability state
skill HUD
mobile skill button support
```

### P6 — Skill Content

Initial skills:

```text
Rocket Boots — equipment skill
Bubble Shield — defensive skill
Color Echo — memory assist
Health Potion — consumable skill
```

### P7 — Economy Foundation

Add:

```text
Rainbow Gems
level rewards
rank-based reward bonuses
daily reward
rewarded ad reward hooks
purchase pack definitions
local wallet first
backend sync later
```

### P8 — Cosmetic Wardrobe Foundation

Add:

```text
OutfitDefinition
wardrobe screen
outfit preview
unlock state
fragment collection model
equip cosmetic outfit
```

### P9 — Backend / Analytics / Ads Integration

Add service abstractions for:

```text
analytics
ads
cloud save / account
remote config
crash reporting
```

Potential analytics events:

```text
level_start
level_complete
level_fail
retry
skill_used
rank_achieved
ad_reward_started
ad_reward_completed
purchase_started
purchase_completed
outfit_unlocked
```

### P10 — Full Art Production

Add:

```text
Jebby full animation set
platform variants
cactus variants
backgrounds per world
skill icons
wardrobe icons
mobile buttons
result panels
level map art
main menu key art
```

### P11 — Soft Launch

Scope:

```text
20–30 polished levels
analytics enabled
ads optional/rewarded only
small tester group
device performance checks
difficulty tuning
```

### P12 — Production Launch v1

Launch target:

```text
100 levels
10 themed worlds
time ranking
rewarded ads
Rainbow Gems
basic wardrobe
several active skills if ready
cloud save / analytics / crash reporting
polished mobile UI
desktop support
```

---

## Open Decisions Before Implementation

```text
1. Rank format: S/A/B/C only, or S/A/B/C plus 1–3 stars?
2. Rewarded ad revive: continue from current row, or restart from row 0?
3. Backend: anonymous cloud save first, or login/account first?
4. Gem spending: cosmetics only, or cosmetics + convenience?
5. Which vendors for analytics / ads / backend?
6. Should time rank require no life loss for S rank?
7. Should best time record only count successful no-revive runs?
```
