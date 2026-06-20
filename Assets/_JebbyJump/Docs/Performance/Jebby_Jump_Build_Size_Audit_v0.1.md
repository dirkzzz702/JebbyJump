# Jebby Jump - Build Size Audit v0.1 (P24)

Authored summary of the Android AAB size. Authoritative compressed size from the
P23 release report; detailed packed (uncompressed-in-package) per-asset breakdown
from a fresh P24 `BuildReport` (raw output in the ignored `Builds/P24/<commit>/`).

## Units

- **bytes** - exact.
- **MB** = bytes / 10^6.
- **MiB** = bytes / 2^20.
- **compressed AAB** = the `.aab` file on disk (what ships / downloads).
- **packed** = uncompressed-in-package asset sizes from `BuildReport.packedAssets`;
  these do NOT sum to the compressed AAB (the package is compressed).

## Compressed AAB (authoritative)

| Build | bytes | MB | MiB |
|---|---|---|---|
| P23 baseline | 113,625,618 | 113.63 | 108.36 |
| P24 (post-fix) | 113,630,295 | 113.63 | 108.37 |
| **Delta** | **+4,677** | +0.005 | +0.004 |

The +4,677 B (~4.6 KiB) delta is the P24 `ProfilerMarker`s + zero-alloc formatter
code. Markers ship at negligible overhead (Decision 8); the increase is ~0.004% of
the AAB and is accepted. (Note: the builder shares the `Builds/P23` output path, so
the P24 rebuild overwrote the on-disk P23 report; the authoritative P23 figure above
is the P23 commit record, not the overwritten on-disk file.)

## Packed contributors (uncompressed-in-package)

Total packed: 337,184,617 B (~337.18 MB) -> compresses to the ~113.6 MB AAB.

The largest packed assets are the **prototype outfit character sprite PNGs**
(~6,292,324 B / ~6.29 MB packed each), spanning the outfit sets (e.g. SunshineKnight,
ForestCavalier, PastelPrince, CrimsonHero). Outfit art dominates packed size; the
top 25 contributors are all outfit sprite frames.

## Policy / notes

- No hard size fail-gate (no approved product budget). Baseline recorded for future
  comparison.
- **Prototype outfit art is NOT final-certified**; texture compression / import
  settings review + any reduction is out of P24 scope (needs separate approval).
- No assets were deleted or re-imported in P24.
- A clear future lever (if a size budget is set): outfit sprite import settings /
  compression / atlasing - to be measured + approved separately.
