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
| P6B   | Analytics Event Review / Provider-Ready Cleanup  | complete (central catalog + payload sanitization; local-only) |
| P6C   | Reward / Economy Design Spec                     | complete (design spec only; no runtime/economy code)      |
| P7A   | Stars-Only Mastery Reward Foundation             | complete (local Stars only; no Spark Coins/Gems/shop/ads) |
| P7B   | Level Select Stars Display                       | complete (read-only "Stars N/3" per card; no economy/ads) |
| P7C   | Reward UI Copy / Visual QA Checklist             | complete (docs/checklist only; manual visual QA deferred) |
| P7D   | Reward UI Copy Consistency Polish                | complete (Stars UI copy standardized to "Stars: N/3")     |
| P7E   | Reward Foundation Closure / Regression Guardrails | complete (coverage reviewed; reward wire names pinned)    |
| P8    | Cosmetic Wardrobe Design Spec                    | complete (design spec only; no runtime; outfits-first, Stars-gated) |
| P9    | Wardrobe Foundation (local, cosmetic-only)       | complete (catalog + equipped store + Stars-gated unlock + text panel) |
| P10   | Wardrobe Visual QA / Art Readiness Plan          | complete (docs/checklist only; manual visual QA deferred) |

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

## P6B — Analytics Event Review / Provider-Ready Cleanup

Status: implemented. Scope: provider-ready cleanup of the P6A surface.
Still **local/debug-only** — no SDK, backend, HTTP/network, packages, or PII.

What changed in P6B (no event name/key or payload values changed):
- **Central catalog** — `AnalyticsEvents` (event names) and `AnalyticsParams`
  (parameter keys) const classes in `Analytics.Runtime`. All emitter call
  sites now reference the consts instead of string literals; emitted wire
  names are identical to P6A.
- **Provider-safe payload sanitization** — `AnalyticsService.Dispatch` now
  drops any param whose value is null or not a simple primitive
  (string/int/float/bool, plus long/double), with an editor-only warning.
  Analytics never throws into gameplay; a future provider can never be
  handed a Unity object, collection, or other complex value.
- **DebugAnalyticsSink.Recent** returns a read-only snapshot copy; buffer
  stays bounded at `BufferCapacity = 64` (ring-trims oldest).
- **Provider boundary** — `IAnalyticsSink` is the integration seam (no new
  interface added). A future Firebase / GameAnalytics / custom sink
  implements `IAnalyticsSink`; `AnalyticsService.SetSink(...)` swaps it and
  **callers do not change**. No provider package added.

Decisions:
- **`settings_changed` — kept on slider `onValueChanged`** (debug-noisy by
  design; the `_initializing` guard prevents firing during populate).
  Reducing per-drag noise would require pointer/EventTrigger scene logic
  (out of scope and fragile), so the future real-provider adapter should
  debounce or commit-on-close. Mute/reset are discrete, not noisy.
- **`rank_earned` — confirmed removed.** `level_completed` already carries
  `rank`; no separate event.

Provider-readiness status by event: all 20 events are stable-named,
single-source, and emit only sanitized primitive payloads. `settings_changed`
is the only event flagged **noisy (debounce at provider)**; every other
event is **provider-ready**.

No PII collected. No network. No SDK. Real analytics provider integration
remains **deferred**. Manual UI QA backlog (P5B–P5F) and P4B balance/playtest
remain **deferred**.

## P6C — Reward / Economy Design Spec

Status: implemented as **design spec only**. No economy/monetization/ads
runtime, no code, no scene/asset/package changes. Full spec:
`Assets/_JebbyJump/Docs/Design/Jebby_Jump_Reward_Economy_Spec_v0.1.md`.

