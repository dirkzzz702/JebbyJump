# Jebby Jump — Manual Test Plan v0.1

Thorough, human-driven test instructions for Jebby Jump. This complements the
automated suites (PlayMode/EditMode tests, outfit QA, release preflight, P24
performance baselines) — it covers what automation **cannot** verify: rendered
visuals, real-device touch/feel, audio, frame smoothness, battery/thermal, and
end-to-end player flows.

> Status: this is an INSTRUCTION document. Executing it is the deferred
> **P25A — Manual Visual + Physical Android Device QA**. Nothing here is
> "verified" until a tester records results.

---

## 1. Scope

In scope: every player-facing flow, control, accessibility feature, persistence,
and subjective performance on landscape Android (primary) + the Unity Editor
(secondary). Out of scope: store submission, monetization, backend (none exist),
and balance tuning (P4B, deferred).

Build identity (P23): product **JebbyJump**, company **SparkLibrary**, id
**com.sparklibrary.jebbyjump**, version **1.0** (code 1). The release-candidate
AAB is **debug-signed** (NOT production-signed) — install via internal/sideload only.

---

## 2. Test environments

Run the full pass on at least one device per row where possible.

| Tier | Example | Why |
|---|---|---|
| Low-end Android | 2–3 GB RAM, older GPU | perf/thermal floor |
| Mid-tier Android | mainstream phone | primary target |
| Tall display | 19.5:9 / 20:9 + notch/punch-hole | safe-area + HUD |
| Short display | 4:3 / 16:9 tablet | layout extremes |
| Unity Editor | 6000.4.7f1, landscape Game view | quick re-checks |

Per device, record: model, OS version, screen size/aspect, refresh rate, RAM.

Orientation: the game is **landscape-only**. Rotating the device must never flip to
portrait.

---

## 3. Setup & reset

1. Install the build (or open the project and press Play with the **Boot** scene, or
   Game/MainMenu directly in editor).
2. To test a clean first-run: clear app data (Android: App info → Storage → Clear
   data) or use the editor menu **Jebby Jump → (reset progress tool)** if present.
3. Confirm default settings on first run: Music/SFX up, Mute off, **Reduce Motion
   OFF**, **Memory Cues OFF**.
4. Have a way to capture screenshots/screen recording for any defect.

Controls reference (verify both touch and, in editor, keyboard/gamepad):

| Action | Touch | Keyboard (typical) |
|---|---|---|
| Move left/right | on-screen Left/Right buttons | A/D or ◄/► |
| Jump | on-screen Jump | Space / W / ▲ |
| Skill 1 / 2 / 3 | on-screen Skill1/2/3 | J / K / L |
| Pause | on-screen Pause (top-right) | Esc |
| UI navigate / confirm | tap | arrows + Enter/Submit |

---

## 4. Test sections

For each step record **PASS / FAIL / BLOCKED** + notes. Severity for fails:
S1 crash/data-loss, S2 broken flow, S3 visual/UX, S4 minor/polish.

### A. Boot & first launch
1. Launch the app. → Boot scene loads, transitions to Main Menu without a hang or
   black screen longer than ~2 s.
2. No error toast, no pink/magenta (missing-shader) elements, no missing-font boxes.

### B. Main Menu
1. All buttons present and readable: Continue, Level Select, Settings, Wardrobe, Quit.
2. Layout sits inside the safe area (not under the notch/home indicator); background
   art may extend edge-to-edge, but text/buttons must be inset.
3. Each button works and routes to the correct screen; Quit exits (device) / stops
   play (editor).
4. Keyboard/gamepad (editor): focus starts on a sensible button; up/down moves focus;
   Submit activates; focus is always visible.

### C. Level Select
1. Grid of levels renders; unlocked levels are tappable, locked levels show a lock
   and are non-interactive (tapping/Submit does nothing, no error).
2. Best time and star rating display on completed levels.
3. Scrolling works (touch drag + keyboard focus scrolls the focused card into view).
4. Back returns to Main Menu with focus restored to the Level Select button.
5. Selecting a level loads the Game scene at that level.

