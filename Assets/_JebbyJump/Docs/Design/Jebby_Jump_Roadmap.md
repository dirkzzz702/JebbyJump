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
P5D — Basic Audio / Settings Foundation             : complete (automated; visible Settings UI QA deferred)
P5E — Settings-from-Pause Integration               : complete (automated; visible Pause->Settings interaction deferred)
P5F — Shell Polish / Deferred QA Consolidation       : complete (PauseButton overlap fixed; visual confirmation deferred)
P6A — Analytics / Event Tracking Foundation          : complete (local debug sink only; no SDK/backend/network; provider integration deferred)
P6B — Analytics Event Review / Provider-Ready Cleanup : complete (central catalog + payload sanitization; settings_changed kept noisy/debounce-later; rank_earned stays removed; local-only)
P6C — Reward / Economy Design Spec                   : complete (design spec only; no runtime/economy code; implementation deferred to P7)
P7A — Stars-Only Mastery Reward Foundation           : complete (local Stars only: S/A=3, B=2, C=1, fail=0; never decreases; not spendable; no economy/ads/backend; Level Select stars deferred to P7B)
P7B — Level Select Stars Display                     : complete (read-only "Stars N/3" per card from StarRewardStore; no writes/analytics; no economy/ads/backend; visual confirmation deferred)
```

P6C produced a design-only reward/economy spec (no code). See
`Assets/_JebbyJump/Docs/Design/Jebby_Jump_Reward_Economy_Spec_v0.1.md`.
Near-term direction: Stars (mastery) primary, Spark Coins (earned-only soft
currency) reserved, Rainbow Gems deferred, no hard/paid currency, no loot
boxes, no forced ads, cosmetic-first spending, reward numbers are placeholders
pending P4B + analytics. Implementation maps to the existing P7 Economy
Foundation phase.

P4 balance is intentionally deferred because manual tester data is not available yet.
Current LevelConfig values and TimeRankConfig thresholds remain provisional.
L8/L9/L10 are known candidates for future retuning after real playtest data.

## Deferred Manual UI QA Backlog

Consolidated list of visible UI checks NOT yet performed. Automated
verification (compile + scaffold + PlayMode logic tests) passed for every
phase below, but the rendered interaction could not be driven headlessly.
None of these is marked complete. (This supersedes the previous scattered
per-phase "remains DEFERRED" notes.)

```text
P5B — Main Menu / Level Select visible flow  [DEFERRED / NOT VERIFIED]
  - Continue / Level Select / Settings / Quit visible and correctly stacked
  - Level Select opens
  - exactly 10 cards render
  - locked / unlocked / completed states visually clear
  - completed card shows Best / Rank
  - locked cards cannot start
  - Continue starts highest unlocked level

P5C — Pause visible flow  [DEFERRED / NOT VERIFIED]
  - Pause button appears and does not overlap important HUD
  - Pause opens panel
  - timer and gameplay visibly freeze
  - Resume / Restart / Main Menu work
  - Retry / Game Over / Level Complete flows still correct

P5D — Settings visible flow  [DEFERRED / NOT VERIFIED]
  - Main Menu Settings opens / closes
  - Music / SFX sliders and Mute toggle display correctly
  - values persist after reopen
  - no console errors

P5E — Pause Settings visible flow  [DEFERRED / NOT VERIFIED]
  - Pause -> Settings opens Settings while paused
  - Back returns to Pause panel
  - Pause hotkey while Settings is open is ignored
  - Resume unpauses only after returning from Settings

Visual polish (low priority)  [DEFERRED / NOT VERIFIED]
  - Settings panel layout / spacing
  - Main Menu button stack spacing
  - Level Select card spacing / readability

P4B — Manual playtest + balance tuning  [DEFERRED — awaiting tester data]
  - per-level clear-time feel, fairness, S/A/B/C threshold tuning
  - LevelConfig values and TimeRankConfig thresholds remain provisional
    (L8/L9/L10 are known retune candidates)
```

P5F note: the PauseButton-vs-timer overlap was addressed objectively in
P5F from the scene RectTransforms (PauseButton moved down to clear the
top-right timer band). Visual confirmation that it no longer overlaps
still belongs to the P5C deferred list above.

## Open Decisions

```text
Timer start: after memory phase or level start?
Rank format: S/A/B/C or stars?
Revive ad behavior: continue from current row or restart row 0?
Backend: anonymous cloud save first or account login?
Vendor choices: ads / analytics / backend?
Gem spending: cosmetics only or cosmetics + convenience?
```