Near-term direction (placeholders pending P4B + analytics): **Stars** as the
primary per-level mastery reward; **Spark Coins** reserved as an earned-only
soft currency for cosmetics; **Rainbow Gems** (GDD launch currency) deferred;
**no hard/paid currency, no loot boxes, no forced ads, no progression-gated
ads**; cosmetic-first spending; rewards never affect rank fairness. Future
economy analytics events are documented (extending the P6A/P6B catalog via
`IAnalyticsSink`) but not implemented. Implementation maps to the existing
**P7 Economy Foundation** phase and is gated on manual playtest + analytics.

## P7A — Stars-Only Mastery Reward Foundation

Status: implemented. Scope: **local Stars only** - a per-level mastery record,
not a spendable/farmable currency. No Spark Coins, Rainbow Gems, shop,
wardrobe, ads, IAP, backend, cloud save, or paid currency.

- Rules: S/A = 3, B = 2, C = 1, completed-with-no-rank-config = 1, failure = 0.
  Stars track the **best** achieved rank per level and **never decrease** on
  replay; they do not affect progression unlocks, rank, or time.
- Persistence: `StarRewardStore` (PlayerPrefs) with per-level keys
  `jebby.rewards.levelStars.{levelIndex}` only - no aggregate key;
  `GetTotalStars(levelCount)` sums on demand (no drift). `StarRewardCalculator`
  is the pure rank→stars mapping. Both live in `JebbyJump.Rewards.Runtime`.
- UI: result panel shows `Stars: N/3` (+ "New Star Best!" on improvement) via
  a scaffolded `StarsText` (`Jebby Jump/Scaffold/Build Stars Display`,
  idempotent). Existing time/rank/best-time display unchanged.
- Analytics (local debug only, extends the P6A/P6B catalog): `reward_granted`
  and `star_total_changed`, emitted **only when a clear increases the stored
  best**. No backend/provider/network.
- Reset: `Jebby Jump/Reset/Reset Stars`, and Stars are included in
  `Reset Everything`. Reset Local Progress / Reset Best Times unchanged.
- Deferred: Spark Coins / Rainbow Gems / cosmetics / shop / ads remain
  deferred per P6C. Reward numbers stay placeholders pending P4B + analytics.

## P7B — Level Select Stars Display

Status: implemented. Scope: shows each level's **stored best Stars** as
`Stars N/3` on its Level Select card. Pure read-only display - Level Select
calls `StarRewardStore.GetStars(i)` only (no writes), emits no analytics, and
does not change unlock/best/rank/tint behavior. No Spark Coins, Rainbow Gems,
shop, wardrobe, ads, backend, or paid currency.

- `LevelSelectCard` gains a null-safe `_starsText` set via the pure
  `StarRewardFormatter.Label(stars)` (clamped 0..3). Locked/unlocked/completed
  states and the locked overlay are unchanged.
- Card prefab carries a `StarsText` line below Rank (y=-86), added by the
  idempotent `Jebby Jump/Scaffold/Build Level Select Panel`: re-running
  upgrades/re-wires the existing prefab without duplicating. No grid/panel
  redesign.
- Visual confirmation joins the deferred manual-UI-QA backlog (alongside the
  P7A result-panel Stars visual).

## P7C — Reward UI Copy / Visual QA Checklist

Status: implemented as **docs/checklist only**. No runtime/code/scene/prefab/
test/package changes. Deliverable:
`Assets/_JebbyJump/Docs/QA/Jebby_Jump_Reward_UI_Visual_QA_Checklist_v0.1.md` -
a 14-section manual visual-QA + copy checklist for the P7A/P7B Stars UI
(result panel + Level Select), with pass/fail criteria, locked/unlocked/
completed scenarios, reset/replay scenarios, and accessibility checks.

Documents (does NOT change) the copy difference: result panel `Stars: N/3`
(with colon, + "New Star Best!") vs Level Select `Stars N/3` (no colon); a
future approved polish phase may standardize it. All deferred manual visual
QA (P5B-P5F, P7A, P7B) and P4B remain deferred and are explicitly listed as
NOT complete. No star-reward semantics, analytics, or economy changes.

