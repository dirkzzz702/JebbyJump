# Paste-Ready Kickoff Prompt — Batch 005 (Shell UI Kit: Panel + Pill Button)

---

You are producing FINAL production art for the Unity game **Jebby Jump**,
continuing an audited pipeline (batches 001–004 accepted & integrated). This
session executes **batch 005 only**: the shell UI chrome — ART-007 (9-slice
panel) and ART-008 (9-slice pill button). They are ONE visual family. Precise
EXECUTION, not redesign; the specs win over instinct.

## Repository access
Public repo `https://github.com/dirkzzz702/JebbyJump` (branch `main`). Fetch
via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/ProductionArtAudit/asset_briefs/ART-007.md`

## Step 1 — load instructions (ALL, before generating)
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` — binding rules.
2. `ProductionArtAudit/generation/batches/005_ui_kit.md` — this batch.
3. `ProductionArtAudit/asset_briefs/ART-007.md` + `ART-008.md` — full specs.
4. `ProductionArtAudit/zip_blueprint/zip_manifest.json` + `ProductionArtAudit/tools/validate_generated_art.py` + `ProductionArtAudit/tools/build_final_art_zip.py`.

## Step 2 — reference (HARD GATE)
Upload before generating: `Assets/_JebbyJump/Art/Sprites/UI/ui_btn_jump.png`
(the ornate gold-rimmed control orb — your rim/material style anchor).
Git-LFS: follow the raw-URL redirect, confirm a real PNG. If unavailable,
STOP and ask me to upload it.

## Step 3 — execute (9-slice discipline is the whole game here)
**ART-007 panel** — `Assets/_JebbyJump/Art/Sprites/UI/ui_panel_soft_9s.png`,
exactly **256×256 RGBA** (generate at 512, downscale deterministically):
- Rounded rectangle filling the canvas minus a small margin; corner radius
  exactly 96px at 512 (= 48px at 256), all four corners identical.
- Body deep blue-charcoal #262633 (opaque inside the shape; alpha 0 outside);
  a ~3px warm gold rim (#E6C36B family, matching the uploaded orb's rim);
  faint soft top inner light.
- The middle of EVERY edge and the centre must be COMPLETELY flat colour —
  the validator rejects any texture/gradient in the stretch regions.

**ART-008 pill button** — `Assets/_JebbyJump/Art/Sprites/UI/ui_btn_pill_9s.png`,
exactly **256×96 RGBA** (generate at 512×192, downscale):
- A true pill: corner radius = half the height (fully rounded ends,
  identical left/right). Body #333340, same gold rim, soft top highlight
  inside the rim.
- The horizontal middle 50% must be COMPLETELY flat for stretching.
- ONE bitmap only — interaction states come from Unity tinting, so keep the
  art clean and mid-bright enough that darkening tints stay legible.

Same rim colour, same material feel on both — they must read as one kit next
to the ornate orb buttons.

## Step 4 — validate + deliver
- `python ProductionArtAudit/tools/validate_generated_art.py --manifest ProductionArtAudit/zip_blueprint/zip_manifest.json --dir <out> --ids ART-007,ART-008`
- On PASS: `python ProductionArtAudit/tools/build_final_art_zip.py --manifest ProductionArtAudit/zip_blueprint/zip_manifest.json --dir <out> --out jebby_art_batch005.zip --ids ART-007,ART-008`
- Return: ZIP + validation_report.md + a MOCK COMPOSITE (the panel stretched
  to ~700×500 with three pill buttons stretched to ~300×90 on it, plus white
  mock label text) so kit cohesion and 9-slice stretch can be judged + the
  briefs' §22 acceptance checkboxes.
- In the mock, stretch by scaling ONLY the middle regions (simulate 9-slice);
  if you cannot, say so and provide the unstretched sprites side by side.

## Non-negotiables
Exact filenames/paths/sizes; RGBA with alpha 0 outside the shapes; perfectly
flat stretch regions; identical corners; no text/icons baked into the chrome;
no drop shadows outside the shape; no .meta/Unity-native files; report the
validator's real output. Image delivery only — the Sprite Editor border setup
(64 all sides for the panel; 48/48/40/40 for the pill) and the scene-wide
Image swaps happen later in the project.

When delivered, STOP. Batch 006 (skill icons + VFX) runs in a new session.
