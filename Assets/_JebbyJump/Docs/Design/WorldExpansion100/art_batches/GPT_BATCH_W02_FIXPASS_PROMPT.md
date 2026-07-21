# Fix-Pass Prompt — Batch W02 (Enchanted Forest)

---

Batch W02 was received and **structurally validated PASS** (all 12 files, exact
sizes/paths/alpha). **8 of 12 assets are ACCEPTED and must not be touched.**
Four assets need regeneration for identity/readability reasons. Keep everything
else byte-identical — do not "improve" accepted assets.

## ACCEPTED — do not regenerate
`bg` is listed below under FIX, but these 8 are final:
- `Platforms/plat_enchantedforest_base_01.png` — **excellent.** Measured saturation 0.000
  (perfectly neutral) and value range 0.31–0.95. Tints cleanly to all six locked colours.
  This is the standard for every future world's platform base.
- `Decorations/deco_enchantedforest_01.png`
- `UI/world_thumb_enchantedforest_01.png`
- `UI/world_badge_enchantedforest_01.png`
- `UI/story_card_enchantedforest_01.png`
- `UI/finale_enchantedforest_01.png`
- `UI/world_gem_enchantedforest_01.png`
- `UI/cosmetic_leafcrown_hood_01.png`

---

## STYLE RULE that was missing from the original brief (read first)

Jebby Jump uses **two different art languages**, and W02 applied the wrong one to
gameplay objects:

| Layer | Language | Reference |
|---|---|---|
| **Backgrounds / story art** | soft *painted* storybook, atmospheric, low-contrast | `Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_menu_01.png` |
| **Gameplay objects** (hazard, platforms, pickups) | **cute cartoon**: chunky, friendly, clean readable silhouette, warm, often a simple face | `Assets/_JebbyJump/Art/Sprites/Obstacles/spr_cactus_obstacle_01.png` |

The shipped cactus is a smiling, rosy-cheeked, chunky green cactus. That is the
target language for **every world's hazard**, re-themed per world. Painterly,
realistic or menacing gameplay objects are rejected — this is a children's game.

---

## FIX 1 — `Obstacles/hazard_forest_thorn_bloom_01.png` (1254×1254, RGBA)
**Rejected:** delivered as a dark maroon/purple painterly flower with a glowing
core and thorny tendrils. Menacing, realistic, no face, off-palette — wrong art
language and not child-safe.

**Regenerate as:** a **cute cartoon forest thorn bloom** in the exact language of
the shipped cactus — chunky simple silhouette, friendly face (eyes + small smile),
warm and readable, flat-ish cartoon shading with a clean outline. It should still
say "don't touch me" via short rounded thorns/spikes, the way the cactus does with
its orange spots — mischievous, never scary. Greens with warm amber/coral accents;
no dark maroon, no glowing menace, no realistic rendering.
Keep it standing on its base at the bottom of its content area (base-pivoted),
same footprint/scale as the cactus.

## FIX 2 — `Backgrounds/landmark_tower_enchantedforest_01.png` (900×1400, RGBA)
**Rejected:** delivered as a pale grey gothic castle spire. The Rainbow Tower is a
**specific recurring structure** and must be identical across all ten worlds.

**The established design** is visible in `bg_menu_01.png` (right-hand side): a tall
slender spire with **stacked tiered rainbow discs / saucer platforms** in pastel
pink, blue, teal and lilac, with small flags and a bright finial — unmistakably
colourful, not a stone castle.

**Regenerate as:** that same tiered rainbow-disc tower, at the **W02 stage**:
small, hazy, very far, glimpsed through the trees, mostly silhouette — but the
rainbow tiers must still be *identifiable* as the same structure. Framing foliage
is fine. Keep it soft/atmospheric; it must never read as a platform or hazard.

## FIX 3 — `Backgrounds/bg_enchantedforest_01.png` (2400×1080, RGB, opaque)
**Not rejected for beauty — it is a lovely illustration — but it breaks the family
and competes with gameplay.** Measured against the shipped W01 background:

| Metric | W01 (target family) | W02 delivered | Fix target |
|---|---|---|---|
| mean luma (0–255) | 177 | 120 | **≥ 150** |
| edge energy (busyness) | 3.00 | 6.55 | **≤ 4.5** |
| luma stddev | 26.1 | 60.7 | **≤ 40** |

**Regenerate as:** the same enchanted forest, but **calmer and lighter** — push the
mid and far field back with atmospheric haze, reduce leaf/branch detail and
speckle, soften the hard light shafts, and lift overall brightness. Keep the warm
amber glow and the magic (fireflies, spores) but sparser. Detail may stay in the
outer left/right thirds; the **central horizontal band where platforms sit must be
visually calm** so six tinted platforms read instantly. Think "soft painted
backdrop", not "detailed illustration".

## FIX 4 — `Floor/floor_enchantedforest_01.png` (512×128, RGB, opaque)
**Rejected:** delivered as flat vector-style bands — a dark green strip, an olive
strip with cream dots, and a brown band of repeating rounded rectangles. It reads
as placeholder art and clashes with the painted background.

**Regenerate as:** a **painted mossy forest ledge** matching the background's
quality and warmth: soft moss/grass top edge, warm earth-and-root body, gentle
painted shading. **The top surface line must stay flat, straight and unbroken
across the full width** (characters stand on it) and the tile must repeat
seamlessly left↔right.

---

## Deliver — EXACTLY ONE ZIP (do NOT return loose PNG files)

1. Write the 4 regenerated PNGs into `<out>` preserving full repo-relative paths:
```
<out>/Assets/_JebbyJump/Art/Worlds/W02_EnchantedForest/Backgrounds/bg_enchantedforest_01.png
<out>/Assets/_JebbyJump/Art/Worlds/W02_EnchantedForest/Backgrounds/landmark_tower_enchantedforest_01.png
<out>/Assets/_JebbyJump/Art/Worlds/W02_EnchantedForest/Floor/floor_enchantedforest_01.png
<out>/Assets/_JebbyJump/Art/Worlds/W02_EnchantedForest/Obstacles/hazard_forest_thorn_bloom_01.png
```
2. Validate — PARTIAL delivery, so restrict to the 4 ids (without `--ids` it fails
   on the 8 accepted assets that are absent):
```
python ProductionArtAudit/tools/validate_generated_art.py   --manifest Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W02_zip_manifest.json   --dir <out> --ids W02-BG,W02-TOWER,W02-FLOOR,W02-HAZ
```
3. On PASS build the zip with the same ids:
```
python ProductionArtAudit/tools/build_final_art_zip.py   --manifest Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W02_zip_manifest.json   --dir <out> --out jebby_art_W02_revA.zip --ids W02-BG,W02-TOWER,W02-FLOOR,W02-HAZ
```
4. Deliver the single file `jebby_art_W02_revA.zip` to
   `Downloads/jebby-jump/jebby_art_W02_revA`.

**Zip contents rule:** exactly those 4 PNGs at those paths — nothing else. No
`.meta`/prefab/scene/material files, no README/validation/report files inside the
zip (the validator rejects unexpected files).

Attach as **separate chat attachments** (not inside the zip): the validation report,
the six-tint strip over the NEW background, and the new background's measured
mean-luma / edge-energy / stddev.

Do not resend the 8 accepted assets. When delivered, STOP.