## P7D — Reward UI Copy Consistency Polish

Status: implemented. Copy-only. The Stars UI wording is standardized to
**`Stars: N/3`** across both surfaces via the single `StarRewardFormatter
.Label` source: the formatter now returns `Stars: N/3` (Level Select picks it
up automatically; previously `Stars N/3`), and `HUDController.GrantStars` uses
the formatter instead of building the string inline (result panel text
unchanged at `Stars: N/3`, still appending `(New Star Best!)` only on
improvement). No scene/prefab changes, no `StarRewardStore`/`StarReward
Calculator`/analytics semantic changes, no economy. Formatter tests updated to
the colon form (suite still 51). Manual visual QA of the standardized copy
remains DEFERRED / NOT VERIFIED.

## P7E - Reward Foundation Closure / Automated Regression Guardrails

Status: implemented. The Stars reward foundation (P7A-P7D) is **closed for
now**. Automated coverage was reviewed and found sufficient: `RewardsTests`
covers `StarRewardCalculator` (all rank cases), `StarRewardStore` (default,
store, never-decrease, clamp 0..3, negative no-op, total sum, non-positive
count, reset), and `StarRewardFormatter` (0..3 + clamp); `AnalyticsCatalogTests`
format-checks all event/param consts. The one gap - the reward analytics
**wire names** were only format-checked, not value-pinned - is closed by a
single new test (`RewardConstants_HaveStableWireNames`) asserting
`reward_granted` / `star_total_changed` / `reward_type` / `amount` /
`total_for_level` / `previous_for_level` / `old_total` / `new_total` / `delta`.
Suite is now 52. No code/semantic change to any `StarReward*` type, analytics
values, HUD, Level Select, scenes, or prefabs; no economy/ads/backend.
Reward emit behaviour in `HUDController.GrantStars` remains
manual/log-verified (MonoBehaviour, by design). Manual visual QA stays
DEFERRED / NOT VERIFIED. Preferred next phase: **P8A Cosmetic Wardrobe Design
Spec (docs-only)**; alternatives are a Spark Coins design spec or pausing
economy work until manual QA + P4B playtest.

## P8 - Cosmetic Wardrobe Design Spec

Status: implemented as **design spec only**. No runtime/wardrobe/inventory/
shop/currency code, no UI/scene/prefab, no art import. Full spec:
`Assets/_JebbyJump/Docs/Design/Jebby_Jump_Cosmetic_Wardrobe_Spec_v0.1.md`.

Direction: **cosmetic-only, outfits-first**, child-safe, preserving Jebby's
identity (Art Bible Design Lock Rule). Initial outfit set: Classic Color
Knight (default, always unlocked; = GDD/Art Bible "Classic Cavalier"), Forest
Cavalier, Sunshine Knight, Aqua Knight, Silver Dreamer. Recommended first
wardrobe implementation after P8 / future P9A: **Stars-gated direct outfit
unlocks** - Stars gate unlocks but are **NOT consumed**; all star thresholds
are **STRICT PLACEHOLDERS** pending P4B + final level count + reward/balance
review. Future data model, analytics events, and art requirements are
documented as proposals only.

Currency stance: no shop, no Spark Coins, no Rainbow Gems **currency**, no
paid currency in P8 or the recommended near-term wardrobe. Distinct from that:
the **rainbow-gem visual motif** remains an allowed/required part of Jebby's
identity and art. Manual visual QA (P5B-P5F, P7A, P7B) and P4B remain
DEFERRED / NOT VERIFIED. Recommended next phase: P9A Wardrobe Data Model +
Local Ownership Store (only after explicit approval).

## P9 - Wardrobe Foundation (local, cosmetic-only)

Status: implemented. First local wardrobe foundation per the P8 spec.
**Cosmetic-only, offline/local, no art swap yet.** No shop, Spark Coins,
Rainbow Gems currency, ads, IAP, or backend.

