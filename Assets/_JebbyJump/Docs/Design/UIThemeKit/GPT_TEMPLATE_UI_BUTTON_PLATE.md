# Template — Single Main-Menu Button Plate (reuse for new/adjusted buttons)

Use this when you need to **add or replace one menu button** (e.g. a future
"Shop", "Daily Gift", or "Friends" button) so it matches the existing bespoke
plates from batch UI02. Fill in the `{{...}}` fields, paste into ChatGPT, and
attach the main-menu mockup + one accepted plate (e.g. `ui_plate_settings.png`)
as the style reference. Produces ONE plate.

---

## Fill these in
- **Button name:** `{{BUTTON_NAME}}`  (e.g. Shop)
- **File name:** `ui_plate_{{slug}}.png`  (e.g. `ui_plate_shop.png`, lowercase, no spaces)
- **Frame kind:** `{{FRAME}}`  → `golden/honey (primary, for the main action)` OR `cream/ivory (secondary)`
- **Icon:** `{{ICON}}`  (a cute-cartoon object, e.g. "a little wooden market stall")
- **Decorations:** `{{DECOR}}`  (e.g. "cloud puffs at the bottom corners; a small vine with a flower on the right; two tiny stars")

---

## Paste-ready prompt

You are producing ONE production UI button plate for the Unity game **Jebby Jump**,
matching the existing bespoke menu plates (batch UI02). Reproduce the established
style exactly. Attached: the main-menu mockup + an accepted plate for reference.

**Deliverable:** a single PNG `Assets/_JebbyJump/Art/Sprites/UI/ui_plate_{{slug}}.png`,
**1280 x 384 px, RGBA, transparent.**

**Plate spec (identical to the other plates — HARD):**
- The button **frame** is a clean **rounded rectangle** (corner radius ~60, soft
  warm-brown outline), ~**1040 wide x 208 tall**, horizontally + vertically
  centred (spans ~x120-1160, y88-296). **Smooth ends — NO pointed ribbon ends.**
- Frame fill: **{{FRAME}}**.
- **Icon** baked inside the frame at the left, vertically centred, ~150 px,
  centred near x=210: **{{ICON}}** — cute-cartoon, chunky, friendly, clean silhouette.
- **Label safe-zone:** the frame interior from ~x=390 to ~x=1120 must be **clean,
  uniform fill, EMPTY** — no text, no icon, no decoration (the engine draws the
  label there). **NO baked text anywhere.**
- **Decorations** live in the margin around the frame and may cross the frame edge
  but never enter the label zone: **{{DECOR}}**. Keep them soft, warm, child-safe,
  in the painted-storybook style of the mockup.
- Warm palette only (cream, honey-gold, soft peach, gentle greens, sky pastels);
  child-safe; no cold/harsh/scary elements; no stray annotations (no numbers,
  filenames, watermarks).

**Deliver** the single file at exactly that path/size/alpha. Attach a preview of
the plate with placeholder label text drawn in the safe-zone so the fit can be
checked. Do not include .meta/prefab/scene/material files.

---

## After delivery (integration notes for the dev)
- Drop the PNG into `Assets/_JebbyJump/Art/Sprites/UI/` and import as a Sprite,
  **Simple** (not sliced — these are fixed-size plates), FullRect.
- The plate is placed as a decorative Image at the button position; the frame's
  central region (`~81% width, ~54% height` of the canvas) aligns to the button's
  RectTransform; the interactive Button + Fredoka label sit on top in the label zone.
- To add the button to a manifest for validation, copy a UI02 plate row in
  `UI02_zip_manifest.json` and change `asset_id` + paths to the new file.
