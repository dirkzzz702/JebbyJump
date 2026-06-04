using UnityEngine;

namespace JebbyJump.Rewards
{
    // Pure display formatting for mastery stars. Kept out of the UI
    // MonoBehaviours so the 0..3 clamp is unit-testable. Format is
    // "Stars N/3" (no colon), used by the Level Select cards.
    public static class StarRewardFormatter
    {
        private const int MaxStars = 3;

        public static string Label(int stars)
        {
            return $"Stars {Mathf.Clamp(stars, 0, MaxStars)}/{MaxStars}";
        }
    }
}
