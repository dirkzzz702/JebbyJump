# 17 — World Art System & Technical Specs

Status: `PROPOSED`; numeric contracts to be **finalised from accepted importers** in P34H. Where a
value cannot be derived from an accepted asset, mark `BLOCKED — TECHNICAL ART DECISION REQUIRED`.

## Derive-from-accepted-art rule

Before authoring import settings, read the importer of a **currently accepted** asset of the same
kind and copy its contract. Reference anchors (REPO-VERIFIED accepted art):

| Kind | Accepted reference | Key values (verify at P34H) |
|---|---|---|
| Background | `Art/Sprites/Backgrounds/bg_menu_01.png` | 2400×1080, PPU 100, Single, RGB opaque |
| Platform 9-slice | `Art/Sprites/UI/ui_btn_pill_9s.png` | PPU 100, 9-slice borders per importer |
| Floor ledge | `Art/Sprites/Platforms/spr_floor_strip_01.png` | 512×128, PPU 100 |
| Hazard | `Art/Sprites/Obstacles/spr_cactus_obstacle_01.png` | 1254², ink-base pivot (FixCactusGrounding) |

## Per-family target contracts (proposed; confirm vs anchors)

| Family | Dimensions | PPU | Mode | Alpha | Pivot | Filter | Wrap | Mips | Max size |
|---|---|---|---|---|---|---|---|---|---|
| Background | 2400×1080 | 100 | Single | opaque | 0.5,0.5 | Bilinear | Clamp | off | 2048* |
| Tower landmark | 900×1400 | 100 | Single | transparent | 0.5,0.0 | Bilinear | Clamp | off | 2048 |
| Floor | 512×128 | 100 | Single | opaque | 0.5,0.5 | Bilinear | Clamp | off | 512 |
| Platform base ×1 | 256×96 | 100 | Single(9-slice) | transparent | 0.5,0.5 | Bilinear | Clamp | off | 256 |
| Hazard | 1254² | 100 | Single | transparent | ink-base | Bilinear | Clamp | off | 2048 |
| Decoration | 1024² | 100 | Single | transparent | 0.5,0.5 | Bilinear | Clamp | off | 1024 |
| UI (thumb/badge/gem/cosmetic) | 512²/192²/256²/512² | 100 | Single | transparent | 0.5,0.5 | Bilinear | Clamp | off | matches |
| Story card | 1600×900 | 100 | Single | opaque | 0.5,0.5 | Bilinear | Clamp | off | 2048 |
| Finale | 1920×1080 | 100 | Single | transparent | 0.5,0.5 | Bilinear | Clamp | off | 2048 |

*Background max-size: `BLOCKED` pending a memory-budget baseline (doc 28); 2048 assumed provisional.

## Atlas grouping

- Per-world env atlas (`<wid>_env`) and per-world UI atlas (`<wid>_ui`) so worlds load/unload as a
  unit. Confirm SpriteAtlas usage matches the repo's current atlas policy at P34H.

## Platform base is TINTED — authoring rule (`REPO-VERIFIED`, P34C)

Each world ships **one** platform sprite, not six. `Platform.ApplyVisualColor()` multiplies it by
the locked semantic colour, so the base must be **near-greyscale / no inherent hue**, mid-value
(avoid pure black or white), carrying only material detail. It must remain readable and
distinguishable under all six tints at ~70 px:
`#E63838 #3878E6 #38BF59 #FAD12E #9938E6 #F28C26`.
A strongly hued base (e.g. a green forest plank) multiplies to mud under red/purple tints.

## Invariants art must not break

- Platform art must not change collider or gameplay width (visual swap only) — test R5 (doc 25).
- Hazard art must keep the cactus footprint/behaviour (ink-base pivot, trigger box) — doc 24.
- Six colours identity-locked; readable at 70px; Memory Cues unaffected.

## Compression / platform overrides

Follow the repo's existing Android/default override policy from an accepted asset; do not invent.
Record final values in the manifest (doc 18) `compression`/`max_texture_size` columns at P34H.
