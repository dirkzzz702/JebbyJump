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
P7C — Reward UI Copy / Visual QA Checklist           : complete (docs/checklist only; no runtime/code/scene changes; manual visual QA still deferred)
P7D — Reward UI Copy Consistency Polish              : complete (Stars UI copy standardized to "Stars: N/3" via single StarRewardFormatter; Level Select changed "Stars N/3" -> "Stars: N/3"; no scene/prefab/economy changes; manual visual QA still deferred)
P7E — Reward Foundation Closure / Regression Guardrails : complete (Stars reward foundation closed; automated coverage reviewed; reward analytics wire names pinned by test; no economy/ads/backend; manual visual QA still deferred/NOT VERIFIED)
P8  — Cosmetic Wardrobe Design Spec                  : complete (design spec only; no runtime/wardrobe code; outfits-first, Stars-gated unlocks recommended with PLACEHOLDER thresholds; no shop/Spark Coins/Rainbow Gems currency/ads/backend; implementation deferred to future P9A)
P9  — Wardrobe Foundation (local, cosmetic-only)     : complete (WardrobeCatalog 5 outfits; WardrobeStore equipped-id only; pure Stars-gated WardrobeUnlockService; text-only Wardrobe panel from Main Menu; Stars gate but are NOT consumed; thresholds 0/8/15/22/30 are PLACEHOLDERS; no art/sprite swap, no shop/Spark Coins/Rainbow Gems currency/ads/backend; outfits have no gameplay effect; visual QA DEFERRED/NOT VERIFIED)
P10 — Wardrobe Visual QA / Art Readiness Plan        : complete (docs/checklist only; manual visual QA still DEFERRED/NOT VERIFIED; no art/sprite-swap; no shop/Spark Coins/Rainbow Gems currency/ads/backend)
P11 — Wardrobe Visual Application Technical Foundation : complete (new JebbyJump.Wardrobe.Visual asmdef: OutfitVisualDefinition + OutfitVisualCatalog resolver + PlayerOutfitVisualController wired onto Jebby.prefab, Game.unity untouched; equipped outfit applied on Start; every outfit HasVisualOverride=false no-op so Jebby is visually unchanged until art exists; default maps to current visuals; 9 new PlayMode tests, 76/76 pass; no art/sprite/anim assets; no gameplay/rank/progression/economy changes; no shop/Spark Coins/Rainbow Gems currency/ads/backend; manual visual QA DEFERRED/NOT VERIFIED)
P12 — First Outfit Art Asset Request Pack / Visual Pipeline Readiness : complete (docs + minimal test seam; Forest Cavalier first-outfit art request pack; OutfitVisualApplier pure-static apply seam + 4 override-assignment tests, 80/80 pass; catalog stays no-op for all outfits; no art/sprite/anim/controller assets; AnimatorOverrideController over JebbyAnimator recommended for first art; no gameplay/economy changes; manual visual QA DEFERRED/NOT VERIFIED)
```

P12 produced the First Outfit Art Asset Request Pack for Forest Cavalier
(`Assets/_JebbyJump/Docs/Art/Jebby_Jump_First_Outfit_Art_Asset_Request_Pack_v0.1.md`):
the selected first outfit (Art Bible #1), identity guardrails, the actual 7
animation states, actual sprite import settings (PPU 100, pivot (0.5,0),
Bilinear), naming/folder conventions, and the AnimatorOverrideController
pipeline (base `JebbyAnimator`). It also added a pure-static
`OutfitVisualApplier` seam (single source of truth for the apply rule; behavior
unchanged) + 4 override-assignment tests (80/80 pass) using an in-memory
controller. No final art is imported; the catalog stays no-op for all outfits,
so Jebby is visually unchanged. No gameplay/economy changes; manual visual QA
stays DEFERRED / NOT VERIFIED. Preferred next phase: P13A generate/request
Forest Cavalier art.

P11 implemented the wardrobe visual application technical foundation: a new
`JebbyJump.Wardrobe.Visual` asmdef (`OutfitVisualDefinition`,
`OutfitVisualCatalog` resolver, `PlayerOutfitVisualController`). The controller
is wired onto `Jebby.prefab` (the Game scene inherits it; Game.unity is not
edited) and, on Start, applies the equipped outfit id from `WardrobeStore`.
Every outfit currently resolves to a no-op (`HasVisualOverride=false`) because
no art exists yet, so Jebby is visually unchanged; the default outfit maps to
the current visuals. The path is the future seam for an
`AnimatorOverrideController` / sprite-sheet. 9 new PlayMode tests (76/76 pass);
no art/sprite/anim assets; no gameplay/economy changes; manual visual QA stays
DEFERRED / NOT VERIFIED.

P10 added a manual visual-QA checklist for the P9 Wardrobe panel plus an
art-production + sprite-swap readiness plan:
`Assets/_JebbyJump/Docs/QA/Jebby_Jump_Wardrobe_Visual_QA_and_Art_Readiness_Plan_v0.1.md`.
Docs-only; records actual P9 UI copy and the real PlayerAnimator contract
(states idle/run/jump/fall/land/hurt/victory) for future art. Manual visual
QA stays DEFERRED / NOT VERIFIED. Preferred next phase: P11A Wardrobe UI Polish
(if QA finds issues) or P11B Art Asset Production Spec.

P8 produced a design-only cosmetic wardrobe spec (no code). See
`Assets/_JebbyJump/Docs/Design/Jebby_Jump_Cosmetic_Wardrobe_Spec_v0.1.md`.
Cosmetic-first, child-safe; preserves Jebby's identity (Art Bible Design Lock
Rule). Recommended first wardrobe implementation after P8 / future P9A:
Stars-gated direct outfit unlocks (Stars gate but are NOT consumed; thresholds
are PLACEHOLDERS pending P4B + level count + balance review). The rainbow-gem
visual MOTIF stays as identity/art; the Rainbow Gems CURRENCY remains deferred
and unimplemented.

The Stars reward foundation (P7A-P7E) is **closed for now**. Automated
coverage (calculator, store, formatter, analytics catalog + pinned reward
wire names) is in place; manual visual QA remains DEFERRED / NOT VERIFIED.
Preferred next phase: **P8A Cosmetic Wardrobe Design Spec (docs-only)** -
cosmetic-first, no shop/ads/backend yet.

P7C added a manual visual-QA + copy checklist for the Stars UI:
`Assets/_JebbyJump/Docs/QA/Jebby_Jump_Reward_UI_Visual_QA_Checklist_v0.1.md`.
It documents the result-panel vs Level Select copy difference
("Stars: N/3" vs "Stars N/3") without changing it, and preserves all
deferred manual visual QA (P5B-P5F, P7A, P7B) and P4B as NOT complete.

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
