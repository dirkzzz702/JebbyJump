# Jebby Jump - Wardrobe Visual QA / UI Polish Checklist + Art / Sprite-Swap Readiness Plan v0.1

Status: CHECKLIST + READINESS PLAN ONLY (P10). Defines how to manually verify
the P9 Wardrobe panel and how to prepare for future art / sprite-swap work.
No code/scene/prefab/art/runtime changes. None of the manual checks below
have been performed - they remain DEFERRED / NOT VERIFIED until someone runs
them in the editor or on a device. P10 does not capture screenshots or claim
any visual QA was performed.

P11 update (implemented, functional only): the equipped outfit now has a safe
visual application path - a new `JebbyJump.Wardrobe.Visual` asmdef
(`OutfitVisualDefinition`, `OutfitVisualCatalog` resolver) and a
`PlayerOutfitVisualController` wired onto `Jebby.prefab` (Game.unity untouched).
On Start it applies the equipped outfit, but every outfit currently resolves to
`HasVisualOverride=false` (no art yet), so it is a **no-op** - non-default
outfits still look like the default Jebby. Final outfit art is still required;
sprite-swap is not visually meaningful until art exists. A future
`AnimatorOverrideController` / sprite-sheet plugs into the resolver. The P11
wardrobe visual application flow is itself DEFERRED / NOT VERIFIED (manual QA
intentionally skipped).

P12 update (docs + test seam): the First Outfit Art Asset Request Pack for
Forest Cavalier was added
(`Assets/_JebbyJump/Docs/Art/Jebby_Jump_First_Outfit_Art_Asset_Request_Pack_v0.1.md`):
first outfit choice, identity guardrails, the 7 animation states, actual sprite
import settings, naming/folder conventions, and the AnimatorOverrideController
pipeline (base JebbyAnimator). A pure-static OutfitVisualApplier seam + 4
override-assignment tests were added (80/80 pass); the catalog stays no-op for
all outfits and no art is imported. The P12 first outfit art pipeline / asset
readiness is DEFERRED / NOT VERIFIED (manual QA intentionally skipped).

P13 update (Mode A - blocked on art): intake was attempted but no Forest
Cavalier art exists, so nothing was imported and no override was wired; the
catalog stays no-op. A read-only editor QA gate was added for future intake
("Jebby Jump/QA/Check Outfit Sprite Alpha": Sprite/Single, PPU 100, pivot
(0.5,0), Alpha Is Transparency, transparent corners). Next step: provide or
externally generate the 7 state sprites per the P12 pack. The P13 Forest
Cavalier art / visual prototype is DEFERRED / NOT VERIFIED.

---

## 1. Purpose

P9 implemented the local wardrobe foundation (catalog, equipped store,
Stars-gated unlock service, text-only panel, Main Menu button, local
analytics). Its rendered appearance was never visually verified, and no
outfit art exists. This document (a) defines exactly what to check on the
Wardrobe panel before sign-off, and (b) defines the art-production and
sprite-swap requirements for a future, separately-approved phase - without
implementing any of it.

## 2. Scope

In scope (later manual work / future planning): Wardrobe panel visual QA,
state/edge-case checks, copy/readability, art-asset requirements, sprite-swap
technical options.

Out of scope (P10 itself): any code/scene/prefab/asset/art change; sprite
swap; debug cheats; shop / Spark Coins / Rainbow Gems currency / ads /
backend; gameplay effects.

## 3. Current P9 implementation summary (verified from code)

| Aspect | Actual (P9) |
|---|---|
| Entry | Main Menu **Wardrobe** button (stack: Continue / Level Select / Settings / Wardrobe / Quit) -> `WardrobePanelController.Open()` |
| Panel title | `Wardrobe` |
| Outfit row | `"<DisplayName>  -  <state>"` |
| State text | `Equipped` / `Unlocked` / `Locked (N Stars)` |
| Preview label | `Selected: <DisplayName>` (or `Selected: --`) |
| State label | repeats the selected outfit's state text |
| Buttons | `Equip`, `Back` |
| Outfits | classic_color_knight (0, always), forest_cavalier (8), sunshine_knight (15), aqua_knight (22), silver_dreamer (30) - thresholds are PLACEHOLDERS |
| Unlock rule | unlocked if AlwaysUnlocked, RequiredStars<=0, or totalStars>=RequiredStars (`WardrobeUnlockService`) |
| Equipped rule | Equipped only if unlocked and id == stored equipped id |
| Stars source | `StarRewardStore.GetTotalStars(catalog.Count)` - read-only; Stars never consumed |
| Equip UX | Equip enabled only when selected is unlocked AND not already equipped |
| Persistence | `WardrobeStore` PlayerPrefs `jebby.wardrobe.equippedOutfit` (equipped id only; ownership derived from Stars) |
| Fallback | unknown/locked stored id -> `classic_color_knight` via `NormalizeEquippedId` |
| Analytics (local/debug) | `wardrobe_opened`, `cosmetic_previewed`, `cosmetic_equipped`, `cosmetic_unlock_failed` |
| Reset | `Reset Wardrobe` (equipped->default); `Reset Everything` includes wardrobe; `Reset Stars` does NOT touch wardrobe |

