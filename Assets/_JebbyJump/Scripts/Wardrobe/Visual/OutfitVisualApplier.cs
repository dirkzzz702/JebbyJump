using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Single source of truth for the outfit visual apply rule. Pure and
    // null-safe: assigns the override animator controller to the given
    // Animator ONLY when the definition actually carries one, and returns
    // the resolved outfit id.
    //
    // P12: production definitions are all no-op (HasVisualOverride=false,
    // AnimatorControllerOverride=null) because no outfit art exists yet, so
    // Apply never changes the Animator in normal play. The override branch
    // exists for future per-outfit AnimatorOverrideController art and is
    // covered by tests using an in-memory controller (no committed asset).
    //
    // Does NOT touch Animator parameters/state names or SpriteRenderer flipX.
    public static class OutfitVisualApplier
    {
        // Returns def.OutfitId (or null for a null def). Assigns
        // animator.runtimeAnimatorController only when the definition has a
        // non-null override and the animator is present.
        public static string Apply(Animator animator, OutfitVisualDefinition def)
        {
            if (def == null) return null;

            if (def.HasVisualOverride
                && def.AnimatorControllerOverride != null
                && animator != null)
            {
                animator.runtimeAnimatorController =
                    def.AnimatorControllerOverride;
            }

            return def.OutfitId;
        }
    }
}
