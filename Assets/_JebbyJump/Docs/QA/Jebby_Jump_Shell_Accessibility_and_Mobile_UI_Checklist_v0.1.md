# Jebby Jump — Shell Accessibility & Mobile UI Checklist v0.1 (P21)

Structural accessibility + mobile (landscape) hardening for the non-gameplay
shell, extending the P20 wardrobe patterns. No gameplay/economy/wardrobe-semantic
changes. Rendered/on-device confirmation remains DEFERRED / NOT VERIFIED.

> P23 note: the automated release-candidate preflight
> (`Docs/Release/Jebby_Jump_Release_Candidate_Checklist_v0.1.md`) pins the
> landscape-only + new-Input-System invariants and the Boot→MainMenu→Game scene
> contract these shell scenes rely on. Manual rendered/device shell QA stays
> DEFERRED / NOT VERIFIED.

## Scope (and excluded gameplay surfaces)

In scope: Main Menu, Level Select, Settings (Main Menu + Pause), Pause,
Pause→Settings, Level Complete, Game Over.

**Excluded gameplay surfaces (NOT changed):** HUD timer/lives/level text,
the Pause button, mobile movement controls, the skill button, and the
memory-phase colour display (SequenceCanvas). The Pause modal traps focus
without modifying these.

## Supported orientation / aspect

Landscape-only (1920×1080 Scale-With-Screen-Size, match 0.5). Validated aspects:
16:9, 18:9 (2:1), 19.5:9, 20:9, 4:3 — with full / notch-left / notch-right /
home-indicator / combined safe areas.

## Canvas architecture (P21 finding)

Game.unity had shell panels sharing gameplay canvases (LevelComplete/GameOver on
HUDCanvas; Pause/Settings on MobileControlsCanvas). The 800×600 Constant-Pixel
canvas is **SequenceCanvas** — the memory-phase gameplay display (out of scope,
**left unchanged**; flagged for P22A). P21 creates a dedicated **GameShellCanvas**
(1920×1080 SWSS, sorted above gameplay) and moves the four shell panels onto it,
leaving all gameplay canvases untouched.

## Safe-area policy

Full-screen dim/blocking backdrops stay edge-to-edge; interactive/text content
sits inside a `SafeArea` (SafeAreaFitter) root. Applied to Main Menu button
stack (MenuSafeArea), Level Select, Settings (both), Pause. Result / Game Over
use a centred modal **Card** (700×400) over a full-screen backdrop — inherently
within safe bounds (no fitter needed on a centred card). Reuses the P20
`SafeAreaFitter` + `SafeAreaCalculator`; no double-fitting.

## Touch-target standard

≥ **90 canvas units** (`ShellLayoutMetrics.MinTouchTargetCanvasUnits`, single
source; `WardrobeLayoutMetrics` reuses it). All shell buttons sized to ≥90.
Settings toggles/sliders get a ≥90 **hit area** with the visible checkbox/track
kept small (DefaultControls fixed-anchor checkbox; slider track/handle in a thin
centred band). Slider Left/Right value adjustment preserved.

## Panel-by-panel focus map

- **Main Menu:** Continue→Level Select→Settings→Wardrobe→Quit; initial focus =
  first interactable.
- **Level Select:** true grid nav (`GridNavigationBuilder`, incl. partial last
  row); bottom row → Back; initial focus = continue/current level (else first);
  focused card scrolls into view; **locked cards focusable but activation is a
  no-op (no analytics)**; Back restores the Main Menu Level Select button.
- **Settings:** Music→SFX→Mute→Reduce Motion→Reset→Back; initial focus = Music;
  Back restores the opener (Main Menu or Pause Settings button).
- **Pause:** Resume→Restart→Settings→Main Menu; initial focus = Resume; user
  Resume restores the opener; run-end defers focus to the result panel.
- **Level Complete:** Next (if not final) → Retry → Menu (first available).
- **Game Over:** Retry → Menu.

## Modal focus-trap rules

Every modal (Pause, Settings, Level Complete, Game Over) blocks **pointer**
(full-screen raycast backdrop) **and navigation** (explicit-nav island + focus
re-asserted into the modal each frame), preserves a valid internal focus, and
restores prior focus exactly on close. Underlying gameplay controls are not
modified; Pause keeps `Time.timeScale = 0` + `PauseState`.

## Text / readability + non-color state

State is conveyed by text, not colour alone: Level Select uses a structural
`LockedOverlay` + best-time/Stars text (completion shown by best-time/Stars);
rank uses colour **and** text; result panel shows New-best/Stars text. Critical
result/pause copy is centred in a bounded card.

## Reduced motion

Reuses `AccessibilitySettingsStore.ReduceMotion` (`jebby.settings.reduceMotion`)
from P20, identical in Main Menu + Pause Settings (`settings_changed`,
`setting_name=reduce_motion`). The shell has **no transition animations**, so the
only affected motion is the Wardrobe pose carousel (freezes to Idle); P21 invents
no new transitions. Focus/selection changes are immediate.

## Automated checks

- Pure: `ShellFocusResolver`, `GridNavigationBuilder`, `ShellStackLayoutPolicy` +
  `ShellGridLayoutPolicy` (per-surface landscape×notch bounds), `ShellLayoutMetrics`.
- Scene-integrity (YAML): one EventSystem + InputSystemUIInputModule per scene;
  one GameShellCanvas; result/game-over modal Cards; MenuSafeArea; SequenceCanvas
  left unchanged.
- Settings consistency (same set/keys/defaults/labels/analytics in both panels).

## Manual / device matrix (DEFERRED / NOT VERIFIED)

Rendered layout per aspect + notch device; touch comfort; contrast; gamepad/
screen-reader UX; result/pause layouts. None performed.

## Known limitations

- Region layout/safe-area/focus never visually verified on devices.
- SequenceCanvas (memory-phase, 800×600 Constant-Pixel) is a gameplay surface,
  deferred to P22A.
- Settings slider track thinning is structural-only (unverified visually).

## Pass / fail

PASS = all automated checks green + scaffolds idempotent (no duplicates). P21 may
claim **automated wider-shell accessibility/mobile hardening complete** only.
Not a claim of device/visual/contrast/gamepad/screen-reader approval. Prototype
art not final-certified; P4B balance deferred.
