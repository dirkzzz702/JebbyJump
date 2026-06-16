# Jebby Jump — Wardrobe Accessibility & Mobile UI Checklist v0.1 (P20)

Structural accessibility + mobile-interaction hardening for the Wardrobe and its
direct entry/overlay surfaces. P20 adds NO new wardrobe feature and changes NO
outfit/reward/migration/gameplay/economy/art semantics. Rendered/on-device
confirmation remains DEFERRED / NOT VERIFIED.

> **P21 update:** the same patterns now extend to the wider shell (Main Menu,
> Level Select, Settings, Pause, Result, Game Over) - see
> `Jebby_Jump_Shell_Accessibility_and_Mobile_UI_Checklist_v0.1.md`. The 90-unit
> touch metric is single-sourced in `ShellLayoutMetrics` (WardrobeLayoutMetrics
> reuses it); the wardrobe's own safe-area/focus/reduced-motion behaviour is
> unchanged.

## Scope

- Main Menu Wardrobe button, Wardrobe panel, runtime outfit rows, selected
  preview, unlock-ceremony overlay, Equip / Back / Equip Now / Continue.
- The Reduce Motion setting (Settings panel) in **both** Main Menu and Pause.
- Out of scope: other screens (broad shell hardening is a later phase), art,
  thresholds, gameplay, economy.

## Supported orientation / resolution

- **Orientation: LANDSCAPE only** (confirmed P20). ProjectSettings locks
  landscape (portrait autorotate disabled; AutoRotation between the two
  landscapes). CanvasScaler unchanged: Scale-With-Screen-Size, reference
  **1920×1080**, match 0.5.
- Approved landscape aspect coverage: 16:9, 18:9 (2:1), 19.5:9, 20:9, 4:3
  (tablet). Logical canvas height ranges ~966 (20:9) to ~1247 (4:3).

## Safe-area policy

- One `SafeAreaFitter` on the wardrobe content root (`SafeArea`) and one on the
  ceremony card root (`CeremonySafeArea`). Dim backdrops stay edge-to-edge;
  only content respects the safe area. No double-padding. Runtime-only (scene
  saved full-stretch); updates on resolution/orientation/safe-area change with
  cheap change-detection (no per-frame rebuild). Pure math: `SafeAreaCalculator`.

## Touch-target policy

- Minimum **90 canvas units** (`WardrobeLayoutMetrics.MinTouchTargetCanvasUnits`,
  ≈48 dp on dense landscape phones at the 1080-height reference).
- Outfit rows ≥ 90 tall; Equip/Back and ceremony buttons ≥ 90. Row preview +
  selection-bar Images are non-raycast so they never block row taps.

## Scroll behavior

- Vertical-only ScrollRect; data-driven Content (VerticalLayoutGroup +
  ContentSizeFitter). Tap vs drag preserved. Keyboard/gamepad focus on a row
  scrolls it into view (`ScrollIntoViewCalculator`). Equip Refresh does not
  reset scroll; ceremony completion rebuilds rows.

## Focus / navigation map

- Explicit navigation (no fragile Automatic on dynamic rows): row ↕ row; last
  row → Equip; Equip ↔ Back; ceremony Equip Now ↔ Continue.
- Initial focus: equipped row (else first row); ceremony → Equip Now if enabled
  else Continue (`WardrobeFocusResolver`).
- Close → focus restored to the opener (Main Menu Wardrobe button).
- Ceremony focus trap: underlying rows/Equip/Back set non-interactable AND focus
  re-asserted to a ceremony control each frame (a real trap, not Navigation.None
  alone). Backdrop blocks pointer.

## State communication (non-color-only)

- Text states: `Equipped` / `Unlocked` / `Locked (N Stars)` / `New`.
- Selected row: a structural left **selection bar** (presence, not hue).
- Locked: dim preview alpha **plus** the `Locked (N Stars)` text. No icon assets
  imported; no emoji assumed.

## Text / readability

- Row labels: no-wrap + ellipsis overflow (no clipping, no sub-readable
  auto-shrink). Long display names produce non-empty model data (tested).

## Reduced motion

- Setting: `AccessibilitySettingsStore.ReduceMotion`
  (`jebby.settings.reduceMotion`, default **false**). Toggle in both settings
  panels; reuses `settings_changed` (`setting_name=reduce_motion`).
- ON → selected-outfit preview stays on a static Idle (no pose cycling, no
  decorative motion); OFF → normal carousel. Cached + event-driven (no per-frame
  PlayerPrefs read); toggling while open re-applies (ON resets to Idle, OFF
  restarts from Idle). Gameplay movement/timers/physics/animations untouched.

## Automated checks (this phase)

- `WardrobeResponsiveLayoutTests`: bounds ⊆ safe area, region non-overlap,
  action touch-target ≥ min, list ≥ min height, compact vs standard — for every
  approved aspect × safe-area shape (full / notch-L / notch-R / home-bottom /
  combined).
- `SafeAreaCalculatorTests`, `CanvasScalerMathTests`, `ScrollIntoViewCalculatorTests`,
  `WardrobeFocusResolverTests`, `WardrobeLayoutMetricsTests`,
  `AccessibilitySettingsStoreTests`, reduce-motion sequence tests,
  `WardrobeRowReadabilityTests`.
- `WardrobeAccessibilitySceneIntegrityTests` (YAML): one SafeArea + one
  CeremonySafeArea; region/scrollRect refs wired; EventSystem present; one
  Reduce Motion toggle wired in both scenes.

## Manual / device matrix (DEFERRED / NOT VERIFIED)

- Rendered layout on each aspect ratio + notch device; touch comfort; contrast;
  gamepad/keyboard feel; screen-reader; reduce-motion visual. None performed.

## Known limitations

- Settings audio + Reduce Motion toggle checkboxes keep the legacy ~60-unit hit
  box (settings-panel touch sizing is a wider-shell concern, not P20 wardrobe
  scope).
- Region layout applies at runtime; the saved scene shows the 1920×1080 seed.
- Editor direct-Play in Game.unity has no wardrobe (Main-Menu-only).

## Pass / fail

- PASS = all automated checks green + scaffolds idempotent (no duplicates).
- P20 may claim **automated structural accessibility/mobile hardening complete**
  only. NOT a claim of: visual approval, notch verified on device, touch comfort,
  contrast/WCAG, gamepad UX, screen-reader. Prototype art not final-certified;
  thresholds remain placeholders; no gameplay/economy changes.