Note: the actual copy is `Selected:` and `Locked (N Stars)` (not "Preview:" or
"Requires N Stars"). Recorded as-is; any copy change is a future polish task,
not P10.

## 4. Manual QA status and deferral rules

```text
All checks below are DEFERRED / NOT VERIFIED.
A check may be marked complete ONLY after it is actually performed on a
target environment, with the result (and any failure) logged honestly.
P10 does not perform, simulate, or claim any visual QA.
P9 Wardrobe panel visual QA stays DEFERRED / NOT VERIFIED.
```

## 5. Test environments / devices

```text
Unity Editor Play Mode (primary dev check)
Mobile portrait build (primary platform - phone)
Desktop build (secondary)
State A: fresh PlayerPrefs (after Reset Everything) - 0 Stars
State B: star-progressed (e.g. 8 / 15 / 22 / 30 total Stars)
```

## 6. Main Menu Wardrobe entry checklist

```text
[ ] Wardrobe button appears exactly once
[ ] Button order reads Continue / Level Select / Settings / Wardrobe / Quit
[ ] Wardrobe button does not overlap other buttons
[ ] Clicking Wardrobe opens the Wardrobe panel
[ ] Continue / Level Select / Settings / Quit still work
[ ] Back from Wardrobe returns to the Main Menu
[ ] Safe area respected (notch / home indicator)
[ ] Layout readable in mobile portrait
```

## 7. Wardrobe panel layout checklist

```text
[ ] Title "Wardrobe" readable
[ ] Outfit list readable
[ ] "Selected: <name>" label readable
[ ] State/requirement label readable
[ ] Equip button readable
[ ] Back button readable
[ ] No clipping / no overlap
[ ] No duplicate rows; no duplicate panel
[ ] Scroll works if content exceeds the viewport
[ ] Panel background / contrast acceptable
```

## 8. Outfit list and state matrix

Expected state per total Stars (Stars are NOT consumed; states are derived):

| Total Stars | Classic Color Knight | Forest (8) | Sunshine (15) | Aqua (22) | Silver (30) |
|---|---|---|---|---|---|
| 0  | Equipped/Unlocked | Locked | Locked | Locked | Locked |
| 8  | Unlocked | Unlocked | Locked | Locked | Locked |
| 15 | Unlocked | Unlocked | Unlocked | Locked | Locked |
| 22 | Unlocked | Unlocked | Unlocked | Unlocked | Locked |
| 30 | Unlocked | Unlocked | Unlocked | Unlocked | Unlocked |

```text
[ ] Each row's state matches the table for the current total Stars
[ ] Locked rows show "Locked (N Stars)" with the correct N
[ ] Unlocking an outfit does NOT change total Stars
```

## 9. Preview / selection / equip checklist

```text
[ ] Opening Wardrobe selects the currently equipped outfit
[ ] Selecting an unlocked outfit updates "Selected: <name>" + state label
[ ] Selecting a locked outfit updates labels but does not equip
[ ] Equip enabled only for an unlocked, not-already-equipped outfit
[ ] Equip disabled for a locked outfit
[ ] Equip disabled / inactive for the already-equipped outfit
[ ] Equipping an unlocked outfit sets its state to Equipped
[ ] Exactly one outfit is Equipped at a time
[ ] Close + reopen preserves the equipped outfit
[ ] Restarting the game preserves the equipped outfit
[ ] Unknown stored id falls back to Classic Color Knight
[ ] Locked stored id falls back to Classic Color Knight
```

## 10. Stars-gated unlock checklist

```text
[ ] After Reset Everything: equipped = Classic Color Knight; others Locked
[ ] 8 total Stars: Forest equippable; Sunshine/Aqua/Silver not
[ ] 15 total Stars: Sunshine equippable
[ ] 22 total Stars: Aqua equippable
[ ] 30 total Stars: Silver equippable
```

Setup options for reaching exact star totals (local testing only):
```text
- play/clear levels to earn Stars, or
- use the existing dev reset tools, or
- temporarily set the star PlayerPrefs in a LOCAL test environment only
Do NOT commit debug cheats. P10 adds none.
```

## 11. Reset / persistence checklist

