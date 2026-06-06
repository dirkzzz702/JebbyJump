# Jebby Jump - Reward UI Copy / Visual QA Checklist v0.1

Status: CHECKLIST ONLY (P7C). Defines the manual visual QA to run later for
the Stars UI added in P7A/P7B. No code/scene/prefab/runtime changes. None of
the checks below have been performed yet - they remain DEFERRED until someone
actually runs them in the editor or on a device.

---

## 1. Purpose

P7A added result-panel Stars and P7B added Level Select card Stars, but their
**rendered** appearance was never visually verified (no Play-Mode testing was
done). This document defines exactly what to check, with pass/fail criteria,
so the Stars UI can be confidently signed off before any deeper economy work
(Spark Coins, cosmetics, shop, ads).

## 2. Scope

In scope (manual checks to perform later):
- Result-panel Stars display + "New Star Best!" behaviour.
- Level Select card Stars display across locked / unlocked / completed states.
- Reset / replay behaviour as seen on screen.
- Copy consistency and readability/accessibility.

Out of scope: any code/UI change. This is a checklist; fixes (if any are found)
belong to a later, separately-approved polish phase.

## 3. Current implementation summary (verified from code)

| Where | Source | Exact text |
|---|---|---|
| Result panel | `HUDController.GrantStars` -> `StarRewardFormatter.Label` | `Stars: N/3`, plus `  (New Star Best!)` appended when the stored best increases |
| Level Select card | `StarRewardFormatter.Label` | `Stars: N/3` |

> P7D standardized both surfaces to `Stars: N/3` via the single
> `StarRewardFormatter.Label` source. (Earlier the Level Select card used
> `Stars N/3` with no colon.) Manual visual verification remains deferred.

Mechanics (P7A/P7B, unchanged here): Stars are **local, per-level, best-only,
clamped 0..3**. Mapping: S/A = 3, B = 2, C = 1, completed-with-no-rank-config
= 1, failure = 0. `StarRewardStore` never decreases stored stars. Level Select
reads `StarRewardStore.GetStars(i)` only (no writes, no analytics). Result-panel
clears emit local `reward_granted` / `star_total_changed` only when the stored
best increases.

## 4. Environments / devices to test later

```text
Unity Editor Play Mode (primary dev check)
Mobile portrait build (primary platform - phone)
Desktop build (secondary)
State A: fresh PlayerPrefs (after Reset Everything) - no stars
State B: partial progress - mix of 1/3, 2/3, 3/3 and locked levels
```

## 5. Result Panel Stars checklist

```text
[ ] Clear a level with C rank -> result panel shows "Stars: 1/3"
[ ] Clear a level with B rank -> "Stars: 2/3"
[ ] Clear a level with A or S rank -> "Stars: 3/3"
[ ] First time stars improve for a level -> "(New Star Best!)" appears
[ ] Replay with a LOWER rank -> stored stars do NOT decrease;
    panel still reads sensibly; NO "(New Star Best!)"
[ ] Replay with the SAME stars -> NO "(New Star Best!)"
[ ] Fail the level (lose all lives) -> result/completion Stars are NOT shown
    as if cleared (failure path does not run the stars grant)
[ ] Time display unchanged
[ ] Rank display unchanged
[ ] Best-time display unchanged
[ ] Retry / Next / Main Menu buttons still usable
[ ] No text overlap, no clipping, Stars line fits the panel
```

## 6. Level Select Stars checklist

```text
[ ] Fresh install / after Reset Everything -> every card shows "Stars: 0/3"
[ ] After clearing a level with C -> that card shows "Stars: 1/3"
[ ] After improving that level to B -> "Stars: 2/3"
[ ] After improving to A/S -> "Stars: 3/3"
[ ] Replay with a lower rank -> card stays at the previous best stars
[ ] Locked card -> still clearly locked: locked overlay visually dominant,
    button non-interactable; Stars text does NOT make it look playable
[ ] Completed card -> Best, Rank, and Stars are all readable together
[ ] Level Select scroll / grid still works
[ ] Card spacing still looks clean (Stars line below Rank, no overlap)
[ ] Best / Rank remain visible alongside Stars
[ ] StarsText does not collide badly with the locked overlay
[ ] No duplicate StarsText (scaffold was verified idempotent in P7B;
    re-confirm visually if BuildLevelSelectPanel is re-run)
```

