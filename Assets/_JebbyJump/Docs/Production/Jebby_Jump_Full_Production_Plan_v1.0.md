# Jebby Jump — Full Production Plan v1.0

## Confirmed Product Direction

Jebby Jump is moving from MVP to full production.

Primary platform:

```text
Mobile first
Desktop second
```

Production target:

```text
A polished free-to-play mobile memory-platformer with 50 launch levels, time ranking, equipped skills, cosmetic wardrobe, rewarded ads, Rainbow Gems, backend, analytics, and cloud/account capability.
```

## Confirmed Core Decisions

### Monetization

```text
Rewarded ads + Rainbow Gems + cosmetic monetization + optional convenience purchases
```

Avoid at launch:
- forced ads
- hard stamina wall
- pay-to-win wardrobe
- aggressive monetization

Recommended monetization:
- rewarded ad revive
- rewarded ad double gems
- daily bonus ad
- Rainbow Gem packs
- starter pack
- no-ads/supporter pack
- cosmetic outfit bundles
- seasonal wardrobe packs

### Audience

```text
Parent-child friendly casual mobile game
Secondary audience: general casual mobile players
```

### Game Mode

```text
Level-based map first
Endless tower later
```

### Stamina / Energy

```text
No hard stamina wall at launch
Possible soft stamina / event tickets later
```

### Skill Slots

```text
Start with 1 active skill slot
Expand to 3 active skill slots later
```

### Wardrobe vs Gameplay Equipment

```text
Wardrobe / outfits are cosmetic only.
Gameplay skills/equipment are separate.
```

### Performance Metric

```text
Remove score as the main metric.
Use time ranking.
```

Recommended:
- Current Time
- Best Time
- Time Rank

Suggested rank model:
- S Rank — excellent clear time
- A Rank — good clear time
- B Rank — normal clear time
- C Rank — completed slowly

Optional:
- Use 1–3 stars visually, but calculate from time rank.

### Launch Content Size

```text
50 production levels
```

Suggested split:
- World 1 — Rainbow Garden: 15 levels
- World 2 — Cloud Valley: 15 levels
- World 3 — Cactus Desert: 20 levels

### Backend / Analytics / Ads

Confirmed:
- Backend
- Cloud/account capability
- Analytics
- Ads

Use service abstractions. Gameplay code should not directly depend on vendor SDKs.

## Production Phase Roadmap

### P1 — Current Screen Production Polish

Fix current UI/presentation issues before adding more content:
- mobile buttons
- background full-screen coverage
- level/time/skill HUD readability
- tutorial messages away from center
- start/origin platform
- Jebby standing position
- Jebby animation transitions
- result panels
- mobile skill button
- fall-off behavior

### P2 — Replace Score With Time Ranking

Add:
- LevelTimer
- BestTime per level
- TimeRankConfig
- result panel time display
- best time display
- S/A/B/C rank

Recommended:
```text
Timer starts when Playing phase begins after the memory sequence.
```

### P3 — 50-Level Data Foundation

Add:
- WorldConfig
- LevelSetConfig
- LevelConfig expansion
- rank thresholds per level
- world metadata
- level unlock data model

### P4 — Mobile-first UI Shell

Add:
- polished main menu
- level map
- world selection
- pause menu
- settings
- safe area support
- mobile controls
- result screen
- skill button

### P5 — Equipped Skill Foundation v2

Add:
- ActiveSkillSlot model
- SkillDefinition ScriptableObject
- EquipmentSkillDefinition
- ConsumableSkillDefinition
- Cooldown system
- skill availability state
- skill HUD
- mobile skill button support

### P6 — Skill Content

Initial skills:
- Rocket Boots — equipment skill
- Bubble Shield — defensive skill
- Color Echo — memory assist
- Health Potion — consumable skill

### P7 — Economy Foundation

Add:
- Rainbow Gems
- level rewards
- rank-based reward bonuses
- daily reward
- rewarded ad reward hooks
- purchase pack definitions
- local wallet first
- backend sync later

### P8 — Cosmetic Wardrobe Foundation

Add:
- OutfitDefinition
- wardrobe screen
- outfit preview
- unlock state
- fragment collection model
- equip cosmetic outfit

### P9 — Backend / Analytics / Ads Integration

Add service abstractions for:
- analytics
- ads
- cloud save / account
- remote config
- crash reporting

Potential analytics events:
- level_start
- level_complete
- level_fail
- retry
- skill_used
- rank_achieved
- ad_reward_started
- ad_reward_completed
- purchase_started
- purchase_completed
- outfit_unlocked

### P10 — Full Art Production

Add:
- Jebby full animation set
- platform variants
- cactus variants
- backgrounds per world
- skill icons
- wardrobe icons
- mobile buttons
- result panels
- level map art
- main menu key art
- **origin/start floor art** — full-width cloud-floor or magical strip sprite,
  fixed scene object, visually distinct from colored sequence platforms.
  Target path: `Assets/_JebbyJump/Art/Sprites/World/spr_origin_floor_01.png`.
  Until supplied, the origin floor stays as a solid warm-beige rectangle
  collider; gameplay behavior unchanged.

### P11 — Soft Launch

Scope:
- 20–30 polished levels
- analytics enabled
- ads optional/rewarded only
- small tester group
- device performance checks
- difficulty tuning

### P12 — Production Launch v1