```text
[ ] Reset Wardrobe: equipped -> Classic Color Knight; Stars unchanged
[ ] Reset Stars: Stars reset; equipped id still stored; if it is now locked,
    the panel normalizes/falls back to Classic Color Knight on next open
[ ] Reset Everything: progress + best times + Stars + wardrobe all reset
[ ] Reopen game: equipped outfit persists if still valid/unlocked
```

## 12. Analytics / log checklist (local/debug only)

```text
[ ] wardrobe_opened fires when opening the panel
[ ] cosmetic_previewed fires when selecting a different outfit
[ ] cosmetic_previewed does NOT spam during list population
[ ] cosmetic_equipped fires only when the equipped outfit changes
[ ] events carry primitive params only; no backend/network/provider
```
Note: because the Equip button is disabled for locked selections,
`cosmetic_unlock_failed` is normally NOT observable through the UI - this is
expected (the event/path remains for defensive/future use).

## 13. Accessibility / readability checklist

```text
[ ] Readable on a small phone screen in portrait
[ ] Buttons large enough for touch
[ ] Acceptable contrast (text vs panel/background)
[ ] Locked state is not confusing
[ ] Disabled Equip is clearly disabled
[ ] Selected / Equipped state is obvious
[ ] Text not too dense; rows not too close together
[ ] No overlap with the safe area
[ ] Back button easy to find
```

## 14. UI copy checklist

