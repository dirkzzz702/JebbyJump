# 09 — Data Model & Config Schema

Status: `PROPOSED`.

## WorldDefinition (ScriptableObject, one per world)

```
_worldId            string   "W01".."W10"   (stable; never reused)
_worldNumber        int      1..10
_displayName        string   "Cloud Meadow"
_firstGlobalLevelId int      1,11,...,91     (contiguous)
_lastGlobalLevelId  int      10,20,...,100
_visualSet          WorldVisualSet
_hazardVisual       WorldHazardVisualDefinition
_storyCardOpen      StoryCardDefinition (optional)
_worldGemIcon       Sprite
_cosmeticUnlockId   string   (wardrobe id; empty until art lands)
_thumbnail          Sprite   (level-select)
_finaleTreatment    Sprite/prefab ref
```

## WorldVisualSet

```
_background         Sprite   (P34C - implemented)
_floor              Sprite   (P34C - implemented)
_landmarkTower      Sprite   (distant Rainbow Tower stage art)   [later phase]
_platformBase       Sprite   (ONE neutral base, tinted at runtime) [P34H/P34J]
_decoration         Sprite[] (non-colliding)                      [later phase]
_ambientVfx         Sprite/particle texture (optional)            [later phase]
```

### Platform art: ONE neutral base per world, not six — `REPO-VERIFIED` (P34C)

`Platform.ApplyVisualColor()` sets `sr.color = PlatformColorPalette.GetColor(_color)`, i.e. the
locked semantic colour is applied as a **tint over a shared sprite**. Therefore a world needs a
single **near-greyscale, mid-value themed base** sprite; the six colours come from the tint and
their hue identity is **mathematically exact** (the palette is the single source of the hue).

This supersedes the earlier `PlatformColorVisual[6]` proposal: six pre-coloured sprites would
fight the tint (requiring `sr.color = white` and a gameplay-code change) and would risk per-world
colour drift away from the locked hex values.

Consequence: platform art drops from **60 → 10** assets (doc 18 regenerated accordingly).
Fields are added in P34H/P34J together with the real art that proves them; `WorldVisualSet`
deliberately ships in P34C with only the fields the applier consumes.

## WorldCatalog (ScriptableObject)

```
_worlds  WorldDefinition[10]   (ordered W01..W10)
```
Mirror of the existing `LevelCatalog` pattern; synced via an editor menu like
`Jebby Jump/Progression/Create Or Sync World Catalog`.

## Level → World mapping (contiguous; no new field on LevelConfig)

`WorldResolver.ForLevel(globalIndex0Based)`:
```
worldNumber = globalIndex / 10 + 1        // 0-9→1, 10-19→2, ... 90-99→10
```
**Why derive, not store:** avoids editing all 100 `LevelConfig` assets and avoids a second source
of truth. Contiguity is guaranteed by the generator (doc 23) and asserted by tests (doc 25).
*Alternative (`PROPOSED`, rejected for now):* add `_worldId` to `LevelConfig` — more explicit but
touches every asset and risks divergence from array order. Revisit only if levels ever become
non-contiguous (not planned).

## Level identity & naming

- Existing: `Level1Config`…`Level10Config` (World 1) — **unchanged** (save-key safety, doc 10).
- New: `Level011Config`…`Level100Config` (zero-padded to 3 for stable sort). World n holds the ten
  configs for global ids `(n-1)*10+1 … n*10`, appended to `LevelSessionController._levels` and the
  `LevelCatalog` in order.
- Sequence step still = `rowIndex`; `total_rows == sequence_length`.

## Authoritative level-design source

`13_100_LEVEL_MASTER_TABLE.csv` (CSV chosen — reviewable, diff-friendly, Gate P11). The generator
(doc 23) reads it to create/update `LevelConfig` assets deterministically. World 1 rows are marked
`EXISTING-PRESERVED` and the generator must **not** overwrite hand-tuned World-1 values.

## Reward data (model C)

```
WorldGemStore        per-world first-clear trophy flag (idempotent; not currency)
Cosmetic unlock      via existing Wardrobe unlock service on world mastery
```
Keys + migration in doc 10. Stars are never read/written by rewards.