### D. Settings (test from BOTH Main Menu and Pause)
1. Music slider changes music volume live; SFX slider changes SFX volume live (trigger
   a SFX to confirm).
2. Mute toggle silences all audio; un-mute restores.
3. **Reduce Motion** toggle — see section M3.
4. **Memory Cues** toggle — see section M4.
5. Reset Defaults restores all of the above to defaults.
6. Sliders adjustable by drag (touch) and Left/Right (keyboard) without oversized
   handles; toggles have a comfortable tap area.
7. Close/Back returns to the opener (Main Menu or Pause) with focus restored and,
   from Pause, the game still paused.
8. **Persistence:** change settings, fully close + relaunch the app → settings retained.

### E. Wardrobe
1. Outfit rows render (catalog has multiple outfits); each shows its name + lock/owned
   state and Stars requirement.
2. Selecting an outfit updates the preview (Jebby shown wearing it / placeholder if
   art is prototype — prototype art is NOT final-certified).
3. Equip applies the outfit; it persists after closing/reopening Wardrobe and after an
   app restart.
4. Unlock ceremony (when a newly-eligible outfit is unlocked) plays and can be
   dismissed; focus is trapped in the ceremony until dismissed.
5. The preview pose carousel animates (Reduce Motion OFF) — see M3.
6. Closing the Wardrobe clears the preview and restores Main-Menu focus.

### F. Gameplay — memory "show" phase
1. Entering a level shows a sequence of colored gem swatches for a few seconds, then
   "Go!".
2. During the show phase the player cannot score (skills disabled, reduced jump);
   movement may be limited as designed.
3. The sequence length / timing matches the level (do not assume — just confirm it is
   consistent run-to-run for the same level).
4. With **Memory Cues OFF**: swatches differ by color only.
5. With **Memory Cues ON**: each swatch shows a stable number (1–6) in addition to its
   color (see M4).

### G. Gameplay — play phase
1. After "Go!", the timer (top-right) starts counting up.
2. Move left/right and jump feel responsive (touch + keyboard); Jebby lands on
   platforms.
3. Land on platforms in the shown color order: correct landing advances the sequence
   ("Correct!"); wrong color or skipping a row loses a life ("Wrong color!").
4. Cactus hazard: hitting it loses a life ("Ouch! Cactus!").
5. Lives deplete to Game Over; completing the full sequence shows Level Complete.
6. Skills (if equipped) fire on their buttons, show a cooldown countdown in the slot,
   dim while cooling/blocked, and respect the cooldown (cannot spam).
7. The live timer reads as MM:SS.SS and updates smoothly (no stutter/jitter from the
   timer text specifically — P24 made it allocation-free).

### H. HUD
1. Lives, level label, and live timer are visible, readable, and inside the safe area.
2. The Pause button (top-right) does not overlap the timer or the skill controls on
   any aspect ratio / notch.
3. Mobile controls (left cluster: move; right cluster: jump + 3 skills) are inside the
   safe area with comfortable spacing; nothing is clipped by a notch/home indicator.

### I. Pause
1. Pause freezes gameplay (Jebby + timer stop; `Time.timeScale` = 0).
2. Pause panel: Resume, Restart, Settings, Main Menu — all work.
3. Resume continues exactly where paused (no lurch/teleport).
4. Restart restarts the level (new memory phase); Main Menu returns to the menu.
5. Settings-from-Pause opens settings over the pause (still paused) and returns to
   Pause on Back.

### J. Level Complete (Result)
1. Shows clear time, best time (new-best highlighted), star rating, and rank.
2. Buttons: Next (if not final level), Retry, Menu — all route correctly.
3. Rank/best are conveyed by **text**, not color alone.

### K. Game Over
1. Shows on life depletion; Retry restarts the level, Menu returns to the menu.

### L. Modal input gating (P22 — critical)
For Pause, Level Complete, and Game Over, while the panel is up:
1. The on-screen mobile controls are non-interactive (taps do nothing / visibly
   disabled) and do NOT move Jebby.
