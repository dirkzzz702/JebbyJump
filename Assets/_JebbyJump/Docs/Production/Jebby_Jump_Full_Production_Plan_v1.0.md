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
| P11   | Wardrobe Visual Application Technical Foundation  | complete (Wardrobe.Visual asmdef: resolver + PlayerOutfitVisualController on Jebby.prefab; equipped outfit applied on Start as safe no-op until art; 76/76 tests; no gameplay/economy/art assets) |
| P12   | First Outfit Art Asset Request Pack / Visual Pipeline Readiness | complete (docs + test seam; Forest Cavalier pack; OutfitVisualApplier seam + 4 tests, 80/80; catalog stays no-op; no art assets) |
| P13   | Forest Cavalier Art Intake Prep (Mode A)         | superseded by Mode B below (was: blocked on art; QA gate added) |
| P13B  | Outfit Art Import + Visual Wiring (7 sets)       | complete (prototype art imported, 49/49 QA PASS; OutfitVisualLibrary wired into prefab; catalog 5->8 approved; outfits visibly swap; 89/89 tests) |
| P14   | Wardrobe Visual Expansion Stabilization          | complete (5 asset-integrity tests pin the real library/AOCs/prefab wiring; panel verified already ScrollRect-based for 8+ rows, no UI change; 94/94 tests) |
| P15   | Wardrobe UI Preview Cards + 8-Outfit Selection Polish | complete (UI-only WardrobePreviewLibrary + pure row-model builder; per-row dimmed thumbnails + selected preview; equip/select/locked behavior preserved; 107/107 tests; no art/semantic/gameplay/economy changes) |
| P16   | Wardrobe Unlock Ceremony + New-Unlock State      | complete (local acknowledgement store + pure new-unlock service + ceremony presenter/overlay; "New" badge; Reset Wardrobe/Everything clear ack, Reset Stars preserves; analytics pinned; 131/131 tests; Stars not consumed, ownership derived; no art/gameplay/economy changes) |
| P17   | Live Outfit Re-Sync + Default Visual Restoration | complete (WardrobeEquipService single equip path + WardrobeAppearanceEvents change event; applier restores captured default JebbyAnimator; PlayerOutfitVisualController live re-sync; missing entry -> default; 144/144 tests; Stars/unlock/ack unchanged; no art/gameplay/economy changes; live sync latent until same-scene wardrobe exists) |
| P18   | In-Panel Outfit Preview + Wardrobe Persistence Migration | complete (UI-only animated pose carousel from extended WardrobePreviewLibrary; WardrobePersistenceMigrator separates ongoing equipped normalization from one-time schema stamp (v1) - runs at MainMenu init + Wardrobe.Open; 170/170 tests; no event/analytics on migration; Stars/acks/thresholds untouched; no art/gameplay/economy changes) |
| P19   | Wardrobe Release Hardening + Save Migration Matrix | complete (read-only WardrobeStateAuditor + WardrobeStateSnapshot reading raw PlayerPrefs/key-presence; future-schema READ-ONLY policy + GetEffectiveOutfitId in-memory Classic fallback for player + panel; parameterized save-compatibility matrix + reset boundary + no-event/status/effective tests; editor Jebby Jump/QA/Audit Wardrobe State read-only command; no schema bump; Stars/unlocks/acks unchanged; no art/gameplay/economy changes) |
| P20   | Accessibility + Mobile (Landscape) Wardrobe UI Hardening | complete (landscape-only ProjectSettings lock; SafeAreaFitter + pure responsive region layout incl. compact variant, validated across 16:9/18:9/19.5:9/20:9/4:3 x notch shapes; 90-unit touch targets; deterministic keyboard/gamepad nav + real ceremony focus trap + scroll-into-view; non-color-only Selected marker; Reduce Motion setting in Main Menu + Pause reusing settings_changed; no gameplay/economy/migration/art-semantic changes) |
| P21   | Wider Shell Accessibility + Mobile Navigation Hardening | complete (extends P20 to Main Menu / Level Select / Settings / Pause / Result / Game Over; JebbyJump.Shell.Runtime pure helpers - ShellLayoutMetrics single-sources the 90 metric, ShellFocusResolver, GridNavigationBuilder, per-surface stack/grid bounds policies - + ShellFocusUtil + ShellScaffold; deterministic nav + initial focus + real modal traps + focus restore; true Level Select grid nav with focusable no-op locked cards + scroll-to-focus; dedicated GameShellCanvas (shell panels off the gameplay HUD/MobileControls canvases; 800x600 SequenceCanvas memory-gameplay left untouched); result/game-over made modal cards; settings hit-areas >=90 without oversizing toggle/slider visuals, slider Left/Right preserved; reuses settings_changed; no gameplay HUD/mobile-control/migration/economy/art changes) |

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

## P11 - Wardrobe Visual Application Technical Foundation

Status: **implemented** (functional foundation; no final art). The equipped
outfit id now has a safe visual application path:
`WardrobeStore.GetEquippedOutfitId()` -> `OutfitVisualCatalog` (resolver) ->
`PlayerOutfitVisualController` -> player `Animator`/`SpriteRenderer`.

A new `JebbyJump.Wardrobe.Visual` asmdef (references
`JebbyJump.Wardrobe.Runtime`) holds `OutfitVisualDefinition` (`OutfitId`,
`DisplayName`, `HasVisualOverride`, `RuntimeAnimatorController
AnimatorControllerOverride`), the pure asset-free `OutfitVisualCatalog`, and the
`PlayerOutfitVisualController` MonoBehaviour. The controller is attached to
`Jebby.prefab` (wired to the existing Animator + SpriteRenderer) via the
idempotent `AttachPlayerOutfitVisual` editor scaffold; the Game scene instance
inherits it, so `Game.unity` is not edited.

