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
P13 — Forest Cavalier Art Intake Prep (Mode A)        : complete-but-BLOCKED ON ART (no Forest Cavalier art exists in repo or working folders; nothing imported; no override wired; OutfitVisualCatalog stays no-op for every outfit; read-only editor QA gate added: Jebby Jump/QA/Check Outfit Sprite Alpha; next step: provide/generate the 7 state sprites per the P12 pack, then run import phase; no gameplay/economy changes; manual visual QA DEFERRED/NOT VERIFIED)
P13 Mode B — Outfit Art Import + Visual Wiring (7 sets) : complete (user-provided palette-transfer PROTOTYPE art, 7 outfits x 7 states, 49/49 QA-gate PASS; sprites+clips+AnimatorOverrideControllers under Art/Characters/Jebby/Outfits/; new OutfitVisualLibrary SO wired into Jebby.prefab PlayerOutfitVisualController; WardrobeCatalog expanded 5->8 with approved rookie_page(4)/crimson_hero(12)/pastel_prince(26) PLACEHOLDER thresholds, original five untouched; equipping a non-default outfit now visibly swaps Jebby's sprites on spawn; cosmetic-only, no gameplay/economy changes; 89/89 tests; manual visual QA DEFERRED/NOT VERIFIED)
P14 — Wardrobe Visual Expansion Stabilization          : complete (asset-integrity guardrails added: 5 editor PlayMode tests validate the REAL OutfitVisualLibrary asset (7 non-default entries, default intentionally absent), every AnimatorOverrideController's base=JebbyAnimator + 7 expected clip overrides, Jebby.prefab wiring, and end-to-end equipped-override apply through the real library; Wardrobe panel verified ALREADY structurally 8+-safe (P9 ScrollRect/Viewport/Content, data-driven rows, no fixed positions) so no UI refactor; 94/94 tests; no code/prefab/scene/art/threshold changes; manual visual QA DEFERRED/NOT VERIFIED)
P15 — Wardrobe UI Preview Cards + 8-Outfit Selection Polish : complete (UI-only outfit thumbnails: new WardrobePreviewLibrary SO (separate from OutfitVisualLibrary; idle sprite per outfit incl. default, 8 entries, 49/49 QA still PASS) + pure WardrobeRowModelBuilder/WardrobeOutfitRowModel; WardrobePanelController now renders per-row thumbnails (locked=dimmed, missing=hidden, preserveAspect, raycastTarget off) + a selected-outfit preview, driven by the builder; equip/select/locked/analytics/normalization behavior preserved; scaffolds idempotent (BuildWardrobePreviewLibrary + BuildWardrobePanel wires _previewLibrary + SelectedPreview into MainMenu.unity, no duplicate objects); 107/107 tests; no art/Animator/WardrobeStore/UnlockService/Stars/threshold/gameplay/economy changes; manual visual QA DEFERRED/NOT VERIFIED)
P16 — Wardrobe Unlock Ceremony + New-Unlock State        : complete (local acknowledgement: new WardrobeUnlockAcknowledgementStore (per-outfit PlayerPrefs key; NOT ownership) + pure WardrobeNewUnlockService (catalog-order, excludes default/locked/acknowledged) + pure WardrobeCeremonyPresenter (queue; acknowledge only on Continue/Equip Now; failed equip stays); on Wardrobe open, newly-unlocked unacknowledged outfits present one at a time in an UnlockCeremonyOverlay (Equip Now via existing WardrobeStore path + Continue; Back disabled during ceremony; rows blocked); P15 rows gain a "New" badge (unlocked+unacknowledged); Reset Wardrobe/Everything clear acknowledgements, Reset Stars preserves them; analytics cosmetic_unlock_presented/acknowledged + cosmetic_equipped(source=unlock_ceremony), pinned; 131/131 tests; Stars never consumed, ownership still derived; no art/threshold/gameplay/economy changes; manual visual QA DEFERRED/NOT VERIFIED)
P17 — Live Outfit Re-Sync + Default Visual Restoration   : complete (new WardrobeEquipService = single validated equip path (Success/AlreadyEquipped/UnknownOutfit/Locked) used by both the normal Equip button and the ceremony Equip Now; raises WardrobeAppearanceEvents.EquippedOutfitChanged on Success only; OutfitVisualApplier refactored to 3-arg + result enum that RESTORES the captured default JebbyAnimator when an outfit has no override (no stale overrides), NoOp on null default; PlayerOutfitVisualController captures the default in Awake, subscribes OnEnable/unsubscribes OnDisable, live re-syncs on the event, keeps the Start spawn path; missing entry/library falls back to default; animation may restart on swap (documented); 144/144 tests incl. real-library Forest->Aqua->default integration; Stars/unlocks/acknowledgements unchanged; no art/gameplay/economy changes; manual visual QA DEFERRED/NOT VERIFIED. NOTE: wardrobe is Main-Menu-only today so live sync has no in-scene player yet - latent until an in-game wardrobe exists; the visible win is correct default restoration + a unified equip path)
P18 — In-Panel Outfit Preview + Wardrobe Persistence Migration : complete (A) preview: animated UI-only pose carousel (idle->run->jump->fall->land->victory, Hurt excluded; locked dimmed) driving the existing SelectedPreview Image on unscaled time - WardrobePreviewLibrary extended to per-pose sprites (TryGetPreview still returns Idle so P15 rows/ceremony unchanged) + pure WardrobePreviewSequenceBuilder + pure WardrobePreviewPlayer (large-delta/empty/zero-dt safe); separate from the gameplay Jebby. (B) persistence: new WardrobePersistenceKeys (centralizes the existing keys + a new jebby.wardrobe.schemaVersion) + WardrobePersistenceMigrator - ONGOING equipped normalization (unknown/empty/locked->default) runs every call regardless of schema version, ONE-TIME schema stamp (current=1; absent=legacy) only when behind, future version never downgraded; corrections write via WardrobeStore so NO event/analytics fire; never touches Stars/acks/thresholds. Invoked at MainMenu init (before Continue) + defensively in Wardrobe.Open. Reset Wardrobe stamps current schema. 170/170 tests; no art/gameplay/economy changes; manual visual QA DEFERRED/NOT VERIFIED)
P19 — Wardrobe Release Hardening + Save Migration Matrix : complete (release-hardening only, no new feature, NO schema bump; future schema (>current) now READ-ONLY - MigrateIfNeeded makes no normalization/schema/Stars/ack writes, no event/analytics, returns FutureVersionUnsupported, and the game shows in-memory Classic via new read-only GetEffectiveOutfitId([stars]) used by PlayerOutfitVisualController spawn + panel display; WardrobeMigrationResult gained Status+DidWrite, schema stamped LAST for interrupt-safe recovery; new read-only WardrobeStateAuditor + WardrobeStateSnapshot read RAW PlayerPrefs/key-presence, distinguishing a missing equipped key (clean implicit Classic) from a present-empty value (repairable), future schema flagged non-repairing; editor Jebby Jump/QA/Audit Wardrobe State read-only command (null-safe); parameterized save-compatibility matrix + reset boundary + no-event/status/effective/schema-last tests + duplicate-visual-id guard; Stars/unlocks/acks unchanged; no art/gameplay/economy changes; manual visual QA DEFERRED/NOT VERIFIED)
P20 — Accessibility + Mobile (Landscape) Wardrobe UI Hardening : complete (automated structural layer; game confirmed LANDSCAPE-only, ProjectSettings locked; SafeAreaFitter on wardrobe + ceremony content roots via pure SafeAreaCalculator; pure CanvasScalerMath + WardrobeResponsiveLayout region layout with a COMPACT variant, validated by tests across 16:9/18:9/19.5:9/20:9/4:3 x notch shapes (bounds/non-overlap/touch>=90); rows 64->90 via single-sourced WardrobeLayoutMetrics; deterministic keyboard/gamepad nav + real ceremony focus trap (interactable + re-assert) + scroll-into-view (pure helpers); non-color-only Selected bar; Reduce Motion setting (AccessibilitySettingsStore, jebby.settings.reduceMotion default off) toggle in Main Menu + Pause, freezes preview to Idle, cached/event-driven, reuses settings_changed; YAML scene-integrity; scaffolds idempotent; no gameplay/economy/migration/art-semantic/package changes; manual/device QA DEFERRED/NOT VERIFIED)
P21 — Wider Shell Accessibility + Mobile Navigation Hardening : complete (automated structural layer; extends P20 to Main Menu / Level Select / Settings / Pause / Result / Game Over; new JebbyJump.Shell.Runtime pure helpers (ShellLayoutMetrics single-sources the 90 touch metric, ShellFocusResolver, GridNavigationBuilder true-grid, Shell stack/grid bounds policies per-surface) + ShellFocusUtil + ShellScaffold; deterministic keyboard/gamepad nav + initial focus + real modal traps (pointer backdrop + focus-island re-assert) + focus restore; true Level Select grid nav with focusable no-op locked cards + scroll-to-focus; >=90 hit areas (settings toggle/slider hit-area enlarged without oversizing visuals, slider Left/Right preserved); dedicated GameShellCanvas (shell panels moved off gameplay HUD/MobileControls canvases) - 800x600 SequenceCanvas (memory gameplay) left untouched; result/game-over made modal cards; reuses settings_changed; no gameplay HUD/mobile-control/migration/economy/art changes; manual/device QA DEFERRED/NOT VERIFIED)
```

P32 (production upload-key signing + Play internal-testing upload) ran **PREPARATION ONLY /
BLOCKED** — no upload-key signing performed, no Play Console action, no upload. The gate
confirmed all external prerequisites still missing (no upload keystore, no Console account, no
hosted privacy URL, no listing graphics, no approvals, no tester list, no device). It
**hardened signing to fail-closed** (an unknown `JJ_SIGNING_MODE` refuses to build — never a
debug fallback; `env-upload`/`env_upload` map to Upload intent) and **proved the env-upload
path fails hard** (no keystore → build refused, no AAB, signing config restored). Added pure
`UploadDistributionPolicy` (cert-fingerprint match; "internal-track complete" requires real
Console evidence) + a **five-separate-status** `UploadDistributionReport`
(UploadKey/UploadKeySignedArtifact/PlayAppSigning/PlayConsoleAction/InternalTrackUpload;
`SubmittedInConsole`/`VerifiedInConsole`=false; version code **NotVerified**) with
secret/tester-email/local-path/env-dump guards, and a `Builds/P32` report tool recording env
**presence only** (never values). Docs: upload-signing record, declaration record v0.2,
internal-testing result — all PREPARATION ONLY. Pre-submission cleanup flagged for the unused
IAP/`BILLING` + network permissions. 8 EditMode tests (112→120); PlayMode 333; outfit 49/49.
No gameplay/ProjectSettings/package/art change. Decision: **P32 blocked — see Play distribution
blocker report.**

P31 finalized the **store listing package** (docs only; nothing uploaded/submitted; not legal
advice): draft listing copy, a privacy-policy draft (public URL still missing → Blocked), a
**multi-source** Data Safety worksheet (an aapt2 artifact audit found `INTERNET` +
`ACCESS_NETWORK_STATE` + `BILLING` permissions — engine defaults + the present-but-unused IAP
package — so "no data collected" is a candidate to confirm, never an automatic pass), a
content-rating questionnaire worksheet (rating **NOT predicted**), a **3-tier** graphics
checklist (internal/public/production; launcher/adaptive icon vs Console-listing graphics
separated), a **new Families readiness worksheet** (obligations to verify; compliance **NOT
claimed**), and honest internal release notes. Added pure `StoreGraphicsPolicy` (asset
separation + tiers) + `StoreReadinessPolicy` (placeholders can never pass final readiness),
reusing the P26 `DistributionDeclarationPolicy`; 8 EditMode tests (104→112). Every doc carries
a DRAFT/NOT-SUBMITTED status block + an evidence/sources block; `<PROVIDE>` placeholders only
in blocked/draft sections. Decision: **Store listing package blocked — missing external
content** (privacy URL, support email, listing graphics, adaptive icon external/missing). No
gameplay/ProjectSettings/package/art change.

P28 prepared the long-deferred P4B balance work as an **evidence system for all 10 levels**
— preparation only, **hypotheses/proposals only**, zero asset edits. The user chose: no
human playtest available now → manual balance playtest **NOT RUN**; tuning authority =
proposals only; intended curve = gentle-to-moderate family/casual (a *preparation
assumption*, not a final decision). Added: field-based playtest models
(attempt/level-summary/review-candidate/report), a pure readiness policy (NOT-RUN wording;
heuristic can never authorize tuning; "validated" needs recorded attempts), a
review-candidate catalog (every entry `AuthorizedForApply=false` + `DeferredPendingPlaytest=
true`, **no exact replacement values**), and a read-only `Playtest Kit + Baseline` tool that
emits the all-10-levels baseline (`EvidenceStatus=NotRun`) + a manual CSV template and proves
read-only by hashing LevelConfig + TimeRankConfig + LevelCatalog + the wardrobe/star threshold
sources before+after (`NoHiddenTuningProof`). Progression-economy observations (e.g. the
30/30 wardrobe unlock) are kept **separate** from level-balance candidates. The P26 heuristic
analysis is used as a hypothesis list, not authority. 15 EditMode tests (89→104) incl.
real-asset invariants (all 10 levels have ordered S<A<B rank configs) + a no-hidden-tuning
guard; PlayMode 333; outfit 49/49. No LevelConfig/TimeRankConfig/protected-system change;
balance stays provisional. Real human play (P4B execution) remains the next step (P29D).

P27 (Play internal-testing distribution) ran in **PREPARATION ONLY** mode — no Play
Console account, no real upload keystore, no device — so no upload/console/device action
was performed or fabricated. Decision: **P27 preparation complete — upload requires
authorized external action** (physical install NOT RUN; no production rollout). Delivered:
a dry-run/refuse-by-default `Apply Approved Distribution Config` writer (+ version-code
policy), a `Distribution Readiness Audit` with an independent status model (incl.
`PlayConsoleActionStatus=NotRun`), enumerated missing external items, dated policy
snapshots carrying a `PolicyVerificationStatus`, and a secret/tester-email guard; pure
policies + automated tests (dry-run config, dirty-tree, debug-not-upload-ready, prep-only
claims, external enumeration, console/device-not-claimed, report guards); and the Play
internal-testing runbook + declaration-record worksheets + readiness result docs. The
rebuilt debug-signed AAB is a **regression-gate artifact, NOT upload-ready**. Tests
402 → 422 (EditMode 69 → 89, PlayMode 333); outfit QA 49/49; preflight + AAB gate green.
The tracked tree is genuinely clean (the personal `.gitignore` line was relocated to
`.git/info/exclude`). P25 stays plan-created / deferred / NOT RUN; P4B balance remains
deferred; no gameplay/economy/art/SDK/ProjectSettings change.

P25 (physical Android device QA) was DEFERRED — no device/tester available — and is
honestly recorded **NOT RUN** (no fabricated results). The team pivoted to P26.

P26 added release-distribution-readiness **scaffolding** (no production signing, no
upload, no balance/SDK change, no device QA):
- **Signing:** env-driven Android signing with EXPLICIT intent
  (`JJ_SIGNING_MODE=debug|upload`). Upload intent FAILS HARD on missing/invalid keystore
  vars (never a silent debug fallback); `EnvUploadKeySigned` is an UPLOAD key, NOT the
  Play App Signing key and NOT store-readiness. Every build now verifies the ACTUAL
  artifact signature (apksigner/jarsigner) and restores the signing config
  byte-for-byte. A separate installable APK path (`JJ_BUILD_FORMAT=apk`) is produced
  alongside the Play AAB in separate output dirs/reports with documented
  distribution/signature purposes. `*.keystore`/`*.jks`/`*.p12` are gitignored; no
  secret is ever committed or persisted.
- **Store:** a read-only `Store Compliance Audit` reports CONFIGURED vs RESOLVED target
  SDK (Automatic=0 is a reproducibility flag, not auto-noncompliance; resolved API 36
  PASSes the assumed ≥35), flags a missing adaptive launcher icon (distinct from Console
  listing graphics), and carries DATED/sourced Play-policy assumptions incl. 16 KB page
  support. The submission-readiness doc keeps data-safety as a DRAFT pending
  artifact/SDK/Console review, marks a privacy policy REQUIRED, records target audience =
  mixed (children + older) → Families Policy, and does NOT predict the IARC rating.
- **Balance:** a read-only `Level Difficulty Audit` treats ONLY intra-level S<A<B
  ordering as a hard invariant; cross-level difficulty scoring + proposals are a
  transparent, deterministic, LOW-CONFIDENCE heuristic pending P4B. It hashes every
  LevelConfig/TimeRankConfig before+after to PROVE read-only (verified; 0 invariant
  failures across 10 levels; no asset changed).

Tests 379 → 402 (EditMode 46 → 69, PlayMode 333); outfit QA 49/49; Android AAB build +
preflight remain complete. No gameplay/economy/migration/art/SDK/threshold changes.
Physical device QA (P25) and P4B balance playtest remain DEFERRED / NOT VERIFIED.

P18 added (A) an in-panel outfit preview and (B) wardrobe save migration. The
preview is a UI-only animated pose carousel (idle->run->jump->fall->land->
victory; Hurt excluded; locked outfits dimmed) shown in the existing
SelectedPreview Image, advanced on `Time.unscaledDeltaTime` - completely
separate from the gameplay Jebby. `WardrobePreviewLibrary` was extended from a
single idle sprite to the full per-pose set (`TryGetPreview` still returns Idle,
so P15 rows + the P16 ceremony are unchanged); a pure
`WardrobePreviewSequenceBuilder` + pure `WardrobePreviewPlayer` (safe for empty
sequences, zero/negative dt, and large deltas) hold the logic. Persistence:
`WardrobePersistenceKeys` centralizes the existing keys and adds
`jebby.wardrobe.schemaVersion`; `WardrobePersistenceMigrator.MigrateIfNeeded`
**separates ongoing equipped-id normalization (unknown/empty/locked -> default,
run on EVERY call regardless of schema version) from the one-time schema stamp
(current = 1; a future version is never downgraded)**. Corrections write through
`WardrobeStore` (the low-level primitive), so no appearance event or analytics
fires; Stars, acknowledgements, and thresholds are never touched. Migration runs
at Main-Menu init (before Continue/any gameplay load) and defensively in
Wardrobe.Open. Reset Wardrobe stamps the current schema. 170/170 tests. No new
art, gameplay, economy, or threshold changes. Rendered preview + on-device
migration remain DEFERRED / NOT VERIFIED.

P24 established an automated performance / memory / build-size / lifecycle-stability
baseline (no device; editor/headless regression signals only). Two measured,
behavior-neutral display-only fixes removed per-frame string allocations: the live
HUD timer and the skill-cooldown label now format into a reused StringBuilder via a
zero-alloc TimeFormat helper (TryFormat-based, proven EXACTLY text-equivalent to the
old $"{}" expressions by dense sweep tests). Added: a pure regression policy
(leak/object/steady-state-GC zero-tolerance; timing relative-only), a concrete
static-subscriber-leak probe + analytics-cap + steady-state zero-alloc tests, a full
scene load-to-readiness benchmark, ProfilerMarkers on scene/spawn/swatch paths, and
an editor-only Performance.Editor build-size audit (authoritative compressed AAB
baseline vs fresh-BuildReport packed contributors; outfit sprite PNGs dominate
packed size). The AAB grew +~4.6 KiB (markers + formatters; negligible, ships).
Tests 355 -> 379 (PlayMode 333, EditMode 46); outfit QA 49/49; the Android AAB build
+ preflight remain complete. No gameplay/timing/economy/quality/import/package
changes. Device FPS / battery / thermal / on-device memory + manual visual QA remain
DEFERRED / NOT VERIFIED.

P23 added an automated, **editor-only** release-candidate pipeline for the
Android AAB target: an `Apply Approved Build Config` command (the only writer of
tracked config) sets the approved identity (SparkLibrary / com.sparklibrary.jebbyjump)
and the build scene list from an immutable contract (Boot->MainMenu->Game, replacing
the stale non-existent SampleScene entry); a read-only RC preflight validates
identity/scenes/orientation/input/backend/arch/packages/required-assets (and fails on
drift, never fixing); a deterministic CLI builder builds the AAB (Windows smoke only
when the Android toolchain is unavailable - never masking a real Android failure),
gates post-build warnings, hashes the complete distributable, and always writes an
independent-status report under the ignored Builds/P23. Signing is reported honestly
(debug-signed; production signing + store upload external). The release tooling is in
an Editor-only asmdef (excluded from the player). No gameplay/economy/wardrobe/
migration changes. Manual device/visual/performance/accessibility/balance/art-final/
signing/store verification remain DEFERRED / NOT VERIFIED.

P22 extended the same accessibility/mobile hardening to the ACTIVE gameplay
layer - the HUD (lives/level/timer), the mobile controls (move/jump/3 skills/
pause), and the memory phase - under safe-area roots across the landscape
aspect matrix. A GameplayModalInputGate now blocks AND clears gameplay input
(keyboard/gamepad/touch, with held-state release) under Pause/Result/Game Over
while keeping Pause + UI navigation live. An opt-in "Memory Cues" setting
(default OFF) renders a stable non-color cue (numbers via PlatformCueMapping) on
both the memory swatches and the platforms - the same mapping on both - without
touching sequence order/timing/colors/validation. SequenceCanvas was migrated
off the legacy 800x600 Constant-Pixel scaler to the standard SWSS 1920x1080
(visual/layout-only; render/sorting preserved). No physics/skill/timer/economy/
art changes; rendered/device QA (incl. the SWSS swatch re-check) remains
DEFERRED / NOT VERIFIED.

P21 extended the accessibility/mobile hardening from the Wardrobe to the wider
shell - Main Menu, Level Select, Settings (both entry points), Pause,
Pause->Settings, Level Complete, and Game Over. A shared JebbyJump.Shell.Runtime
layer (pure focus/grid-nav/bounds helpers + the single-sourced 90-unit touch
metric) plus ShellFocusUtil/ShellScaffold drive deterministic keyboard/gamepad
navigation, initial focus, real modal traps (full-screen raycast backdrop +
focus-island re-assert, no gameplay-control changes), and exact focus
restoration. Level Select got true grid navigation (focusable but no-op locked
cards, scroll-to-focus). Verified the Game.unity 800x600 canvas is the
memory-phase SequenceCanvas (gameplay, left untouched, flagged for P22A); the
shell panels were moved onto a dedicated GameShellCanvas, and the centered
result/game-over panels became proper modals. Reduce Motion stays the only
affected motion (no shell transitions). No gameplay HUD/mobile-control/
migration/economy/art changes; rendered/device QA remains DEFERRED / NOT
VERIFIED.

P20 hardened the Wardrobe for landscape mobile + accessibility (no new feature).
The game is landscape-only (ProjectSettings locked; stale "portrait" doc lines
corrected). A safe-area-fitted content root + a pure responsive region layout
(with a compact variant for short landscape screens) are validated by automated
tests across every approved landscape aspect ratio x notch shape (bounds,
non-overlap, 90-unit touch targets). Deterministic keyboard/gamepad navigation,
a real ceremony focus trap (underlying controls non-interactable + focus
re-asserted), and scroll-the-focused-row-into-view use pure, tested helpers.
State stays text-based with a structural Selected marker (non-color-only). A
Reduce Motion setting (Main Menu + Pause settings) freezes the preview carousel
to a static Idle; cached + event-driven, reusing settings_changed. No
gameplay/economy/migration/art-semantic changes; rendered/device QA remains
DEFERRED / NOT VERIFIED.

P19 hardened the wardrobe save subsystem for release (no new feature, no schema
bump). A **future** schema (`> current`) is now **read-only**: migration makes
no normalization/schema/Stars/acknowledgement writes, raises no event/analytics,
and returns `FutureVersionUnsupported`; the game falls back to an **in-memory
Classic** via the new read-only `WardrobePersistenceMigrator.GetEffectiveOutfitId`
(Stars-free for the gameplay player at spawn; Stars-aware for the panel display),
so the future save on disk is never rewritten. `WardrobeMigrationResult` gained
`Status` + `DidWrite`, and the schema is stamped **last** (equipped first) for
interrupt-safe recovery. A new read-only `WardrobeStateAuditor` +
`WardrobeStateSnapshot` read **raw PlayerPrefs + key presence**, distinguishing a
**missing** equipped key (clean implicit Classic) from a **present-empty** value
(repairable under a supported schema), and mark a future schema clearly
non-repairing; an editor-only `Jebby Jump/QA/Audit Wardrobe State` command logs
this (read-only, null-safe). A parameterized save-compatibility matrix + reset
boundary tests were added. Stars/unlocks/acknowledgements unchanged; ownership
stays Stars-derived. Automated release-hardening complete; on-device migration +
rendered QA remain DEFERRED / NOT VERIFIED. See the Wardrobe Save Compatibility
Matrix doc.

P17 added a same-scene live appearance re-sync + safe default restoration. A
single `WardrobeEquipService.TryEquip(id, totalStars)` now backs both equip
sites (normal Equip + ceremony Equip Now), returning Success / AlreadyEquipped
/ UnknownOutfit / Locked; on Success it writes through `WardrobeStore` and
raises `WardrobeAppearanceEvents.EquippedOutfitChanged` (stable ids; never on
the other results, never merely on a PlayerPrefs read). `OutfitVisualApplier`
became a 3-arg method returning a result enum that assigns an override when
present and otherwise RESTORES a captured default controller (NoOp if no
default captured - never assigns null), so switching to Classic (or any
outfit with no override) no longer leaves a stale override.
`PlayerOutfitVisualController` captures the prefab's default `JebbyAnimator` in
Awake, subscribes on OnEnable / unsubscribes on OnDisable+OnDestroy, re-applies
live on the event, and still applies the stored outfit in Start (spawn path).
Missing library/entry falls back to default; a controller swap may restart the
current animation (documented limitation; no state preservation). Reset and
acknowledgement semantics are unchanged; analytics stays one canonical
`cosmetic_equipped` per equip (`source=wardrobe` or `unlock_ceremony`) - the
visual subscriber emits nothing. 144/144 PlayMode tests (incl. editor
integration applying the real Forest->Aqua overrides and restoring JebbyAnimator
on Classic). KNOWN LIMITATION: the wardrobe panel is Main-Menu-only and the
player lives in Game.unity, so the live event has no in-scene subscriber today -
it activates once a wardrobe is reachable in a scene containing a Jebby; the
spawn path already covers scene loads.

P16 added a local outfit unlock ceremony. New `WardrobeUnlockAcknowledgementStore`
(per-outfit PlayerPrefs key `jebby.wardrobe.unlockAcknowledged.<id>`) records which
unlock presentations the player has seen - this is acknowledgement, NOT ownership
(ownership stays derived from Stars; Stars never consumed). Pure
`WardrobeNewUnlockService` returns currently-unlocked-but-unacknowledged outfits in
catalog order (default + locked + acknowledged excluded); pure
`WardrobeCeremonyPresenter` queues them and acknowledges only on Continue / Equip Now
(a failed equip leaves the queue untouched). On Wardrobe open the queued outfits are
shown one at a time in an `UnlockCeremonyOverlay` (dim backdrop blocks the rows, Back
disabled until Continue/Equip); Equip Now uses the existing `WardrobeStore` path
(appearance applies at next spawn). The P15 rows gained a "New" badge for
unlocked-unacknowledged outfits. Reset Wardrobe + Reset Everything clear
acknowledgements (ceremonies replay for currently-eligible outfits); Reset Stars
preserves them. Analytics: `cosmetic_unlock_presented` (on show),
`cosmetic_unlock_acknowledged` (Continue/Equip, with `acknowledgement_action`), and
the existing `cosmetic_equipped` with `source=unlock_ceremony`; never emitted during
queue build / row refresh; wire names pinned. Existing players with Stars see each
eligible ceremony once on first P16 open. 131/131 tests. No new art, no threshold/
gameplay/economy changes. Ceremony rendered appearance is DEFERRED / NOT VERIFIED.

P15 turned the text-only wardrobe into a data-driven preview-card list. A NEW
`WardrobePreviewLibrary` ScriptableObject (kept SEPARATE from
`OutfitVisualLibrary` so P14 invariants stay intact) maps each of the 8
outfits (incl. default) to its existing idle sprite - UI-only, never touching
the player Animator. A pure `WardrobeRowModelBuilder` (+ `WardrobeOutfitRowModel`)
produces per-row view data (state/copy/preview, normalizing the equipped id
internally), so `WardrobePanelController` only renders: each row shows a
thumbnail (locked = dimmed via UI alpha, missing = hidden, `preserveAspect`,
`raycastTarget` off) + name + state, plus a larger selected-outfit preview.
Equip/select/locked/analytics behavior and copy are unchanged. Idempotent
scaffolds (`BuildWardrobePreviewLibrary` + an extended `BuildWardrobePanel`
that wires `_previewLibrary` and a `SelectedPreview` image into MainMenu.unity)
re-run with no duplicate objects. 107/107 PlayMode tests. No art/Animator/
WardrobeStore/UnlockService/Stars/threshold/gameplay/economy changes; outfits
still apply at next spawn (no live re-sync). RENDERED preview-card appearance
(thumbnail readability, dimming, scrolling, selected-preview placement) is
DEFERRED / NOT VERIFIED.

P14 stabilized the 8-outfit wardrobe after P13B. Inspection confirmed the
wardrobe panel was ALREADY structurally ready for 8+ rows (the P9 scaffold
built ScrollRect -> Viewport -> Content with a VerticalLayoutGroup +
ContentSizeFitter, and rows are data-driven from WardrobeCatalog - 8 rows
simply scroll), so no UI refactor was made; visual confirmation of the
scrolling list remains deferred. The gap P14 closed: nothing automated
validated the REAL assets. New `OutfitVisualAssetIntegrityTests` (editor
PlayMode, AssetDatabase under UNITY_EDITOR) now pin: the OutfitVisualLibrary
asset has entries for all 7 non-default outfits (default intentionally has
NO entry - no-op/base JebbyAnimator at spawn); every aoc_jebby_<id> is based
on JebbyAnimator and overrides exactly the 7 expected clips with correctly
named outfit clips; Jebby.prefab's PlayerOutfitVisualController is wired to
Animator/SpriteRenderer/library; and the real forest_cavalier override
applies end-to-end (also documenting spawn-only semantics - re-applying
default mid-scene does not clear the override by design). 94/94 PlayMode
tests pass. No runtime code, prefab, scene, art, or threshold changes.

P13 Mode B (art arrived): the user supplied 7 complete outfit sprite sets
(palette transfer from the approved default poses - PROTOTYPE status, not
final-art-certified). All 49 sprites passed independent validation (exact
default sizes 1122x1402, true RGBA, corners alpha=0) and the editor QA gate
(49/49 after import). The `ImportOutfitVisuals` scaffold (idempotent)
configured import settings to the default-Jebby contract, generated 7
single-frame clips per outfit (settings copied from the default
`Jebby_<State>` clips), built one `aoc_jebby_<id>.overrideController` per
outfit over `JebbyAnimator`, registered all 7 in the new
`OutfitVisualLibrary` ScriptableObject, and wired the library into
`PlayerOutfitVisualController` on `Jebby.prefab`. The static
`OutfitVisualCatalog` stays asset-free; overrides come only from the
serialized library. With user approval the catalog expanded 5 -> 8
(rookie_page 4, crimson_hero 12, pastel_prince 26 - STRICT PLACEHOLDERS;
original five ids/thresholds untouched; ladder ascending 0..30, all
reachable with 10 levels). Equipping any non-default outfit now visibly
changes Jebby at spawn. Animator params/triggers/states, flipX,
PlayerAnimator, and default Jebby assets are untouched; outfits remain
cosmetic-only. 89/89 PlayMode tests pass. Manual visual QA of the new
outfits remains DEFERRED / NOT VERIFIED.

Pre-P13 outfit-agnostic review (maintenance pass, no refactor needed): the
wardrobe/visual code was audited for Forest-Cavalier or "5 outfits forever"
assumptions and found already generic - the only `forest_cavalier` in
production code is the catalog data entry; `OutfitVisualCatalog`/`Applier`/
`PlayerOutfitVisualController` are id-agnostic and any unknown/future id
safely resolves to the default visuals. Jebby remains ONE playable character;
outfits are cosmetic appearance variants only. The current 5-outfit catalog is
the MVP initial runtime set; the 8-design board's extra outfits (Crimson Hero,
Rookie Page, Pastel Prince) stay a documented future pool, NOT runtime (per
the Cosmetic Wardrobe Spec section 6). Three guardrail tests were added:
catalog ids+order pinned, future candidate ids not runtime-visible, and
future/arbitrary ids resolving safely to default visuals. No runtime behavior,
ids, thresholds, display names, or save keys changed.

P13 ran as Mode A (no art available): Forest Cavalier art intake was
attempted but no art exists in the repo or provided working folders, and no
generation source was approved, so nothing was imported and no override was
wired - `forest_cavalier` still looks like default Jebby. The phase added a
read-only editor QA gate for future intake ("Jebby Jump/QA/Check Outfit
Sprite Alpha": Sprite/Single, PPU 100, pivot (0.5,0), Alpha Is Transparency,
transparent corners) verified against the 7 default Jebby sprites. P13 is
BLOCKED ON ART; next step is to provide or externally generate the 7 Forest
Cavalier state sprites per the P12 pack, then run the import phase (Mode B).

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

P7A — Result Panel Stars visual confirmation  [DEFERRED / NOT VERIFIED]
  - "Stars: N/3" (+ "(New Star Best!)") renders correctly on the result panel

P7B — Level Select Stars visual confirmation  [DEFERRED / NOT VERIFIED]
  - per-card "Stars: N/3" readable across locked/unlocked/completed states

P9 — Wardrobe panel visible flow  [DEFERRED / NOT VERIFIED]
  - rows/states/preview/Equip/Back render and behave correctly
    (see Jebby_Jump_Wardrobe_Visual_QA_and_Art_Readiness_Plan_v0.1.md)

P11 — Wardrobe visual application flow  [DEFERRED / NOT VERIFIED]
  - equipped outfit applied on spawn (no-op until art; nothing visibly changes)

P12 — First outfit art pipeline / asset readiness  [DEFERRED / NOT VERIFIED]
  - Forest Cavalier pack not yet exercised; no art exists

P13 — Outfit visual prototypes (7 sets)  [DEFERRED / NOT VERIFIED]
  - art imported + wired (Mode B) but RENDERED appearance never checked:
    per-outfit look in all 7 states, flipX mirroring, state transitions,
    equip->spawn swap, return-to-default - all unverified on screen

P14 — 8-outfit wardrobe stabilization  [DEFERRED / NOT VERIFIED]
  - asset wiring is test-verified, but the RENDERED 8-row scrolling panel
    (row readability, scroll behavior, no overlap) was never checked

P15 — wardrobe preview-card UI  [DEFERRED / NOT VERIFIED]
  - thumbnails/dimming/selected-preview wired + builder test-verified, but
    the RENDERED cards (thumbnail framing, locked dim, SelectedPreview
    placement, mobile readability) were never checked on screen

P16 — wardrobe unlock ceremony  [DEFERRED / NOT VERIFIED]
  - acknowledgement/queue/equip flow + analytics test-verified, but the
    RENDERED ceremony overlay (card layout, preview, queue interaction,
    New badge, Back-disabled behavior) was never checked on screen

P18 — in-panel outfit preview + persistence migration  [DEFERRED / NOT VERIFIED]
  - carousel/sequence/player + migration logic test-verified, but the
    RENDERED animated preview (framing, pose timing, locked dim) and
    on-device save migration were never observed

P20 — wardrobe accessibility / mobile (landscape)  [DEFERRED / NOT VERIFIED]
  - safe-area/layout/nav/focus/reduce-motion logic test-verified, but rendered
    layout on real devices, notch handling, touch comfort, contrast, and
    gamepad/screen-reader UX were never observed

P21 — wider-shell accessibility / mobile (landscape)  [DEFERRED / NOT VERIFIED]
  - shell nav/focus/modal-trap/safe-area/grid logic test-verified, but rendered
    Main Menu / Level Select / Settings / Pause / Result / Game Over layouts,
    notch handling, touch comfort, contrast, and gamepad/screen-reader UX were
    never observed on a device

P19 — wardrobe save/release hardening  [DEFERRED / NOT VERIFIED]
  - audit/matrix/future-read-only/reset logic test-verified, but on-device
    upgrade/downgrade migration was never observed on a real device; the
    editor audit command output was not manually reviewed on-device

P17 — live outfit re-sync + default restoration  [DEFERRED / NOT VERIFIED]
  - equip-service/event/applier/restore logic test-verified (incl. real-
    library integration), but the on-screen live swap + JebbyAnimator
    restoration + any animation restart were never observed (no in-scene
    wardrobe+player today)

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