2. Keyboard/gamepad gameplay input (move/jump/skill) does nothing; only the panel's
   UI navigation responds.
3. **Held control on open:** hold Left (or Jump), then trigger the panel (e.g. let a
   life deplete / open pause) — Jebby must not keep moving/jumping; the held input is
   released. On resume, no "stuck" movement.
4. The Pause key/Esc still works where appropriate; UI focus stays trapped in the
   panel (cannot focus underlying controls).

### M. Accessibility
1. **Safe area:** on a notched/punch-hole device, no text/button/control sits under
   the cutout or the home indicator, in every screen (menu, settings, wardrobe, HUD,
   controls, memory swatches).
2. **Touch targets:** all interactive controls feel at least ~48 dp; no tiny/hard-to-hit
   buttons.
3. **Reduce Motion:** turn ON → the Wardrobe pose carousel stops / reduces animation;
   required gameplay feedback ("Go!/Correct!/Wrong!") is NOT suppressed. Turn OFF →
   carousel animates again.
4. **Memory Cues (colorblind):** turn ON → the memory swatches AND the in-world
   platforms show a stable number (1–6) paired with each color; the SAME number maps
   to the SAME color on both swatch and platform. Confirm you can complete a level
   using the numbers alone (e.g., with a colorblind simulation). Turn OFF → numbers
   disappear, color-only. The sequence/timing/difficulty must be identical either way.
5. **Keyboard/gamepad:** every menu + modal is fully operable without touch; focus is
   always visible and never escapes a modal.

### N. Orientation & display
1. Rotate the device through all orientations → game stays landscape.
2. Resize (foldable / split-screen if supported) → UI re-fits safe area without
   clipping or stretching.

### O. Audio
1. Music plays in menus + gameplay; SFX on correct/wrong/skill/cactus.
2. Volume sliders + mute behave; settings persist across restart.
3. No clipping, no looping pops, no audio continuing after Quit.

### P. Performance (device — the P24-deferred items)
> P24 established automated regression baselines only; the following require a device
> and are DEFERRED / NOT VERIFIED until run here.
1. Frame smoothness: target 60 FPS (30 FPS minimum acceptable). Watch for hitches on
   scene load, level start (memory phase + platform spawn), pause/resume, and result.
2. No growing slowdown over an extended session (see R).
3. Battery drain + device temperature over ~15–30 min of continuous play (note only;
   no certification claim).
4. Memory: no crash/OOM over a long session on a low-end device.

### Q. Persistence
1. Settings, level progress/best times/stars, and equipped outfit all survive a full
   app close + relaunch.
2. No corruption after force-kill mid-level (relaunch returns to a sane state).

### R. Stability / soak (subjective leak check)
1. Play/retry ~20+ levels and open/close Settings + Wardrobe + Pause ~20 times each.
2. Watch for: progressive slowdown, rising memory, duplicated UI (e.g., doubled
   wardrobe rows or memory-cue labels), audio buildup, or input becoming unresponsive.
3. Return to Main Menu and back into a level repeatedly → load time stays stable.

---

## 5. Defect reporting template

```
Title:
Severity: S1 / S2 / S3 / S4
Device / OS / aspect / refresh:
Build version (+ AAB SHA if known):
Setting state (Reduce Motion / Memory Cues / Mute):
Steps to reproduce:
Expected:
Actual:
Frequency: always / sometimes / once
Screenshot / video:
```

---

## 6. Sign-off

| Area | Device | Result | Tester | Date | Notes |
|---|---|---|---|---|---|
| A–E shell |  |  |  |  |  |
| F–H gameplay/HUD |  |  |  |  |  |
| I–L pause/result/gating |  |  |  |  |  |
| M accessibility |  |  |  |  |  |
| N–O orientation/audio |  |  |  |  |  |
| P performance |  |  |  |  |  |
| Q–R persistence/soak |  |  |  |  |  |

A release-readiness pass requires no open S1/S2 and a documented decision on any S3.
Prototype outfit art, reward thresholds, and gameplay balance remain provisional and
are out of this pass's authority.
