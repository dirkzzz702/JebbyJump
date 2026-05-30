# Jebby Jump — Roadmap Production Update v1.0

## Confirmed Production Direction

```text
Mobile-first
Desktop second
50 levels
Level-based map first
Endless later
Rewarded ads + Rainbow Gems + cosmetics
No hard stamina wall at launch
Backend + analytics + ads
Time ranking instead of score
Wardrobe cosmetic only
Equipment skills and consumable skills
```

## Major Roadmap Changes

### Replace Score With Time

Old:

```text
Score-based progress
```

New:

```text
clear time
best time
time rank
S/A/B/C or star equivalent
```

### Monetization

Launch model:

```text
rewarded ads
gems
cosmetics
starter pack
optional convenience
```

Later:

```text
soft stamina / event tickets
seasonal cosmetics
event levels
```

### Skill System

Two types:

```text
Equipment Skill
Consumable Skill Item
```

Both:

```text
equipped into active skill slots
cooldown-based
not random scene pickups by default
```

### Content Target

Production v1:

```text
50 levels
3 worlds
```

## Updated Production Phase Order

### P1 — Current Screen Production Polish

Fix current UI/presentation issues:
- mobile buttons
- HUD readability
- background coverage
- start platform/floor
- Jebby spawn/animation
- tutorial placement
- result panels
- mobile skill button
- camera/background bounds

### P2 — Time Ranking System

Replace score with timer/best time/rank.

### P3 — 50-Level Data Foundation

WorldConfig, LevelSetConfig, LevelConfig expansion, rank thresholds per level.

### P4 — Mobile-first UI Shell

Level map, polished menus, safe area, mobile controls, settings, pause.

### P5 — Equipped Skill Foundation v2

Skill slots, skill definitions, cooldown model.

### P6 — Skill Content

Rocket Boots, Bubble Shield, Color Echo, Health Potion.

### P7 — Economy Foundation

Rainbow Gems, rewards, rewarded ad hooks.

### P8 — Cosmetic Wardrobe Foundation

Outfit definitions, unlocks, preview, equip.

### P9 — Backend / Analytics / Ads Integration

Cloud/account, analytics, ads abstraction.

### P10 — Full Art Production

Final sprites, UI, worlds, icons, animations.

### P11 — Soft Launch

Limited test, analytics, tuning.

### P12 — Production Launch v1

50 levels, monetization, wardrobe, services, mobile polish.

## Current Production Phase Status

```text
P1 — Current Screen Production Polish              : complete
P2 — Time Ranking System                           : complete
P3 — 10-Level Data Foundation (vertical slice)     : complete (commit 42ee80f)
P4A — Readiness / Analytical Verification          : complete (read-only verification, accepted)
P4B — Manual Playtest + Balance Tuning             : deferred (awaiting tester data)
P5A — Level Select / Local Progression Foundation  : complete
P5B — Continue Flow + Level Select UX Polish        : complete (automated; manual UI smoke deferred)
P5C — Pause Menu / In-Game Flow Polish              : complete (automated; visible pause interaction deferred)
P5D — Basic Audio / Settings Foundation             : in progress
```

P4 balance is intentionally deferred because manual tester data is not available yet.
Current LevelConfig values and TimeRankConfig thresholds remain provisional.
L8/L9/L10 are known candidates for future retuning after real playtest data.

P5B manual rendered UI smoke test remains DEFERRED. Automated compile and
PlayMode logic tests passed, but the visible Main Menu / Level Select
interaction (panel opens, 10 cards render, lock/unlock/completed visuals,
Continue jumps to the right level) still requires manual QA later. It is
NOT marked completed.

P5C visible pause interaction remains DEFERRED. Automated compile, scaffold,
and PlayMode logic tests passed, but the visible in-game pause flow
(Pause button opens panel, gameplay freezes, Resume/Restart/Main Menu
behave correctly, timeScale restored) still requires manual QA later.
It is NOT marked completed.

## Open Decisions

```text
Timer start: after memory phase or level start?
Rank format: S/A/B/C or stars?
Revive ad behavior: continue from current row or restart row 0?
Backend: anonymous cloud save first or account login?
Vendor choices: ads / analytics / backend?
Gem spending: cosmetics only or cosmetics + convenience?
```