Record ACTUAL wording (do not change code in P10):
```text
Menu button / title : "Wardrobe"
Row                 : "<DisplayName>  -  <state>"
State               : "Equipped" / "Unlocked" / "Locked (N Stars)"
Selected label      : "Selected: <DisplayName>"
Buttons             : "Equip", "Back"
```
Copy guidelines (for any FUTURE polish phase): honest requirements; no "buy",
"premium", "limited time", "watch ad", or pressure language; never imply
gameplay power. If a future phase standardizes wording (e.g. "Requires N
Stars"), do it there - not here.

## 15. Pass / fail criteria

FAIL if any of:
```text
Wardrobe button missing or duplicated
panel cannot open/close
Equip works on a locked outfit / a locked outfit becomes Equipped
Stars are consumed or changed by the wardrobe
more than one outfit appears Equipped
equipped outfit not persisted
unknown/locked equipped id crashes instead of falling back
rows overlap or are unreadable
Main Menu / Settings / Level Select broken
any shop / ads / coins / gems UI appears
any gameplay value changes due to an outfit
manual QA marked complete without being performed
```
PASS (a surface) if: all its state rules hold, UI stays readable, no Stars
semantics change, no gameplay effect, no monetization UI, and the result is
logged honestly.

## 16. Required screenshots / observations (for a FUTURE manual QA run)

Not captured in P10. A future tester should capture:
```text
Main Menu with Wardrobe button
Wardrobe at 0 / 8 / 15 / 30 Stars
a locked outfit selected
an unlocked outfit selected
an equipped non-default outfit
after Reset Wardrobe
after Reset Everything
(optional) debug log showing wardrobe analytics events
```

## 17. Known limitations

```text
Rendered appearance is UNVERIFIED (testing here is headless).
The equipped outfit is stored but NOT visually applied to Jebby yet
  (no sprite swap in P9 - by design).
Outfit thresholds (0/8/15/22/30) are PLACEHOLDERS pending P4B + level count.
This document certifies nothing as passing.
```

## 18. Art production readiness requirements

No art is produced/imported in P10; this defines requirements for a future
art phase.

**Jebby identity guardrails (Art Bible section 2.3):** Jebby the Color Knight
- a Cavalier-inspired fantasy knight, warm eyes, brown/white long-ear hair
silhouette, small cape, small boots, rainbow/color-memory motif, friendly /
brave / child-safe.

> The rainbow/color-memory **motif** is part of Jebby's visual identity. The
> Rainbow Gems **currency** is a separate, deferred concept and is NOT
> implemented.

Outfits **may** change: cape color, clothing theme, boots, decorative
accessories, VFX color, small hat/trim. Outfits **must NOT** change: face
identity, chibi proportions, warm eyes, long ear-feather/hair silhouette,
rainbow-gem motif, friendly personality, core Cavalier softness.

Required outfit art states (match the actual `PlayerAnimator` contract -
see section 19): **idle, run, jump, fall, land, hurt, victory**.

Per-outfit notes:

| Outfit | Palette | May change | Must not change | Readability risk |
|---|---|---|---|---|
| Classic Color Knight | brown/white + blue cape + rainbow badge | (default) | all identity items | none |
| Forest Cavalier | greens + warm brown | cape/leaf accent/boots | face/ears/eyes/silhouette/gem motif | low |
| Sunshine Knight | golds/bright | cape/badge trim/accents | keep gentle, non-premium look | low |
| Aqua Knight | teal/blue + water VFX | cape/water sparkle | silhouette readable under VFX | low-med |
| Silver Dreamer | silver/lavender night | cape/soft glow | avoid dark/low-contrast | med (mobile contrast) |

## Asset QA checklist (future imported assets must pass)

```text
[ ] RGBA transparent background; corner alpha = 0
[ ] no baked checkerboard; no white halo / matte fringe
[ ] consistent canvas size across an outfit's frames
[ ] consistent pivot
[ ] consistent pixels-per-unit
[ ] correct sorting-layer expectations
[ ] consistent outline/shading; no style drift
[ ] no naming drift; no duplicate/confusing files
```
No assets are imported in P10.

## 19. Future sprite-swap technical readiness

Actual rendering/animation contract (from `PlayerAnimator`), which any future
swap must preserve:
```text
Animator params : Speed (float), IsGrounded (bool), VerticalVelocity (float)
Animator triggers: LandTrigger, HurtTrigger, VictoryTrigger
Explicit state   : "Idle" (Animator.Play("Idle"))
Rendering        : a single SpriteRenderer; flipX mirrors horizontally
Implied states   : Idle, Run (Speed), Jump/Fall (VerticalVelocity+IsGrounded),
                   Land, Hurt, Victory
```

Options (design only - do NOT implement in P10):

```text
Option A - full sprite-sheet replacement per outfit
  Pros: simple model; complete frames per outfit; least runtime layering
  Cons: most art; more memory; all frames must stay aligned
  Mechanism: an AnimatorOverrideController per outfit swaps clips while
  keeping the SAME Animator params/state names and flipX behavior.

Option B - layered cosmetic overlays (base Jebby + cape/boots layers)
  Pros: reuse base; mix-and-match later
  Cons: harder animation alignment; runtime + sorting complexity

Option C - palette/recolor + small accessory overlays
  Pros: lightweight; fast production; good for early outfits
  Cons: limited variety
```

Recommended FIRST visual implementation: **Option A or C** (preserve the
Animator parameter contract via an AnimatorOverrideController for A; keep base
silhouette for C). Avoid layered mix-and-match (B) until the base wardrobe
flow is proven. Equipped id is available via
`WardrobeStore.GetEquippedOutfitId()`; P9 left it disconnected, and **P11**
connected it through `OutfitVisualCatalog` -> `PlayerOutfitVisualController`
(attached to `Jebby.prefab`). That apply path is live but a **no-op** until an
outfit supplies a non-null `AnimatorControllerOverride` - so Option A/C art
plugs straight into the existing resolver/controller without code changes.

## 20. Future implementation phases (recommend only; do not start)

```text
P11A - Wardrobe UI Polish Implementation (if QA finds layout/copy issues)
P11B - Wardrobe Art Asset Production Spec / Asset Request Pack
P11C - First Outfit Visual Implementation Prototype
P11D - Sprite-Swap Technical Prototype (AnimatorOverrideController, Option A)
P11E - Cosmetic Unlock Ceremony Design Spec
```
Preferred next: **P11A** if manual QA surfaces clear layout/copy issues;
otherwise **P11B** to begin art production.

## 21. Deferred QA backlog (preserved - none complete)

```text
P5B Main Menu / Level Select visible flow ............ DEFERRED
P5C Pause visible flow ............................... DEFERRED
P5D Settings visible flow ............................ DEFERRED
P5E Pause -> Settings visible flow ................... DEFERRED
P5F PauseButton visual confirmation .................. DEFERRED
P7A Result Panel Stars visual confirmation ........... DEFERRED
P7B Level Select Stars visual confirmation ........... DEFERRED
P9  Wardrobe panel visual flow ....................... DEFERRED / NOT VERIFIED
P11 Wardrobe visual application flow ................. DEFERRED / NOT VERIFIED
P12 First outfit art pipeline / asset readiness ...... DEFERRED / NOT VERIFIED
P13 Forest Cavalier art / visual prototype ........... DEFERRED / NOT VERIFIED
P4B manual playtest + balance tuning ................. DEFERRED
```

## 22. Open questions

```text
22.1 Standardize copy ("Selected:" vs "Preview:"; "Locked (N Stars)" vs
     "Requires N Stars")? - future polish, not decided here.
22.2 Sprite-swap approach for first art (Option A vs C)?
22.3 When is the equipped outfit applied to Jebby visually (which P11 phase)?
22.4 Outfit thresholds - confirm after P4B + final level count.
22.5 Is an unlock ceremony (P11E) wanted, or silent passive unlocks?
```