In P11 every outfit resolves to `HasVisualOverride=false` /
`AnimatorControllerOverride=null` (no art yet), so the on-Start apply is a
**no-op** - Jebby is visually unchanged and the default outfit maps to the
current visuals. The controller only assigns
`Animator.runtimeAnimatorController` when a definition carries a non-null
override, and never touches Animator parameters/state names or SpriteRenderer
flipX. Future outfit art (an `AnimatorOverrideController` or sprite-sheet) plugs
into the resolver without changing the controller.

9 new PlayMode tests cover resolution + fallback + the controller's no-op apply
(76/76 pass). No art/sprite/animation assets; no `WardrobeCatalog`/`Store`/
`UnlockService`/`StarReward*`/analytics changes; no gameplay/rank/progression/
economy changes; no shop/Spark Coins/Rainbow Gems currency/ads/backend.
**Known limitation:** non-default outfits still look like the default Jebby
until art is added (by design). Manual visual QA remains **DEFERRED / NOT
VERIFIED**. Recommended next phase: P12A First Outfit Art Asset Request Pack.

## P12 - First Outfit Art Asset Request Pack / Visual Pipeline Readiness

Status: **implemented** (docs + minimal test seam; no final art). Deliverable:
`Assets/_JebbyJump/Docs/Art/Jebby_Jump_First_Outfit_Art_Asset_Request_Pack_v0.1.md`
- a 16-section pack selecting **Forest Cavalier** (Art Bible's #1 first-outfit
candidate) and specifying identity guardrails (Design Lock section 2.3), the
actual 7 animation states (idle/run/jump/fall/land/hurt/victory), actual sprite
import settings (Sprite Single, PPU 100, pivot (0.5,0) bottom-center, Bilinear,
Mesh Tight, Alpha Is Transparency), transparency rules, file naming
(`spr_jebby_forest_cavalier_<state>_01.png`), the future asset folder layout,
and the AnimatorOverrideController pipeline (base = `JebbyAnimator`, override
the 7 default clips).

Code: a pure-static `OutfitVisualApplier` (single source of truth for the apply
rule) was extracted, and `PlayerOutfitVisualController.ApplyOutfit` delegates to
it with **identical behavior** (the catalog still returns a no-op definition for
every outfit). 4 new PlayMode tests prove an override controller is assigned
only when a definition carries one (in-memory controller; no committed asset).
**80/80 PlayMode tests pass** (76 + 4).

No final art / sprite / animation / `.controller` / `.overrideController`
assets; no `OutfitVisualCatalog`/`WardrobeCatalog`/`WardrobeStore`/
`WardrobeUnlockService`/`StarReward*`/`PlayerAnimator` changes; no
`SpriteRenderer` tint; no shop/Spark Coins/Rainbow Gems currency/ads/backend.
Outfit gameplay perks (an Art Bible "possible later" idea) stay deferred -
outfits remain cosmetic-only. **Known limitation:** non-default outfits still
look like the default Jebby until art exists. Manual visual QA remains
**DEFERRED / NOT VERIFIED**. Recommended next phase: P13A Generate / Request
Forest Cavalier Art Assets.

## P13 - Forest Cavalier Art Intake Prep (Mode A - blocked on art)

Status: ran as **Mode A (no art available)**. Intake was attempted: no Forest
Cavalier art exists in the repo or the provided working folders, and no
generation source was approved, so **nothing was imported** - no sprites, no
clips, no AnimatorOverrideController, no `Art/Characters/Jebby/Outfits/`
folders. `OutfitVisualCatalog` remains no-op for every outfit and
`forest_cavalier` still looks like default Jebby.

Added: a read-only editor QA gate for future outfit-art intake -
`Jebby Jump/QA/Check Outfit Sprite Alpha` (`CheckOutfitSpriteAlpha`, editor
only). It validates selected textures (or, with nothing selected, the 7
default Jebby sprites) against the recorded contract: Sprite type, Single
mode, PPU 100, pivot Custom (0.5, 0), Alpha Is Transparency on, and fully
transparent corner pixels (decoded from the PNG bytes, so Read/Write does not
need to be enabled). It never modifies importers or assets.

No gameplay/economy changes; no Wardrobe/StarReward semantics changed; no
shop/Spark Coins/Rainbow Gems currency/ads/backend. Manual visual QA remains
**DEFERRED / NOT VERIFIED**. **P13 is blocked on art**: provide or externally
generate the 7 state sprites per the P12 pack, then run the import phase
(Mode B: import + clips + `aoc_jebby_forest_cavalier` + catalog wiring +
tests).

## P13 Mode B - Outfit Art Import + Visual Wiring (7 sets)

Status: **complete** (supersedes the Mode A "blocked on art" state). The user
supplied 7 complete outfit sprite sets (7 states each, 49 PNGs) generated by
deterministic palette transfer from the approved default poses - **PROTOTYPE
art status** (identity-preserving, but not final-art-certified). Validation:
all 49 match the default per-state size (1122x1402), true RGBA, corners
alpha=0; after import the editor QA gate passed **49/49**.

Pipeline (built by the idempotent `ImportOutfitVisuals` scaffold under
`Art/Characters/Jebby/Outfits/<Outfit>/`): import settings matched to the
default contract; 7 single-frame sprite clips per outfit
(`anim_jebby_<id>_<state>.anim`, settings copied from the default
`Jebby_<State>` clips); one `aoc_jebby_<id>.overrideController` per outfit
(base `JebbyAnimator`, only the 7 clips overridden); all 7 registered in the
new `OutfitVisualLibrary` ScriptableObject; the library wired into
`PlayerOutfitVisualController._library` on `Jebby.prefab`. The static
`OutfitVisualCatalog` remains asset-free - overrides come only from the
serialized library (no Resources/paths/GUID loading).

