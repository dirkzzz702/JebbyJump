using System;

namespace JebbyJump.Rewards
{
    // Pure world-reward decisions (WorldExpansion100, phase P34G, model C).
    // No engine/scene/save dependency - the caller injects the facts, so this
    // is fully unit-testable. WorldMapping (world<->level ranges) lives in the
    // caller; here a world is described by its level index range.
    public static class WorldRewardLogic
    {
        // World mastery = EVERY level in the world has been cleared. Used to
        // grant the themed cosmetic (model C); Stars are never consumed and
        // no rank threshold is required (any clear counts). firstLevelIndex /
        // lastLevelIndex are inclusive 0-based indices; isLevelCleared is the
        // caller's predicate (e.g. a best time exists / unlock passed it).
        public static bool IsWorldMastered(
            int firstLevelIndex, int lastLevelIndex,
            Func<int, bool> isLevelCleared)
        {
            if (isLevelCleared == null) return false;
            if (firstLevelIndex < 0 || lastLevelIndex < firstLevelIndex) return false;
            for (int i = firstLevelIndex; i <= lastLevelIndex; i++)
                if (!isLevelCleared(i)) return false;
            return true;
        }

        // The World Gem is granted the first time a world's finale is cleared.
        // The finale is the world's last level; a "clear" is any completion.
        // (The one-shot / idempotency guarantee is enforced by WorldGemStore;
        // this just states the eligibility rule.)
        public static bool IsFinaleClear(int clearedLevelIndex, int worldLastLevelIndex)
            => clearedLevelIndex >= 0 && clearedLevelIndex == worldLastLevelIndex;
    }
}
