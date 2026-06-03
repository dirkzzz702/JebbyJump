namespace JebbyJump.Rewards
{
    // Pure mapping from a level's achieved rank to mastery stars.
    // Placeholder rule set (no balance tuning): S/A = 3, B = 2, C = 1,
    // any completed clear with unknown/missing rank = 1, failure = 0.
    // Takes the rank as a string so it stays engine-free and testable;
    // callers pass the TimeRank enum's name (e.g. "S").
    public static class StarRewardCalculator
    {
        public const int MaxStars = 3;

        public static int StarsForRank(string rank, bool completed)
        {
            if (!completed) return 0;
            switch (rank)
            {
                case "S":
                case "A":
                    return 3;
                case "B":
                    return 2;
                case "C":
                    return 1;
                default:
                    // Completed, but no/unknown rank config -> minimum 1.
                    return 1;
            }
        }
    }
}
