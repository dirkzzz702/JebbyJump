# 28 — Performance & Memory Budget

Status: `BLOCKED — MEASURED BASELINE REQUIRED`. Do **not** invent final budgets. Measure current
`Game` scene first (P34U), then set budgets from the baseline.

## Measure first (baseline capture in P34U)

Capture on a representative low-end Android + editor profiler for the current single-world scene:
- background texture memory; active sprite count; sprite atlas memory; particle count; material
  count; draw calls; Canvas rebuilds/frame; scene load time; world-theme application time;
  GC allocation/frame; Android build size; low-end FPS; thermal/battery over a session.

## Budget approach (fill after baseline)

| Metric | Baseline (measure) | Budget (baseline-derived) |
|---|---|---|
| Background texture memory / world | TBD | ≤ baseline (one world loaded at a time) |
| Per-world env atlas | TBD | TBD |
| Draw calls (gameplay) | TBD | ≤ baseline + small margin |
| World-theme apply time | n/a (new) | ≤ 1 frame budget; no visible hitch |
| Scene load time | TBD | ≤ baseline (worlds don't add scenes) |
| GC alloc during play | TBD | no per-frame alloc from theming |
| Android build size | TBD | track growth per world art batch |
| Low-end FPS | TBD | ≥ baseline target |

## Key design levers that protect budget

- **One world loaded at a time** (only the current world's env/UI atlases resident); worlds
  load/unload as a unit (doc 17 atlas grouping).
- **No parallax at launch** (Gate #6) — single camera-locked background per world.
- Max texture sizes capped (doc 17); background max-size confirmed against the memory baseline.
- Theming is a one-time apply at level start (doc 08), not per-frame.

## Gate

Numeric budgets are `BLOCKED` until the P34U baseline exists. Art max-sizes in doc 17 flagged
`provisional` inherit this block for the background family.
