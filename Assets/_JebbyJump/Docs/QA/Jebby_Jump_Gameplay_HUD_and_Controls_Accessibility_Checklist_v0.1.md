# Jebby Jump — Gameplay HUD & Controls Accessibility Checklist v0.1 (P22)

Structural accessibility + mobile (landscape) hardening for the **active gameplay
layer** — the HUD, the mobile touch controls, and the memory phase — extending the
P20/P21 patterns. No gameplay rules, physics, balance, timing, level content,
answer validation, or input semantics changed. Rendered/on-device confirmation
remains **DEFERRED / NOT VERIFIED**.

## Scope

In scope: gameplay HUD (lives / level / live timer + feedback on HUDCanvas), the
mobile controls (Btn_Left/Right, Btn_Jump, Btn_Skill1/2/3, PauseButton on
MobileControlsCanvas), and the memory phase (SequenceCanvas swatches + the spawned
colored platforms).

Out of scope (NOT changed): PlayerMotor physics/jump, skill cooldown/effect math,
`ColorSequenceManager`/`MemoryPhaseController` sequence order/timing/validation,
`LevelTimer` semantics, lives rules, LevelConfig/TimeRankConfig/StarReward,
Wardrobe, Jebby.prefab, PlayerAnimator/JebbyAnimator, audio, input actions /
control scheme.

## Supported orientation / aspect

Landscape-only (1920×1080 Scale-With-Screen-Size, match 0.5). Validated aspects:
16:9, 18:9 (2:1), 19.5:9, 20:9, 4:3 — with full / notch-left / notch-right / top
inset / home-indicator / combined safe areas (see `GameplayLayoutPolicyTests`).

## Safe-area policy

HUD content, the mobile controls, and the memory `SequencePanel` each sit inside a
`SafeArea` (SafeAreaFitter) root (reuses P20 `SafeAreaCalculator` /
`ShellLayoutMetrics`; no duplicated safe-area math). Control sizes and relative
cluster positions are preserved; the inset comes from the safe-area root. The
top-anchored memory swatch row now respects the top inset.

## Touch targets

All controls remain ≥ 90 canvas units (the shell minimum) — never shrunk:
Btn_Left/Right 130, Btn_Jump 140, Btn_Skill1/2/3 100, PauseButton 96. The
combined cross-canvas check (`GameplayLayoutPolicy`) asserts the PauseButton clears
the Timer and the skill controls, and that the HUD/control clusters do not collide,
across the full aspect/safe-area matrix.

## Modal input gating (block AND clear)

`GameplayModalInputGate` (on the always-active HUDCanvas) blocks gameplay input
whenever a shell modal is active — Pause, Pause→Settings (via `PauseState`), Level
Result, or Game Over (via the explicit panel active-state, not the GameShellCanvas
object). It:

- disables the mobile-control `CanvasGroup` (pointer / raycast), and
- calls `InputReader.SetGameplayInputEnabled(false)`, which ignores move/jump/skill
  for keyboard, gamepad, AND on-screen touch, and **clears any held state** (zeroes
  Move, releases a held jump) so a control held when a modal opens cannot stick
  "on" or lurch on resume.

The Pause action and the UI input module / navigation stay active. Decision logic
is pure + tested (`GameplayInputBlockPolicy`).

## Readability / non-color

Lives = icon count / text; skill cooldown = overlay **+** numeric label; timer /
level = text — already non-color. The memory phase was the only color-only surface.

## Memory cues (opt-in, default OFF)

A new **Memory Cues** accessibility setting (`AccessibilitySettingsStore.MemoryCues`,
default OFF, in both Settings panels, included in Reset Defaults, analytics
`setting_name=memory_cues`) renders a stable non-color cue (numbers 1–6 via
`PlatformCueMapping`, no new art) on BOTH the presentation swatches and the spawned
platforms — the same mapping on both so presentation and response always match.
Validation stays `platform.Color == ExpectedColor`; sequence order, timing, and
colors are unchanged. Platform cue labels are world-space `TextMeshPro` (sorted
above the sprite, no collider / no input, counter-scaled for the platform's
non-uniform scale, safely unsubscribed). Swatch labels are idempotent (rebuilt per
sequence; subscribed once).

## SequenceCanvas migration

SequenceCanvas was migrated off the legacy 800×600 Constant-Pixel scaler to the
standard SWSS 1920×1080 (match 0.5) so the swatches + cue labels scale legibly on
high-DPI phones. Visual/layout-only: RenderMode and sorting are unchanged (it stays
below GameShellCanvas); swatch proportions / relative positions preserved; the
on-screen swatch SIZE changes and needs a rendered re-check (deferred).

## Reduced motion

No cosmetic HUD motion exists to gate; the "Go!/Correct!/Wrong!" feedback is
required gameplay feedback and is NOT suppressed.

## Automated checks

- Pure: `PlatformCueMappingTests`, `GameplayInputBlockPolicyTests`,
  `GameplayLayoutPolicyTests` (aspect/safe-area matrix incl. Pause-vs-Timer/skill),
  `AccessibilitySettingsStoreTests` (Memory Cues persistence/event/reset).
- Scene-integrity (Game.unity): one `GameplayModalInputGate`; SequenceCanvas
  converted off 800×600; 5 SafeArea roots; all SafeAreaFitter targets wired.
- Idempotency: `BuildSettingsPanel` + `BuildGameplayAccessibility` run twice with no
  second-run scene diff.

## Manual device matrix (DEFERRED / NOT VERIFIED)

On real landscape phones across the aspect/notch matrix, confirm: HUD + controls +
pause sit inside the safe area with no overlap; touch targets feel ≥ 48 dp; controls
are inert + cleared under Pause/Result/Game Over while UI nav works; Memory Cues ON
shows legible matching cues on swatches AND platforms; SequenceCanvas SWSS swatch
size/legibility looks correct; default (cues OFF) is unchanged.

## Known limitations

- Memory Cues default OFF (opt-in); color-only is still the default experience.
- Cue contrast color is luminance-derived (black/white); not yet outlined.
- All rendered/on-device verification is DEFERRED / NOT VERIFIED.