With explicit user approval, `WardrobeCatalog` expanded **5 -> 8**:
rookie_page (4), crimson_hero (12), pastel_prince (26) - **STRICT
PLACEHOLDER** thresholds interleaved ascending; the original five
ids/thresholds/display names are untouched; the full 0..30 ladder stays
reachable with 10 levels. Equipping any non-default outfit now **visibly
swaps Jebby's sprites at spawn**; equipping Classic restores default visuals
on next spawn (scene load resets the Animator; live mid-scene re-sync is
intentionally still out of scope).

Animator params/triggers/states, `flipX`, `PlayerAnimator`, movement, and
default Jebby assets untouched; outfits remain cosmetic-only. 89/89 PlayMode
tests pass (catalog guardrails updated for the approved expansion + 5 new
library tests). Manual visual QA of the new outfits remains **DEFERRED / NOT
VERIFIED**. Recommended next phase: P14A Outfit Visual QA / Polish pass.

## P14 - Wardrobe Visual Expansion Stabilization

Status: **complete**. Stabilization pass after P13B, no new features.

UI readiness: inspection confirmed the wardrobe panel was **already
structurally 8+-safe** - the P9 scaffold built `ScrollRect -> Viewport ->
Content (VerticalLayoutGroup + ContentSizeFitter)` and rows are created
data-driven from `WardrobeCatalog` with no hardcoded count or positions, so
the 8 rows scroll. No UI/scene/prefab change was made; visual confirmation
of the scrolling list stays deferred.

Guardrails: new `OutfitVisualAssetIntegrityTests` (editor PlayMode,
`AssetDatabase` under `UNITY_EDITOR`) validate the REAL assets, closing the
gap where logic tests used only in-memory libraries: the
`OutfitVisualLibrary` asset has non-null entries for all 7 non-default
outfits and exactly 7 entries; the default outfit intentionally has NO entry
(no-op apply; Animator keeps the serialized `JebbyAnimator` at spawn); every
`aoc_jebby_<id>` is an AnimatorOverrideController based on `JebbyAnimator`
overriding exactly the 7 expected clips with correctly named outfit clips;
`Jebby.prefab`'s `PlayerOutfitVisualController` is wired to Animator,
SpriteRenderer, and the library asset; and the real `forest_cavalier`
override applies end-to-end through the real library. The end-to-end test
also documents the **spawn-only semantics**: re-applying the default
mid-scene does not clear an active override (by design; live mid-scene
re-sync remains intentionally unimplemented).

**94/94 PlayMode tests pass.** No runtime code, prefab, scene, art,
threshold, gameplay, or economy changes. Manual visual QA remains
**DEFERRED / NOT VERIFIED**. Recommended next: P15A Wardrobe Art Visual QA
Checklist Execution or P15B Wardrobe UI Preview Thumbnail / Outfit Card
Polish.

## P24 - Automated Performance + Memory + Build-Size + Stability Baseline

Status: **complete** (automated regression baseline). No physical device: editor/
headless metrics are regression signals, NOT FPS/battery/thermal certification
(DEFERRED / NOT VERIFIED). Frame target documented as 60 / 30-floor (targetFrameRate
+ vSync unchanged).