- `JebbyJump.Wardrobe.Runtime` (new asmdef): `CosmeticItemDefinition` +
  `WardrobeCatalog` (exactly the 5 P8 outfits; ids snake_case; thresholds
  0/8/15/22/30 stored as **PLACEHOLDER** data); `WardrobeStore` (PlayerPrefs
  key `jebby.wardrobe.equippedOutfit` only - ownership derived from Stars,
  not persisted; unknown/null id -> `classic_color_knight`); pure
  `WardrobeUnlockService` (Stars-gated; `IsUnlocked`/`GetState`/
  `NormalizeEquippedId`; **Stars are NOT consumed**, no PlayerPrefs writes,
  no `StarRewardStore` coupling - total stars passed in).
- UI: `WardrobePanelController` - text-only outfit rows with Locked /
  Unlocked / Equipped state, a preview + state label, Equip (disabled for
  locked or already-equipped) and Back. Opened from a new Main Menu
  **Wardrobe** button (stack: Continue / Level Select / Settings / Wardrobe
  / Quit). Total stars read via `StarRewardStore.GetTotalStars(catalog.Count)`
  (read-only). **No sprite swap, no PlayerAnimator coupling, no animation
  change.** A future phase may apply the equipped outfit visually once art
  assets exist.
- Analytics (local/debug-only, extends the P6 catalog; wire names pinned by
  test): `wardrobe_opened`, `cosmetic_previewed`, `cosmetic_equipped`,
  `cosmetic_unlock_failed`. No `cosmetic_unlocked` (unlocks are passive/
  derived). No spam during list population.
- Reset: `Jebby Jump/Reset/Reset Wardrobe` (equipped -> default) and Stars
  is included in **Reset Everything**; **Reset Stars** does NOT touch the
  wardrobe.
- Tests: `WardrobeTests` (catalog / store / unlock service) + pinned
  wardrobe analytics wire names.

Outfits are cosmetic-only and do not affect jump/speed/skills/damage/rank/
time/progression/level validation. Thresholds are placeholders pending P4B +
final level count + balance review. Manual visual QA of the Wardrobe panel
is **DEFERRED / NOT VERIFIED**. Recommended next phase: P10A Wardrobe Visual
QA / UI Polish Checklist.

## P10 - Wardrobe Visual QA / Art Readiness Plan

Status: implemented as **docs/checklist only**. No code/scene/prefab/asset/
test/runtime changes; no art/sprite swap. Deliverable:
`Assets/_JebbyJump/Docs/QA/Jebby_Jump_Wardrobe_Visual_QA_and_Art_Readiness_Plan_v0.1.md`
- a 22-section plan covering (P10A) the manual visual-QA checklist for the P9
Wardrobe panel (entry, layout, state matrix at 0/8/15/22/30 Stars, preview/
equip, reset/persistence, analytics, accessibility, copy, pass/fail,
screenshots) and (P10B/C) art-production + sprite-swap readiness.

It records the **actual** P9 UI copy ("Selected: <name>", "Locked (N Stars)",
"Equip", "Back") and the real `PlayerAnimator` contract (params Speed/
IsGrounded/VerticalVelocity + Land/Hurt/Victory triggers + Idle; single
SpriteRenderer with flipX) - future outfit art must author idle/run/jump/fall/
land/hurt/victory and preserve that contract (an AnimatorOverrideController per
outfit is the recommended sprite-swap mechanism). The rainbow-gem **motif**
(identity/art) is kept distinct from the Rainbow Gems **currency** (deferred).

Manual visual QA of the Wardrobe panel remains **DEFERRED / NOT VERIFIED**
(nothing captured or claimed in P10). Recommended next phase: P11A Wardrobe UI
Polish (if QA finds issues) or P11B Art Asset Production Spec.

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
