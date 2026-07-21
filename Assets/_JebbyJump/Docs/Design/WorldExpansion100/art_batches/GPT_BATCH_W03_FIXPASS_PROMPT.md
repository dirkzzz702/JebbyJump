# Fix-Pass Prompt — Batch W03 (Crystal Caves)

---

Batch W03 structurally validated PASS (all 12 files, exact sizes/paths/alpha),
but several assets fail review. **The hazard and the platform base ARTWORK are
both excellent — keep those designs.**

## BLOCKER 1 — baked index numbers on most assets (affects the whole batch)

Each delivered asset has its **manifest index number burned into the bottom-left
corner** of the image: floor "3", platform base "4", decorations "6", thumbnail
"7", badge "8", gem "11", cosmetic "12" (check all 12 — some are faint).

This is forbidden: "no text baked into any asset". The **platform base is the
worst case** — that sprite is drawn on every platform in every level, so a faint
"4" would tile across the whole game. A grey glyph also defeats the neutrality
check, because grey *is* neutral.

**Fix:** re-export ALL 12 assets with **no index numbers, labels, watermarks,
captions, filenames or annotations anywhere in the image** — including faint,
low-contrast or corner marks. Keep the artwork identical where the asset is
otherwise accepted (see below); this is a clean re-export, not a redesign.

## KEEP THESE DESIGNS (re-export clean, do not redesign)
- `Obstacles/hazard_crystal_spike_cluster_01.png` — **excellent.** Cute cartoon
  crystal cluster with a friendly face and rosy cheeks, exactly the shipped-cactus
  language. Do not change it.
- `Platforms/plat_crystalcaves_base_01.png` — artwork is right and measured
  saturation 0.000 (perfectly neutral). Only remove the baked "4".
- `Decorations/`, `UI/world_thumb`, `UI/world_badge`, `UI/world_gem`,
  `UI/cosmetic_geode_pauldrons`, `UI/story_card`, `UI/finale` — artwork accepted;
  clean re-export only.

## FIX 2 — `Backgrounds/bg_crystalcaves_01.png` (2400x1080, RGB, opaque)

**Rejected: far too cold.** This is a children's game with an explicit warm-palette
mandate, and Crystal Caves came back almost entirely violet-blue.

| Metric | W01 | W02 (accepted) | W03 delivered | Target |
|---|---|---|---|---|
| warm-pixel share (R>=B) | 21% | 88% | **4%** | **>= 45%** |
| mean luma | 177 | 215.9 | 160.7 | >= 150 (met) |
| edge energy | 3.00 | 3.36 | **4.59** | <= 4.5 (just over) |
| luma stddev | 26.1 | 17.3 | 36.3 | <= 40 (met) |

**Regenerate as:** the same crystal cavern, but **warm-lit**. The brief said
"violet + WARM TORCHGLOW" and you delivered violet with two token torches. Invert
the balance: warm amber/honey torchlight should be the DOMINANT light across the
scene, washing the crystal faces and the cave walls in warm tones, with cool
violet surviving only in the deep recesses and far background. Think "candlelit
geode", not "ice cave". Also soften the crystal-facet detail slightly to bring
edge energy under 4.5. Keep luma and stddev where they are.

## FIX 3 — `Floor/floor_crystalcaves_01.png` (512x128, RGB, opaque)

**Rejected: this is not a tileable ground strip.** You delivered a single isolated
rounded platform-island floating in dark void, with a domed top and dark bands at
the top and bottom of the canvas. Repeating it produces a row of separate islands
with gaps, not continuous ground. (It also carries the baked "3".)

**Regenerate as:** a **horizontally tileable ground strip** that fills the full
512x128 canvas edge to edge — no void, no framing, no isolated island. Crystal-
studded cave rock: a **flat, straight, unbroken top surface line** (characters
stand on it) running the entire width, with the rocky/crystal body below filling
to the bottom edge. Left and right edges must match so it repeats seamlessly.
Compare with the accepted W02 floor (`Art/Worlds/W02_EnchantedForest/Floor/`),
which is the correct shape and treatment.

## FIX 4 — `Backgrounds/landmark_tower_crystalcaves_01.png` (900x1400, RGBA)

**Rejected: full-canvas scene blob.** The content fills 100% x 100% of the canvas
and is 0% fully opaque — it is a whole hazy cavern illustration, not a layerable
element.

**Regenerate as:** the **tower alone** — the canonical tiered rainbow-disc spire —
plus at most a wisp of haze. Tight to its content on clean transparency with a
soft feathered edge, occupying roughly the middle 40-60% of the canvas width.
No cavern walls, no crystals, no terrain, no framing. It is a layerable element
that gets composited over a background, not a second illustration.

## Deliver — EXACTLY ONE ZIP (do NOT return loose PNG files)

All 12 assets (clean re-exports + the 3 regenerated) so the batch is
self-consistent:

1. Write all 12 PNGs into `<out>` preserving full repo-relative paths
   (`<out>/Assets/_JebbyJump/Art/Worlds/W03_CrystalCaves/...`).
2. `python ProductionArtAudit/tools/validate_generated_art.py --manifest Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W03_zip_manifest.json --dir <out>`
3. On PASS: `python ProductionArtAudit/tools/build_final_art_zip.py --manifest Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W03_zip_manifest.json --dir <out> --out jebby_art_W03_revA.zip`
4. Deliver the single file `jebby_art_W03_revA.zip` to
   `Downloads/jebby-jump/jebby_art_W03_revA`.

**Zip contents rule:** exactly the 12 manifest PNGs at their manifest paths —
nothing else (no README/report/manifest files inside the zip).

Attach as separate chat attachments: the validation output, the six-tint platform
strip over the NEW background, the new background's warm-share / mean-luma /
edge-energy / stddev, and a **bottom-left corner crop of all 12 assets proving no
baked numbers remain**.

When delivered, STOP.
