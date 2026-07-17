# Paste-Ready Fix-Pass Prompt — Batch 006 Revision A (ART-011 shield icon only)

---

Your batch 006 delivery: ART-012, ART-013 and ART-014 are ACCEPTED and already
integrated. ART-011 (`ui_icon_skill_shield_01.png`) FAILED visual review: it is
a washed-out, pale translucent bubble with a barely-visible shield glint — on
the game's BLUE skill button at 70px it disappears almost completely.
Translucency belongs to the VFX sprite (ART-013, accepted); an ICON needs
opacity and contrast. This is a targeted revision of ONE asset.

## Corrections
**C1 — opacity/saturation.** The bubble body must be a RICH saturated
aqua-blue (#3878E6 family), ~90%+ visually opaque, with a strong white rim
highlight and darker blue bottom shading — the same rendering weight as the
uploaded rocket-boots icon, not a ghostly soap bubble.

**C2 — bold emblem.** Centre a SOLID crest-style shield shape (~45% of the
bubble's width): warm ivory/cream fill with a gold outline, clearly readable
as a shield at 70px. Not a faint white outline.

**C3 — self-test before delivering.** Downscale your render to 70px and place
it over a dark blue circle (#2A3C78). If the icon does not pop instantly,
iterate before packaging.

Everything else from the original brief stands: exactly 256x256 RGBA
(generate at 512, downscale deterministically), centred ~78% of canvas,
transparent corners, painterly-ornate family style, no text.

## References (re-upload if this is a fresh session)
- `Assets/_JebbyJump/Art/Sprites/Items/spr_item_rocket_boots_01.png` (style anchor)
- `Assets/_JebbyJump/Art/Sprites/UI/ui_btn_jump.png` (button context)

## Validate + deliver
- `python ProductionArtAudit/tools/validate_generated_art.py --manifest ProductionArtAudit/zip_blueprint/zip_manifest.json --dir <out> --ids ART-011`
- On PASS: build `jebby_art_batch006_revA.zip` (ART-011 only).
- Return: ZIP + validation_report.md + the 70px-over-blue-circle preview +
  an honest diff note vs revision 0.

When delivered, STOP.
