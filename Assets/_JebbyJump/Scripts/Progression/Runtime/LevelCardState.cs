namespace JebbyJump.Progression
{
    // Visual state of a Level Select card. Kept separate from
    // LevelProgressStore so the store stays progression-only.
    public enum LevelCardState
    {
        Locked,
        Unlocked,
        Completed,
    }

    // Pure classification of a card's state from facts the caller already
    // has. Takes plain bools so this assembly does not need to reference
    // BestTimeStore (which lives in Assembly-CSharp); the caller passes
    // hasBestTime = !float.IsNaN(BestTimeStore.GetBest(key)).
    public static class LevelCardClassifier
    {
        public static LevelCardState Classify(bool unlocked, bool hasBestTime)
        {
            if (!unlocked) return LevelCardState.Locked;
            return hasBestTime
                ? LevelCardState.Completed
                : LevelCardState.Unlocked;
        }
    }
}
