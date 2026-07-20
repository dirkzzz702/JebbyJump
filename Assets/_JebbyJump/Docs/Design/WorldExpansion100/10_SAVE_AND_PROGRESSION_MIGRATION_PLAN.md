# 10 — Save & Progression Migration Plan

Status: `PROPOSED`. Objective: expand 10→100 levels with **zero loss** of existing local progress.

## Current keys (REPO-VERIFIED, doc 01 §5)

| Concern | Key | Keyed by |
|---|---|---|
| Unlock | `jebby.level.highestUnlocked` (int) | **index** (monotonic) |
| Best time | `JebbyJump.BestTime.<levelAssetName>` (float) | **asset name** |
| Stars | `jebby.rewards.levelStars.<levelIndex>` (int 0..3) | **index** |
| Rank | not persisted (recomputed) | — |
| Wardrobe | `WardrobePersistenceKeys` + versioned migrator | (existing) |

## Why the expansion is save-safe (the core guarantee)

Because World 1 = Levels 1–10 **unchanged** (Gate #4): array indices 0–9 stay put and asset names
stay `Level1Config`…`Level10Config`. Therefore:
- `jebby.level.highestUnlocked` (index) still points at the same levels.
- `jebby.rewards.levelStars.<0..9>` still map to the same levels.
- `JebbyJump.BestTime.Level{1..10}Config` still resolve.

Appending Levels 11–100 as indices 10–99 with new asset names **adds** keys, strands none.

## Behaviour to preserve / add

- **Existing partial progress:** a player who unlocked up to index 5 keeps `highestUnlocked=5`;
  Continue still lands on level 6. No forced re-unlock, no mass-unlock of 100.
- **World unlock derivation:** a world is unlocked iff its first level is unlocked. World n first
  index = `(n-1)*10`. Derive from `highestUnlocked`; do **not** add a separate world-unlock key
  unless a design reason appears (avoids drift).
- **Continue:** unchanged (`LevelProgressStore.GetContinueIndex(count)` with `count=100`).
- **Finale unlocks next world:** clearing level `n*10-1` (0-based) advances `highestUnlocked` to
  `n*10` = next world's first level — already how `UnlockNext` works. No special case needed.
- **Corrupted/unknown level id or out-of-range index:** existing defensive clamps
  (`IsUnlocked` treats negative/huge as locked; `GetContinueIndex` clamps) already cover 100.

## New keys (reward model C — non-currency)

```
jebby.rewards.worldGem.<worldId>        int/bool  first-clear trophy (idempotent, never decreases)
jebby.story.seen.<cardId>               bool      story-card first-view (ack pattern)
```
- World Gem write is **idempotent**: set-once on finale first clear; re-clear is a no-op. Not a
  balance/currency value; never spent.
- Cosmetic unlock reuses the **existing Wardrobe unlock service** (world mastery = all 10 levels
  cleared). **Stars are never consumed** and wardrobe thresholds are **not modified** (Gate #3).

## Schema version

- Current stores are keyless-versioned except Wardrobe (which has `WardrobePersistenceMigrator`).
- `PROPOSED`: **no global schema bump needed** for the level expansion (additive keys only). Only
  introduce a version stamp if the reward/story keys need future migration; if so, mirror
  `WardrobePersistenceMigrator` (write-last stamp, future-version read-only tolerance).

## Tests (see doc 25)

- Existing L1–10 saves remain valid after expansion (unlock/stars/best-time).
- No mass-unlock; unlock crosses world boundaries at finale.
- World Gem idempotent (double-clear grants once).
- Story-card ack once; Stars total unchanged by any reward path.
- Unknown/oob index handled.