## 7. Reset / replay scenarios

```text
[ ] Jebby Jump/Reset/Reset Stars -> all cards return to "Stars: 0/3"
[ ] Jebby Jump/Reset/Reset Everything -> progress, best times, AND stars cleared
[ ] Replay without improvement -> no star change, no reward_granted in the log
[ ] Replay with improvement -> stars increase; reward_granted +
    star_total_changed appear in the local debug log ([Analytics] lines)
```

## 8. Locked / unlocked / completed card scenarios

| State | Tint | Locked overlay | Button | Best / Rank | Stars |
|---|---|---|---|---|---|
| Locked | dim/gray | visible (dominant) | non-interactable | "Best --" / "Rank --" | "Stars: 0/3" (must not imply playable) |
| Unlocked, uncleared | normal | hidden | interactable | "Best --" / "Rank --" | "Stars: 0/3" |
| Completed | completed tint | hidden | interactable | real Best / Rank | "Stars: N/3" (1-3) |

```text
[ ] Each state matches the row above on screen
```

## 9. Copy consistency notes

**STANDARDIZED in P7D.** Both surfaces now use the same wording, sourced
from the single `StarRewardFormatter.Label`:

```text
Result panel:      "Stars: N/3"   (+ "(New Star Best!)" on improvement)
Level Select card: "Stars: N/3"
```

The earlier inconsistency (P7C documented the Level Select card as
`Stars N/3` with no colon) is resolved - the colon form is used everywhere.
Manual visual verification of the standardized copy remains DEFERRED.

## 10. Accessibility / readability checks

```text
[ ] Stars text is large enough to read on a phone in portrait
[ ] Readable against the card background (completed/unlocked/locked tints)
[ ] Readable when the locked overlay is present
[ ] No low-contrast combination (text vs background)
[ ] No truncation / ellipsis on small screens
[ ] No overlap with Best / Rank lines
[ ] Safe area (notch / home indicator) not intruding on the text
[ ] Result panel remains visually balanced with the extra Stars line
```

## 11. Known limitations

```text
Rendered appearance is UNVERIFIED (testing is headless in CI/dev here).
Stars use plain "N/3" text by design (no star glyphs / no new art).
Reward numbers (S/A=3, B=2, C=1) are placeholders pending P4B + analytics.
This document does not certify any of the above as passing.
```

## 12. Deferred manual QA backlog (preserved - none complete)

```text
P5B Main Menu / Level Select visible flow ............ DEFERRED
P5C Pause visible flow ............................... DEFERRED
P5D Settings visible flow ............................ DEFERRED
P5E Pause -> Settings visible flow ................... DEFERRED
P5F PauseButton visual confirmation .................. DEFERRED
P7A Result Panel Stars visual confirmation ........... DEFERRED
P7B Level Select Stars visual confirmation ........... DEFERRED
P4B manual playtest + balance tuning ................. DEFERRED
```

## 13. Pass / fail criteria

```text
PASS (a surface) = every checkbox in its section is satisfied on the target
device, with no overlap/clipping and correct star counts.

FAIL (any of):
- wrong star count for a given rank
- stored stars decrease on replay
- "(New Star Best!)" shows without an actual improvement
- failure path shows completion stars
- locked card looks playable / overlay not dominant
- text overlap, clipping, truncation, or low contrast
- duplicate StarsText on a card
- Best / Rank / time / best-time display changed vs pre-P7A behaviour
```

## 14. Recommended next phase

```text
1. Actually perform this checklist in the Unity Editor and on a phone build;
   record pass/fail and file any fixes as a separate approved polish phase.
2. Keep P4B manual playtest + balance tuning deferred until testers are ready.
3. Only after manual QA + P4B + analytics review, consider the next economy
   step (Spark Coins) per the P6C Reward / Economy spec.
Do NOT proceed to shop, ads, backend, wardrobe, or paid currency before that.
```
