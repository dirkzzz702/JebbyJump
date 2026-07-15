# Paste-Ready Fix-Pass Prompt — Batch 001 Revision A

Use in the SAME ChatGPT session that produced batch 001 if possible (it has
the context and the accepted ART-003); otherwise start fresh with the kickoff
prompt rules loaded first.

---

Your batch 001 delivery passed validation and visual review with two
corrections required. This is a targeted REVISION pass — regenerate ONLY what
is listed below; everything else from the original briefs and master rules
still binds you.

## Corrections

**C1 — ART-001 hair fidelity.** Your Jebby's hair is fluffier and longer than
the locked reference. Regenerate `jebby_jump_icon_512.png` matching
`jebby_color_knight_character_sheet_v01.png` MORE closely on these specific
points, changing nothing else about your accepted composition (bust, pastel
sky, gem badge, sparkles):
- hair volume: moderate, close to the head — NOT a large fluffy mane;
- ear-feathers: two defined feather-like tufts with ivory/white tips, matching
  the reference's length (they end near the shoulders, not beyond);
- keep the short ahoge (antenna tuft) on top;
- the brown/ivory two-tone split at the fringe stays.

**C2 — ART-015 silhouette content.** Your monochrome layer was head-only.
The silhouette must be the head PLUS the rainbow-gem badge shape below it
(as a small separated circle or attached pendant shape), so the themed icon
still carries the gem motif. Single colour #FFFFFF, alpha-only shape,
everything inside the centred 264×264 safe zone, exactly as before.

## Regeneration order (dependencies)
1. Regenerate **ART-001** with correction C1.
2. Re-derive **ART-002** (`ic_launcher_foreground_432.png`) from the NEW
   ART-001 subject — same rules as before (transparent, subject inside the
   264px safe zone, outer 72px band fully transparent).
3. Re-derive **ART-015** (`ic_launcher_monochrome_432.png`) from the new
   ART-002 with correction C2 (deterministic flatten, not fresh generation).
4. Do NOT touch **ART-003** (accepted as delivered) — but include the
   accepted file unchanged in the package so the ZIP stays complete.

## Validate + deliver (same as before)
- Exact filenames/paths/dimensions unchanged.
- `python validate_generated_art.py --manifest zip_manifest.json --dir <out>
  --ids ART-001,ART-002,ART-003,ART-015`
- Build `jebby_art_batch001_revA.zip` only on PASS.
- Return ZIP + validation_report.md + a short diff note per asset (what
  changed vs revision 0), honestly including anything you could not fix.

All master-prompt non-negotiables still apply (locked identity, no text, no
.meta, exact numbers, report real validator output).
