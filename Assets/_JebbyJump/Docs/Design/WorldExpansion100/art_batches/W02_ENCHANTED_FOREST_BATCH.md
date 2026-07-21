# ChatGPT Art Batch — W02 Enchanted Forest (paste-ready)

> Deliver **PNG files only**. Never produce `.meta`, prefab, scene, material,
> animation controller, SpriteAtlas or ScriptableObject files.
> Deliver as a zip to: `C:\Users\dontb\Downloads\jebby-jump\jebby_art_W02`

---

## Project context (locked — do not reinterpret)

**Jebby Jump** is a mobile-first, **landscape** memory-platformer for **kids**.
Jebby memorises a colour sequence, then climbs platforms landing on the correct
colour for each row. Art direction is **warm, soft, storybook, high-readability**
— think gentle painted pastel fantasy. It must match the existing accepted art
(warm cloud/rainbow key art, gold-rimmed UI, chibi knight character).

**Hard rules for every asset**
- Child-safe. Nothing scary, dark, gory, or threatening.
- **Warm palette bias.** Avoid cold, grey, washed-out results.
- No text baked into any gameplay art.
- No characters (Jebby is added separately) unless the asset explicitly asks.
- Leave crop-safe margins; nothing important within 5% of any edge.
- Exact canvas size and exact filename as specified — no variations, no suffixes.

## This world: **W02 — Enchanted Forest**

- **Fantasy:** a glowing woodland of mossy boughs.
- **Mood:** cozy, magical, verdant.
- **Material:** mossy wood & leaves.
- **Background palette:** deep green + amber. **Support:** teal + gold.
- **Lighting:** dappled canopy light (warm shafts through leaves).
- **Atmosphere:** floating spores, drifting fireflies.
- **Recurring landmark:** the **Rainbow Tower**, seen *glimpsed through the trees* —
  small, hazy, very far, silhouette-only. (It reappears larger/closer in later worlds,
  so keep it clearly the *same* structure: a tiered rainbow-crystal spire.)

---

## Assets to produce (12)

### 1. Background — `bg_enchantedforest_01.png`
- **2400 × 1080**, RGB, **opaque** (no alpha).
- Full-bleed enchanted-forest sky/canopy environment. Warm dappled light.
- Must still read well when the camera shows only the **middle ~65%** horizontally
  (it is camera-locked and over-scaled), and across 16:9 → 20:9.
- No UI, no debug elements, no characters, no text.

### 2. Distant Rainbow Tower — `landmark_tower_enchantedforest_01.png`
- **900 × 1400**, RGBA, **transparent** background.
- The tiered rainbow-crystal tower, **glimpsed through trees**: small, hazy, distant,
  mostly silhouette. Soft edges. Must never look like a platform or a hazard.

### 3. Start ledge / floor — `floor_enchantedforest_01.png`
- **512 × 128**, RGB, opaque, horizontally tileable.
- A mossy forest ledge. The **top surface line must be flat and clearly readable**
  (characters stand on it). Keep the top edge consistent across the width.

### 4. Platform base — `plat_enchantedforest_base_01.png`  ← **read carefully**
- **256 × 96**, RGBA, transparent outside the platform shape, **9-slice safe**
  (uniform left/right ends, stretchable middle).
- **CRITICAL:** this sprite is **multiplied by a colour at runtime.** It must be
  **NEUTRAL / near-greyscale, light-to-mid value**, carrying only *material detail*
  (moss texture, bark grain, soft top-light, gentle rounded edge).
- **Do NOT give it its own strong hue.** No green tint, no colour cast.
- It must look correct when tinted to each of these exact colours:
  `#E63838` red · `#3878E6` blue · `#38BF59` green · `#F9DE2C` yellow ·
  `#9938E6` purple · `#E58A1B` orange.
- Keep the silhouette simple and the value range mid (avoid pure black/white),
  so every tint stays bright and distinguishable at ~70 px on screen.

### 5. Hazard — `hazard_forest_thorn_bloom_01.png`
- **1254 × 1254**, RGBA, transparent.
- A **forest thorn bloom**: the Enchanted-Forest version of a spike hazard.
- Must read **unmistakably as a hazard** (spiky, "don't touch") and must **never**
  be mistakable for a safe platform. Still child-friendly, not gory.
- Draw it standing on its base; the visible ink should sit at the bottom of the canvas
  area it occupies (it is pivoted at its base).

### 6. Decoration set — `deco_enchantedforest_01.png`
- **1024 × 1024**, RGBA, transparent.
- A sheet of non-colliding forest props (ferns, mushrooms, glowing berries, vines),
  visually separated with clear spacing so they can be cut apart.

### 7. World thumbnail — `world_thumb_enchantedforest_01.png`
- **512 × 512**, RGBA. Level-select identity image for the world. Must be recognisable
  at **128 px**. Iconic, centred composition.

### 8. World badge — `world_badge_enchantedforest_01.png`
- **192 × 192**, RGBA. A simple emblem/crest for the world card (leaf-and-spire motif).
  Readable at 64 px.

### 9. Story card illustration — `story_card_enchantedforest_01.png`
- **1600 × 900**, RGB, opaque. A warm "magical travel postcard" of the forest with the
  distant tower visible through the trees.
- Keep the **lower third visually calm** — text is overlaid there (do not bake text).

### 10. Finale treatment — `finale_enchantedforest_01.png`
- **1920 × 1080**, RGBA, transparent where empty.
- A celebratory set-piece framing for the world's final level: a **great glowing tree**
  archway. Celebratory, warm, child-safe. No text.

### 11. World Gem trophy — `world_gem_enchantedforest_01.png`
- **256 × 256**, RGBA. A forest-themed collectible gem/trophy icon (emerald-and-gold,
  leaf motif). It is a **collection marker, not currency** — no coin/cash imagery.

### 12. Cosmetic reward — `cosmetic_leafcrown_hood_01.png`
- **512 × 512**, RGBA. Presentation art for the **"Leafcrown hood"** cosmetic —
  the item shown on its own (not worn), like a wardrobe preview.

---

## Delivery & acceptance

Zip all 12 PNGs (flat, no subfolders) to
`C:\Users\dontb\Downloads\jebby-jump\jebby_art_W02`.

They will be checked for:
1. **Exact** dimensions + filenames.
2. Correct opaque vs transparent per asset.
3. Platform base: near-neutral hue, and still readable when tinted to all six locked colours at 70 px.
4. Hazard reads as a hazard, never as a safe platform.
5. Background covers 16:9 → 20:9 with no cold/grey wash.
6. Tower is the same structure as other worlds, at the "distant, through trees" stage.
7. Child-safe, warm, no baked text.

If any asset fails, only that asset is regenerated — keep the others stable.
