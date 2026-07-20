# 11 — Level Select & World Map UX

Status: `PROPOSED`. Mobile-first. Replaces the current flat grid (doc 01 §6) which cannot scale to 100.

## Two-tier navigation

```
World Map          →   World Detail
10 world cards         10 level cards (selected world)
```

- **World Map:** 10 world cards (thumbnail/badge from art manifest), each showing: world name,
  completion state (locked / in-progress / complete / mastered), Stars total for the world
  (`Σ stars over its 10 levels`), World Gem indicator, finale badge, cosmetic-reward preview.
- **World Detail:** the existing card grid, but scoped to one world's 10 levels. Reuses
  `LevelSelectCard`, `LevelCardClassifier`, `ShellFocusUtil.BuildGridNavigation`, and the
  scroll-to-focus logic already in `LevelSelectController` — now driven per-world.

## Locked/gating rules

- World locked until its first level is unlocked (derive from `highestUnlocked`, doc 10).
- Locked world card is focusable but not enterable (mirrors locked level cards today).
- Finale (level 10) card visually distinct (finale badge).

## Continue flow (preserve)

`Continue` still targets `GetContinueIndex(100)`; on press, open the **World Detail** of that
level's world with focus on that level (compute world = `index/10`). Main Menu `Continue` button
behaviour unchanged otherwise.

## Mobile / accessibility (keep current guarantees)

- Touch targets ≥ current project minimum (reuse `ShellLayoutMetrics`); safe-area via existing
  `SafeAreaFitter`.
- Deterministic keyboard/gamepad nav: world map grid → enter world → level grid → Back returns to
  world map → Back returns to Main Menu (restore opener focus via `ShellFocusUtil.Select`).
- Scroll-to-focus per world; localisation-expansion tolerant (no fixed-width labels);
  Memory-Cue/colour-blind cues unaffected (this is navigation UI only).

## Data source

`WorldCatalog` (doc 09) for world cards; `LevelCatalog` sliced by world range for level cards.
No new flat list of 100. Analytics: extend existing `LevelSelected` with `world_id`
(reuse `AnalyticsParam`); add `WorldSelected` event (no new SDK).

## Art dependencies

World thumbnail 512², world badge 192² (locked/complete/finale states), cosmetic preview 512²
— all in doc 18 manifest under each world's `UI/`.

## Phase

Implemented in **P34D** (after world data foundation P34B). Story cards (P34F) hook the
world-enter transition.
