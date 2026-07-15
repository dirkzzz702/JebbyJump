# Paste-Ready Fix-Pass Prompt — Batch 002 Revision A (Wordmark lettering)

Use in the same ChatGPT session that produced batch 002 (or load the batch-002
kickoff context first in a fresh one).

---

Your batch 002 wordmark passed technical validation (exact canvas, real vector
SVG) and the style direction is approved. It FAILS visual review on lettering
integrity — this is a targeted REVISION pass. Keep the approved style exactly
(plump rounded storybook letterforms, warm ivory fill, gold outline, single
rainbow-gem accent on the second "J", arc <= 8 degrees, transparent
background) and fix ONLY:

**C1 — the "e" must read unambiguously as a lowercase "e".** Currently it
reads as "a"/"o" (closed blob with a mid slash). Give it a clear open counter
and horizontal crossbar: the mark must read "Jebby", never "Jabby".

**C2 — "Jump" is one word.** Remove the gap before the final "p"; all four
letters "J-u-m-p" sit at consistent size, weight and baseline with even
letter-spacing. No shrunken or detached letters.

**C3 — put a clear word space between "Jebby" and "Jump"** — roughly one
lowercase-letter width, and visibly wider than any letter-spacing inside the
words.

Sanity gate before delivering: downscale your render to 200px wide and confirm
a fresh reader sees exactly "Jebby Jump". If not, iterate before packaging.

Deliver as before:
- `Assets/_JebbyJump/Art/Sprites/UI/ui_wordmark_jebbyjump.png` — exactly
  1200x400 RGBA (deterministic Pillow fit; trim to content + ~4% padding).
- `Marketing/Brand/jebby_jump_wordmark_master.svg` — true vector paths (same
  standard as your first delivery).
- PLUS the two preview composites you skipped: the wordmark over (a) a
  pastel-sky swatch and (b) a dark #262633 panel swatch (not in the ZIP).

Validate with `--ids ART-010`, build `jebby_art_batch002_revA.zip` only on
PASS, and report the validator's real output. Still a CANDIDATE pending human
brand/legal review.
