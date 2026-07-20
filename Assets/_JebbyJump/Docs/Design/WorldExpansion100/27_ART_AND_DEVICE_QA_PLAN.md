# 27 — Art & Device QA Plan

Status: `PROPOSED`. Mobile-first; landscape.

## Aspect / device matrix

| Aspect | Example | Checks |
|---|---|---|
| 16:9 | 1920×1080 | background fills; safe areas; UI fits |
| 18:9 | 2160×1080 | bg over-fill (1.5× camera-lock) holds |
| 19.5:9 | notch phones | safe-area insets; controls reachable |
| 20:9 | tall phones | bg width still covers; no letterbox |
| 4:3 landscape | tablet | bg vertical coverage; UI scale |

Plus: notches/safe areas; low-end Android (texture/memory load); colour-blind cue verification;
background contrast; hazard recognition; platform semantic-colour checks; world transitions; story
cards; finale art.

## Per-world art QA

- 6 platform colours distinct at 70px on that world's background.
- hazard reads as hazard on that world's palette.
- tower-landmark stage correct (doc 05); no gameplay obstruction.
- Memory Cues legible; decorations non-colliding.
- world transition: no stale art, no one-frame fallback.

## Camera-lock background verification

The single camera-locked background (doc 08) must cover the widest supported aspect (20:9) with the
1.5× over-fill; verify no uncovered margin shows the clear colour at any supported aspect (the
"faint grey" class of bug, fixed for the menu-art in `a2daa75`, must not recur per world).

## Sign-off

Runs in P34U with the automated aspect/safe-area checks; art acceptance per doc 22.