Launch target:
- 50 levels
- 3 worlds
- time ranking
- rewarded ads
- Rainbow Gems
- basic wardrobe
- several active skills if ready
- cloud save / analytics / crash reporting
- polished mobile UI
- desktop support

## Production Phase Status

| Phase | Description                                      | Status                                                    |
|-------|--------------------------------------------------|-----------------------------------------------------------|
| P1    | Current Screen Production Polish                 | complete                                                  |
| P2    | Time Ranking System                              | complete                                                  |
| P3    | 10-Level Data Foundation (vertical slice)        | complete (commit 42ee80f)                                 |
| P4A   | Readiness / Analytical Verification              | complete (accepted as read-only verification)             |
| P4B   | Manual Playtest + Balance Tuning                 | deferred (awaiting tester data)                           |
| P5A   | Level Select / Local Progression Foundation      | complete                                                  |
| P5B   | Continue Flow + Level Select UX Polish           | complete (automated; manual UI smoke deferred)            |
| P5C   | Pause Menu / In-Game Flow Polish                 | complete (automated; visible pause interaction deferred)  |
| P5D   | Basic Audio / Settings Foundation                | complete (automated; visible Settings UI QA deferred)     |
| P5E   | Settings-from-Pause Integration                  | complete (automated; visible Pause->Settings deferred)    |
| P5F   | Shell Polish / Deferred QA Consolidation         | complete (PauseButton overlap fixed; visual QA deferred)  |
| P6A   | Analytics / Event Tracking Foundation            | complete (local debug sink only; no SDK/backend/network)  |

P4 balance is intentionally deferred because manual tester data is not available yet.
Current LevelConfig values and TimeRankConfig thresholds remain provisional.
L8/L9/L10 are known candidates for future retuning after real playtest data.

## Deferred Manual UI QA Backlog

Consolidated list of visible UI checks NOT yet performed. Automated
verification (compile + scaffold + PlayMode logic tests) passed for every
phase below, but the rendered interaction could not be driven headlessly.
None of these is marked complete. (Supersedes the previous scattered
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
top-right timer band). Visual confirmation belongs to the P5C list above.

## P6A — Analytics / Event Tracking Foundation

Status: implemented. Scope: **local debug sink only** (`DebugAnalyticsSink`
logs to the Unity console + keeps a small in-memory ring buffer). No
third-party SDK, no backend, no HTTP/network, no PII. Deferred: real
analytics provider integration (a future `IAnalyticsSink` implementation
plugs in with zero gameplay change).

Architecture: `JebbyJump.Analytics.Runtime` asmdef — `AnalyticsService`
(static facade: `Enabled`, `SetSink`, `Track`), `IAnalyticsSink`,
`AnalyticsEvent`/`AnalyticsParam`, `DebugAnalyticsSink`. Session id is a
process-lifetime GUID (not persisted, not PII). `app_session_started` is
self-emitted once per process on the first `Track` (scene-independent — no
BootController dependency). Level context for emitters without a
`LevelSessionController` reference comes from the read-only
`JebbyJump.Session.LevelContext` mirror.

Event catalog (all snake_case; values are string/int/float/bool only):

| Event | Trigger (single source) | Key parameters | Notes |
|---|---|---|---|
| `app_session_started` | first `Track` of the process (AnalyticsService) | session_id | once per session; lazy |
| `main_menu_continue_clicked` | `MainMenuController.OnContinueClicked` | target_level_index, target_level_number | |
| `main_menu_level_select_clicked` | `MainMenuController.OnStartClicked` | — | |
| `main_menu_settings_opened` | `MainMenuController.OnSettingsClicked` | — | |
| `level_select_opened` | `LevelSelectController.Open` | — | |
| `level_selected` | `LevelSelectController.OnCardClicked` | level_index, level_number, is_replay, has_best_time | is_replay == has_best_time |
| `level_started` | `MemoryPhaseController.RunMemoryPhase` (entry) | level_index, level_number, source | source: continue/level_select/retry/next_level/default |
| `memory_phase_started` | same (entry) | level_index, level_number, sequence_length | |
| `gameplay_started` | `MemoryPhaseController` (Playing phase begins) | level_index, level_number | |
| `level_completed` | `HUDController.PopulateTimeRank` | level_index, level_number, elapsed_time, rank, is_new_best | rank included here |
| `best_time_improved` | same | level_index, level_number, old_best_time, new_best_time, improvement_seconds | only when prior best existed and improved |
| `level_failed` | `MemoryPhaseController.OnGameOver` | level_index, level_number, reason | reason: lives_depleted |
| `player_damaged` | `MemoryPhaseController` damage sites | level_index, level_number, remaining_lives, source | source: wrong_color/hazard; only on real (non-shielded) hits |
| `skill_used` | `ActiveSkillController.TryUseSkill` (at activation) | skill_type, level_index, level_number | never when paused/cooldown-blocked |
| `pause_opened` / `pause_resumed` / `pause_restart_clicked` / `pause_main_menu_clicked` / `pause_settings_opened` | `PauseMenuController` | level_index, level_number | |
| `settings_changed` | `SettingsPanelController` committed handlers | setting_name, value | setting_name: music_volume/sfx_volume/muted/reset_defaults; suppressed during panel init; slider drags are debug-noisy |

Intentionally **omitted**: `rank_earned` (rank is already carried by
`level_completed`, so a separate event would duplicate rank data);
`level_retried` (folded into `level_started` with `source=retry`).

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