- **Two measured fixes (display-only, behavior-neutral):** the live HUD timer
  (`HUDController`) and skill-cooldown label (`ActiveSkillHUD`) allocated a string
  every frame; now they format into a reused `StringBuilder` via `TimeFormat`
  (Core.Runtime) using .NET `TryFormat` into a stack buffer - **zero allocation** and
  **exactly text-equivalent** to the old `$"{m:00}:{rest:00.00}"` / `$"{x:F1}"`
  (proven by dense sweep tests; .NET formats the shortest-round-trippable value +
  rounds half-away-from-zero, which a hand-rolled rounder can't match - hence
  TryFormat). Pre-fix evidence captured before editing (correction #1).
- **Measurement (correction #2):** Unity Test Framework GC constraint
  (`Is.[Not.]AllocatingGCMemory`), not `GC.GetAllocatedBytesForCurrentThread` (returns
  0 under the editor Boehm GC). Subscriber-leak probe via reflection on the event's
  backing delegate (correction #6).
- **Tooling (editor-only `JebbyJump.Performance.Editor`):** pure report models,
  `PerformanceRegressionPolicy` (leak/GC zero-tolerance; timing relative-only; never
  device-cert), `BuildSizeMath`, `BuildSizeAuditTool` (authoritative compressed AAB
  from the P23 report + packed contributors from a fresh `BuildReport`; distinguishes
  bytes/MB/MiB/packed/compressed). `ProfilerMarker`s on scene/spawn/swatch paths.
- **Build size (corrections #8,#9):** compressed AAB 113,625,618 -> 113,630,295 B
  (**+4,677 B / ~4.6 KiB**, markers + formatters; ships per Decision 8). Packed total
  ~337 MB; outfit sprite PNGs (~6.29 MB packed each) dominate.
- **Re-ran P23 gates (correction #10):** assembly-exclusion (EditMode), preflight,
  warning gate, Android AAB build - all still complete.
- Tests 355 -> 379 (PlayMode 333, EditMode 46); outfit QA 49/49. Committed env-tagged
  baseline (`Docs/Performance/...baseline_v0.1.json`); raw samples ignored
  (`Builds/P24`). No gameplay/reward/wardrobe/migration/quality/import/package change.
  Recommended next: P25A manual device/visual QA, or P25B P4B balance playtest.

## P23 - Production Build + Release-Candidate Automated Readiness

Status: **complete** (automated build-readiness layer). Proves the repo produces a
clean, correctly configured **Android AAB** via an editor-only preflight + CLI
builder + artifact/hash report. Claims automated build readiness ONLY - never store/
signing/device/visual/performance/balance/art. Manual QA DEFERRED / NOT VERIFIED.

- **Editor-only tooling** (`JebbyJump.Release.Editor`, `includePlatforms:["Editor"]`):
  excluded from the player; pure DTOs tested via a new `JebbyJump.Tests.EditMode`
  asmdef. `[Serializable]` field-based report serialized with JsonUtility in the
  editor layer.
- **Fixed the release blocker:** `EditorBuildSettings` enabled only a non-existent
  `SampleScene`. `Apply Approved Build Config` (the only tracked-config writer) sets
  identity (SparkLibrary / com.sparklibrary.jebbyjump) + the scene list from the
  immutable `ReleaseSceneContract` (Boot->MainMenu->Game; persisted to disk directly
  because the EditorBuildSettings API does not flush in batchmode).
- **Preflight (validate, never fix):** identity/scenes/orientation/input/backend/arch/
  packages/required-assets; read-only AssetDatabase loads + TMP digit-glyph check; no
  SaveAssets/menu-tools/mutation. Fails on drift.
- **Builder:** Android AAB; **Windows smoke only if the Android toolchain is
  unavailable** (statuses AndroidAabBuilt / AndroidToolchainBlocked_WindowsSmokePassed
  / AndroidBuildFailed - a real Android failure is never masked). Captures + restores
  build target / `buildAppBundle` / dev-debug-profiler flags in try/finally; menu
  never exits; CLI exits non-zero on failure; always writes the report; post-build
  warning gate (classified allowlist). Honest signing (debug-signed; store upload
  external). Hashes the complete distributable (AAB file; or full Windows dir
  manifest), paths relative to Builds/P23, no secrets.
- **Packages** explicitly classified (purchasing/gdk/cloud-build/coplay = unused/
  editor/target-irrelevant); none added/removed.
- Tests: a full EditMode release suite (preflight matrix, decision/exit/readiness,
  state-guard restore, report/secrets/relative-paths, hasher, package classification,
  editor-only asmdef, runtime/editor + manifest audits) + the unchanged PlayMode suite
  + outfit QA. Recommended next: P24A manual device/visual QA, or P24B perf profiling.

## P22 - Gameplay HUD + Mobile Controls + Memory Accessibility Hardening

Status: **complete** (automated structural layer). Extends the P20/P21
accessibility patterns to the ACTIVE gameplay layer: the HUD (lives/level/timer +
feedback), the mobile controls (move/jump/3 skills/pause), and the memory phase
(swatches + platforms). No gameplay rules, physics, balance, timing, level
content, answer validation, or input semantics changed. Rendered/device QA
DEFERRED / NOT VERIFIED.

**Input gate (correction #1):** `GameplayModalInputGate` (on the always-active
HUDCanvas) blocks AND clears the gameplay input layer under any shell modal -
disables the mobile-control `CanvasGroup` AND calls
`InputReader.SetGameplayInputEnabled(false)` (ignores move/jump/skill for
keyboard/gamepad/touch and releases held state), while leaving Pause + UI nav
active. Keys off the explicit active panels (result/game-over) + `PauseState`,
not the GameShellCanvas object. Pure decision tested via `GameplayInputBlockPolicy`.

**Memory cues (corrections #2,#3,#4,#6):** opt-in `AccessibilitySettingsStore.MemoryCues`
(default OFF, both Settings panels, in Reset Defaults, `setting_name=memory_cues`)
renders a stable non-color cue (numbers 1-6 via an explicit, tested
`PlatformCueMapping` in the new `JebbyJump.Core.Runtime` asmdef) on BOTH the
swatches and the spawned platforms - same mapping on both. Swatch labels are
idempotent (rebuilt per sequence, subscribed once); platform labels are
world-space `TextMeshPro` (sorted above the sprite, no collider/input,
counter-scaled, safely unsubscribed). Validation/timing/order/colors unchanged.

**SequenceCanvas:** migrated off the legacy 800x600 Constant-Pixel scaler to the
standard SWSS 1920x1080 (visual/layout-only; RenderMode + sorting preserved, stays
below GameShellCanvas); swatch proportions/relative positions kept; top safe-area
inset applied. The on-screen swatch size changes - rendered re-check deferred.

**Layout (correction #5):** HUD content, controls, and the memory `SequencePanel`
each sit under a `SafeArea` root (reusing the P20/P21 `SafeAreaCalculator` /
`ShellScaffold`). Controls stay >=90 (never shrunk). A combined cross-canvas
`GameplayLayoutPolicy` asserts the PauseButton clears the Timer + skills and that
clusters do not collide across the 16:9/18:9/19.5:9/20:9/4:3 x safe-area matrix.

Tests: pure (`PlatformCueMappingTests`, `GameplayInputBlockPolicyTests`,
`GameplayLayoutPolicyTests`, Memory-Cues store tests) + `ShellSceneIntegrityTests`
(one gate; SequenceCanvas converted off 800x600; 5 SafeArea roots; fitters wired).
Scaffolds (`BuildSettingsPanel`, `BuildGameplayAccessibility`) run twice with no
second-run diff (fixed a latent pause SettingsButton sibling-order oscillation).
317/317 PlayMode tests pass. No new analytics/packages; no
Jebby.prefab/physics/skill/timer/economy/art changes. Recommended next: P23A
manual visual + device QA, or P23B production build / RC checklist.

## P21 - Wider Shell Accessibility + Mobile Navigation Hardening

Status: **complete** (automated structural layer). Extends the P20 wardrobe
accessibility patterns to the non-gameplay shell: Main Menu, Level Select,
Settings (Main Menu + Pause), Pause, Pause->Settings, Level Complete, Game Over.
No gameplay/economy/wardrobe-semantic changes; gameplay HUD/mobile controls +
the memory-phase SequenceCanvas are untouched. Rendered/device QA DEFERRED.

**Canvas (correction #1, verified):** the Game.unity 800x600 Constant-Pixel
canvas is **SequenceCanvas** (memory-phase gameplay) - left unchanged, flagged
for P22A. The shell panels shared gameplay canvases (LevelComplete/GameOver on
HUDCanvas; Pause/Settings on MobileControlsCanvas), so `BuildGameShellCanvas`
creates a dedicated **GameShellCanvas** (1920x1080 SWSS, sorted above gameplay)
and moves them onto it (serialized refs survive). No gameplay-canvas scaler
change.

**Shared layer:** new `JebbyJump.Shell.Runtime` - `ShellLayoutMetrics`
(canonical 90 touch metric; `WardrobeLayoutMetrics` now single-sources it),
`ShellFocusResolver`, `GridNavigationBuilder` (true grid incl. partial rows),
`ShellStackLayoutPolicy` + `ShellGridLayoutPolicy` (per-surface landscape bounds,
not one proof for all - correction #3). Plus `ShellFocusUtil` (focus/nav/grid/
modal-trap) + `ShellScaffold` (safe-area re-parent, hit-area sizing, standard
scaler).

**Per-surface:** safe-area content roots (backdrops edge-to-edge); >=90 touch
targets (settings toggle/slider get a >=90 hit area with small visuals, slider
Left/Right preserved); explicit nav + deterministic initial focus; real modal
traps (pointer backdrop + focus-island re-assert + exact restore - correction
#4); Level Select true grid nav with **focusable but no-op (no-analytics) locked
cards** (correction #2) + scroll-to-focus; result/game-over centered cards turned
into modals (full-screen backdrop + Card). Result focus fallback Next->Retry->
Menu / Retry. Pause keeps `Time.timeScale`/`PauseState`.

**Reduced motion:** the shell has no transition animations, so the Wardrobe pose
carousel remains the only affected motion (documented; nothing invented). Same
`AccessibilitySettingsStore` setting in both settings panels.

Tests: pure (`ShellAccessibilityPureTests`) + `ShellSceneIntegrityTests`
(GameShellCanvas, result/game-over modal cards, MenuSafeArea, one EventSystem +
InputSystemUIInputModule per scene, SequenceCanvas preserved); the P20 wardrobe
scene-integrity test relaxed for the added shell SafeArea roots. Scaffolds run
twice (idempotent). No new analytics; no package/Jebby.prefab/gameplay-script
changes. Recommended next: P22A gameplay HUD + mobile-controls hardening, or
P22B manual visual + device QA.

## P20 - Accessibility + Mobile (Landscape) Wardrobe UI Hardening

Status: **complete** (automated structural layer). Accessibility + mobile
interaction hardening for the Wardrobe and its entry/overlay surfaces. No new
wardrobe feature; no outfit/reward/migration/gameplay/economy/art semantic
changes. Rendered/on-device QA remains DEFERRED / NOT VERIFIED.

**Orientation (confirmed):** the game is **landscape-only**. ProjectSettings now
locks landscape (portrait autorotate disabled; AutoRotation between the two
landscapes); reference resolution stays 1920x1080. Stale "portrait-first" doc
lines were corrected.

**Safe area:** pure `SafeAreaCalculator` + a runtime `SafeAreaFitter` on the
wardrobe content root (`SafeArea`) and the ceremony card root
(`CeremonySafeArea`); dim backdrops stay edge-to-edge. Cheap change-detection
(no per-frame rebuild).

**Responsive layout (objective validation):** pure `CanvasScalerMath` +
`WardrobeResponsiveLayout` compute Header/List/Preview/Action region rects from
the safe-area content size, switching to a **compact** layout on short landscape
screens (20:9 logical height ~966). `WardrobeResponsiveLayoutTests` assert region
bounds ⊆ safe area, non-overlap, and touch targets >= 90 across every approved
aspect (16:9/18:9/19.5:9/20:9/4:3) x safe-area shape. The panel applies the
computed layout to scaffolded region containers at runtime.

**Touch / scroll:** `WardrobeLayoutMetrics` single-sources a 90-unit minimum
(rows raised 64->90; action/ceremony buttons 90). Row preview + selection-bar
Images are non-raycast; ScrollRect stays vertical-only/data-driven.

**Navigation / focus:** explicit row<->row / row->Equip / Equip<->Back /
ceremony nav (pure `WardrobeFocusResolver`); initial focus = equipped/first row;
ceremony focus = Equip Now/Continue; close restores the opener. **Real focus
trap**: underlying rows/Equip/Back set non-interactable AND focus re-asserted to
a ceremony control each frame. Focused rows scroll into view (pure
`ScrollIntoViewCalculator`).

**Readability / non-color:** state stays text (`Equipped`/`Unlocked`/
`Locked (N Stars)`/`New`); a structural left **Selected** bar marks the selected
row; row labels use ellipsis overflow. No icon assets imported.

**Reduce Motion:** new `AccessibilitySettingsStore` (`jebby.settings.reduceMotion`,
default false) + a "Reduce Motion" toggle in **both** Main Menu and Pause
settings (Game.unity scoped change). ON freezes the preview to a static Idle;
cached + event-driven (no per-frame PlayerPrefs read); toggling while open
re-applies. Reuses `settings_changed` (`setting_name=reduce_motion`); cosmetic UI
motion only - gameplay movement/timers/physics untouched.

Tests: pure layout/safe-area/scroll/focus/metrics/reduce-motion/store +
readability + `WardrobeAccessibilitySceneIntegrityTests` (YAML: one SafeArea +
one CeremonySafeArea, region/scrollRect refs wired, EventSystem present, Reduce
Motion toggle in both scenes). Scaffolds run twice (idempotent; integrity tests
assert singletons). No new analytics for focus/scroll/safe-area/carousel frames.
No package/Jebby.prefab changes. Recommended next: P21C manual visual + device
QA, or P21E wider shell accessibility hardening.

## P19 - Wardrobe Release Hardening + Save Migration Matrix

Status: **complete**. Release-hardening of the whole wardrobe save subsystem;
runtime/editor/tests/docs only. No new player-facing feature, no schema bump, no
scene/prefab/art/economy/gameplay changes.

**Future-version policy (changed from P18, user-approved).** A stored schema
GREATER than `CurrentVersion` (e.g. a downgrade after a later build) is now
treated as **READ-ONLY**: `WardrobePersistenceMigrator.MigrateIfNeeded` makes
no normalization writes, no schema write, no downgrade, no Stars/acknowledgement
change, and (by Runtime-asmdef isolation) no event/analytics; it returns
`FutureVersionUnsupported`. The game shows a safe **in-memory Classic** via the
new read-only `GetEffectiveOutfitId()` (Stars-free, used by
`PlayerOutfitVisualController` at spawn) and `GetEffectiveOutfitId(totalStars)`
(used by the wardrobe panel display) - the on-disk future save is never
rewritten. MainMenu logs a warning in editor/development builds. (P18 previously
still normalized/wrote on a future version; that test was replaced.)

**Migration result hardening.** `WardrobeMigrationResult` gained `Status`
(`NoChange` / `MigratedLegacy` / `NormalizedCurrent` / `MigratedAndNormalized` /
`FutureVersionUnsupported`) and `DidWrite`. The supported path still separates
ongoing equipped normalization (every call) from the one-time schema stamp, and
now writes the **schema last** (equipped first) so an interrupted run re-runs
safely (idempotent recovery).

**Read-only audit.** `WardrobeStateAuditor.AuditPersistence(totalStars)` builds a
`WardrobeStateSnapshot` from **raw PlayerPrefs + key presence** (never the
sanitized store), so it distinguishes a **missing equipped key** (clean implicit
Classic - not an issue) from a **present-empty value** (invalid, repairable under
a supported schema). It classifies schema (`Missing`/`Legacy`/`Future`) and
equipped (`Empty`/`Unknown`/`Locked`) issues; a future schema is explicitly
**non-repairing** (`IsSupportedSchema=false`, `RequiresMigration=false`,
`RequiresNormalization=false`). It performs no writes.

**Editor QA tool.** `Jebby Jump/QA/Audit Wardrobe State` (read-only, null-safe
for missing assets) logs schema/support, raw + normalized equipped id, Stars,
unlocked/acknowledged/new-unlocked, visual (7/7) + preview (8/8) + pose (56/56)
registration, and any issues. It never writes PlayerPrefs or assets.

Tests: `WardrobeStateAuditorTests`, parameterized
`WardrobeSaveCompatibilityMatrixTests` (legacy/current normalization, threshold
boundaries, acknowledgement preservation, future no-writes, repeatability),
`WardrobeResetBoundaryTests`, expanded `WardrobePersistenceMigrationTests`
(future read-only, no-appearance-event, status, schema-last recovery, missing vs
empty, effective read), and a duplicate-visual-id integrity guard. See the
**Wardrobe Save Compatibility Matrix v0.1** doc (every case maps to a test).

No changes to `WardrobeCatalog`/`WardrobeUnlockService` rules, acknowledgement
semantics, P17 equip/event semantics, `StarReward*`, `OutfitVisualLibrary`/
`WardrobePreviewLibrary` contents, `PlayerAnimator`/`PlayerMotor`/`JebbyAnimator`,
Jebby prefab, scenes, or art. Automated release-hardening complete; manual
rendered UI/art/gameplay and on-device migration QA remain **DEFERRED / NOT
VERIFIED**; prototype art is not final-certified. Recommended next: P20C
accessibility + mobile wardrobe UI hardening, or P20D manual visual QA execution.

## P18 - In-Panel Outfit Preview + Wardrobe Persistence Migration

Status: **complete**. Two coordinated objectives; runtime/tests/docs + the
preview-library asset only. No scene/prefab/art changes.

(A) **In-panel preview** (Option A - UI Image pose carousel; chosen because the
outfit clips are single-frame and per-state sprites already exist). The existing
P15 `SelectedPreview` Image is now animated: on selection the panel builds a
sequence via `WardrobePreviewSequenceBuilder` and advances it with a pure
`WardrobePreviewPlayer` on `Time.unscaledDeltaTime`. Sequence =
idle->run->jump->fall->land->victory (Hurt excluded from the child-facing loop);
locked outfits are dimmed (alpha 0.4); missing poses are skipped; a missing
sequence hides the image (no stale sprite). It is **UI-only and fully separate
from the gameplay Jebby**. `WardrobePreviewLibrary` was extended from a single
idle sprite to the full per-pose set; `TryGetPreview` still returns Idle so P15
rows + the P16 ceremony are unchanged. No new scene objects (no MainMenu.unity
change).

(B) **Persistence migration.** `WardrobePersistenceKeys` centralizes the
existing keys (now referenced by `WardrobeStore` + the acknowledgement store)
and adds `jebby.wardrobe.schemaVersion`. `WardrobePersistenceMigrator` keeps two
**independent** jobs: ongoing equipped-id normalization (unknown/empty/locked ->
default) that runs on **every** call regardless of schema version, and a
one-time schema stamp (`CurrentVersion = 1`; absent = legacy 0) applied only when
behind; a future (greater) stored version is never downgraded. Corrections are
written via `WardrobeStore.SetEquippedOutfitId` (the low-level primitive), so
**no appearance event and no analytics fire**; Stars, acknowledgements, and
thresholds are never touched. It is invoked at **Main-Menu init (before Continue
or any gameplay scene load)** and defensively at the top of `Wardrobe.Open`
(idempotent). Reset Wardrobe stamps the current schema version (a clean reset
does not replay history); Reset Stars preserves schema + acknowledgements.

Tests: sequence builder, preview player (empty/zero-negative/large-delta safe),
migration (incl. the regression that a CURRENT schema still normalizes a
now-locked equipped id after a Stars drop; future-version-not-downgraded;
no-Star/ack change; idempotent), key-literal pins, and preview asset-integrity
(all 8 outfits have all 7 pose sprites matching paths). **170/170 PlayMode tests
pass**; outfit-sprite QA gate still 49/49; preview-library scaffold rerun is
hash-stable.

No changes to WardrobeCatalog/WardrobeUnlockService rules, acknowledgement
semantics, P17 equip-event semantics, StarReward*, OutfitVisualLibrary,
PlayerAnimator/PlayerMotor/JebbyAnimator, Jebby prefab, Game.unity, or art.
Manual visual QA of the rendered preview + on-device migration remains
**DEFERRED / NOT VERIFIED**. Recommended next: P19C save-migration test matrix /
release hardening, or P19A outfit-swap transition polish.

## P17 - Live Outfit Re-Sync + Safe Default Visual Restoration

Status: **complete**. Adds a same-scene live appearance re-sync after a
successful equip + correct default-controller restoration. Runtime/tests/docs
only - no scene/prefab/art changes.

`WardrobeEquipService.TryEquip(id, totalStars)` (Runtime, pure, no analytics) is
now the single validated equip path for both the normal Wardrobe Equip button
and the P16 ceremony Equip Now: returns `Success` / `AlreadyEquipped` /
`UnknownOutfit` / `Locked`; on Success writes through `WardrobeStore` and
publishes `WardrobeAppearanceEvents.EquippedOutfitChanged` (stable ids; only on
Success). `WardrobeStore` stays the persistence primitive (key/normalization/
reset unchanged). Stars and acknowledgement state are never touched by the
service.

`OutfitVisualApplier` was refactored to
`Apply(Animator, RuntimeAnimatorController defaultController, OutfitVisualDefinition)`
returning `OutfitVisualApplyResult { AppliedOverride, RestoredDefault,
NoAnimator, NoOp }`: a real override assigns it; otherwise it RESTORES the
captured default controller (so a previous override never lingers); a null
default on the restore path is a NoOp (never assigns null). It still only
touches `Animator.runtimeAnimatorController` - never params/triggers/states,
flipX, color, materials, or sorting; no Resources/paths.

`PlayerOutfitVisualController` now captures the prefab's default `JebbyAnimator`
in **Awake**, subscribes on **OnEnable** / unsubscribes on **OnDisable** +
**OnDestroy** (no leak, no DontDestroyOnLoad), re-applies on the change event,
and still applies the stored outfit in **Start** (spawn path retained). Missing
library/entry falls back to default. A live controller swap may restart the
current animation - **allowed and documented** (no state preservation; all
outfit AOCs share the JebbyAnimator base if continuity is added later).

UI: both equip sites route through the service; the panel emits exactly one
canonical `cosmetic_equipped` per successful equip (`source=wardrobe` or
`unlock_ceremony`) and the visual subscriber emits nothing. Ceremony Equip Now
treats `AlreadyEquipped` as success-for-flow (still acknowledges + advances);
`Locked`/`Unknown` do not acknowledge/advance. Reset unchanged (editor-only, no
active player).

Tests: equip service (Success/Locked/Unknown/AlreadyEquipped, publish-on-success-
only, unsubscribe, no-Star-mutation), controller live re-sync (event updates,
disable unsubscribes, re-enable resubscribes), applier (override/NoOp/NoAnimator),
and `#if UNITY_EDITOR` integration against the real `OutfitVisualLibrary` +
`JebbyAnimator` (Forest->Aqua applies Aqua; Aqua->default restores JebbyAnimator;
all 7 overrides apply). **144/144 PlayMode tests pass**; outfit-sprite QA gate
still 49/49.

**Known limitation:** the wardrobe is Main-Menu-only and the player lives in
Game.unity, so the live event currently has no in-scene subscriber - live sync
is latent until an in-game wardrobe exists; the spawn path covers scene loads,
and the visible P17 win is correct default restoration + a unified equip path.
No `WardrobeStore`/`WardrobeUnlockService`/`WardrobeCatalog`/`StarReward*`/
`OutfitVisualLibrary`/`PlayerAnimator`/`JebbyAnimator`/Jebby-prefab/Game.unity/art
changes. Manual visual QA remains **DEFERRED / NOT VERIFIED**. Recommended next:
P18A in-panel animated preview or P18F persistence/migration hardening.

## P16 - Wardrobe Unlock Ceremony + New-Unlock State Foundation

Status: **complete**. Adds a local, deterministic outfit unlock ceremony +
acknowledgement state. Functionality only - no shop/currency/ads/backend/art.

Data: new `WardrobeUnlockAcknowledgementStore` (Runtime, per-outfit PlayerPrefs
key `jebby.wardrobe.unlockAcknowledged.<id>`) records which unlock presentations
the player has seen. **Acknowledgement is NOT ownership** - ownership stays derived
from total Stars via `WardrobeUnlockService`; Stars are never consumed. null/empty/
unknown ids are ignored on read+write; the AlwaysUnlocked default is treated as
acknowledged and never gets a key. Pure `WardrobeNewUnlockService` returns
currently-unlocked-but-unacknowledged outfits in catalog order (default + locked +
acknowledged excluded). Pure `WardrobeCeremonyPresenter` (Visual) queues them; it
acknowledges (injected store) and equips (injected `WardrobeStore` path) **only** on
Continue / Equip Now, and a failed equip leaves the queue untouched.

UI: on Wardrobe open the queued outfits are presented one at a time in an
`UnlockCeremonyOverlay` scaffolded into MainMenu.unity by an extended (idempotent)
`BuildWardrobePanel` - dim backdrop blocks the rows beneath, Back is disabled until
Continue/Equip. Equip Now uses the existing `WardrobeStore.SetEquippedOutfitId`
(appearance applies at next spawn; P16 had no live re-sync - SUPERSEDED by P17
same-scene live re-sync). The P15 preview rows gained a
"New" badge (unlocked + non-default + unacknowledged) via an optional
`isAcknowledged` predicate on `WardrobeRowModelBuilder` (+ `IsNew` on the row model).
Existing players with Stars see each eligible ceremony once on first P16 open.

Reset (`ResetProgressTool`): Reset Wardrobe -> `WardrobeStore.Reset()` +
`WardrobeUnlockAcknowledgementStore.ResetAll()` (ceremonies replay for currently-
eligible outfits); Reset Everything includes it; Reset Stars preserves
acknowledgements; new menu **Reset Wardrobe Unlock Acknowledgements**.

Analytics (local, pinned): `cosmetic_unlock_presented` (once when an item is shown),
`cosmetic_unlock_acknowledged` (Continue/Equip, distinguished by
`acknowledgement_action`), and the existing `cosmetic_equipped` reused with
`source=unlock_ceremony` (no separate equip event). Params add `queue_position`/
`queue_count`. Never emitted during queue build / row refresh / scaffold.

Tests: acknowledgement store, new-unlock service, ceremony presenter, IsNew badge,
analytics wire-name pins, and a YAML scene-integrity test (one overlay + ceremony
refs wired - read from MainMenu.unity text, no asmdef restructure). **131/131
PlayMode tests pass**; outfit-sprite QA gate still 49/49. No changes to
`WardrobeStore`/`WardrobeUnlockService`/`WardrobeCatalog`/`StarReward*`/
`OutfitVisualLibrary`/`PlayerAnimator`/Jebby prefab/art/thresholds. Ceremony rendered
appearance remains **DEFERRED / NOT VERIFIED**. Recommended next: P17B live
mid-scene re-sync or P17A in-panel animated preview; P17E manual visual QA.

## P15 - Wardrobe UI Preview Cards + 8-Outfit Selection Polish

Status: **complete**. The text-only wardrobe list became a data-driven
preview-card list with **UI-only** outfit thumbnails.

Data model: a NEW `WardrobePreviewLibrary` ScriptableObject (id -> Sprite),
deliberately **separate** from `OutfitVisualLibrary` so P14's gameplay-override
invariants stay untouched. It maps all 8 outfits (incl. the default) to their
existing idle sprites; null-safe (missing entry/sprite -> no thumbnail). A pure
`WardrobeRowModelBuilder` produces `WardrobeOutfitRowModel`s (OutfitId,
DisplayName, RequiredStars, IsUnlocked, IsEquipped, PreviewSprite, StateText,
CanEquip), iterating `WardrobeCatalog.Outfits` and normalizing the equipped id
internally - so the panel MonoBehaviour (Assembly-CSharp, not test-reachable)
only renders, and the logic stays unit-tested.

UI: `WardrobePanelController` renders per-row thumbnails (locked = dimmed via UI
Image alpha 0.4 - NOT art tint; missing = hidden; `preserveAspect`,
`raycastTarget` off so the row button keeps clicks) + a larger selected-outfit
preview; equip/select/locked/equipped behavior, copy, analytics, and equipped-id
normalization are unchanged. Rows remain runtime-built and data-driven (no
hardcoded count). Idempotent scaffolds: `BuildWardrobePreviewLibrary` (populates
the asset from existing idle sprites) and an extended `BuildWardrobePanel` (wires
`_previewLibrary` + a `SelectedPreview` image into MainMenu.unity); re-run adds no
duplicate objects.

**107/107 PlayMode tests** (94 + 13: preview library, builder, and preview-asset
integrity). Outfit-sprite QA gate still 49/49 (import settings untouched). No new
art; no `OutfitVisualLibrary`/Animator/`WardrobeStore`/`WardrobeUnlockService`/
StarReward/threshold/gameplay/economy changes. Outfits still apply at next spawn
(no live mid-scene re-sync). Manual visual QA of the rendered cards remains
**DEFERRED / NOT VERIFIED**. Recommended next: P16A unlock ceremony or P16B
in-panel live character preview; P16E manual visual QA when a tester is available.

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
