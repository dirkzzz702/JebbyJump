using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    public enum OutfitVisualApplyResult
    {
        AppliedOverride = 0,  // assigned the definition's override controller
        RestoredDefault = 1,  // assigned the captured default controller
        NoAnimator = 2,       // animator was null - nothing done
        NoOp = 3,             // restore wanted but no default captured - left as-is
    }

    // Single source of truth for the outfit visual apply rule. Pure and
    // null-safe. A definition that carries a real override assigns it; any
    // other case (default outfit, no override, missing library entry) RESTORES
    // the captured default controller so a previous override never lingers. A
    // null default on the restore path is a NoOp (never destroys the current
    // controller by assigning null).
    //
    // Only touches Animator.runtimeAnimatorController. Never touches Animator
    // parameters/state names, SpriteRenderer flipX/color, materials, or
    // sorting layers. No Resources / Addressables / paths / GUID lookup.
    public static class OutfitVisualApplier
    {
        public static OutfitVisualApplyResult Apply(
            Animator animator,
            RuntimeAnimatorController defaultController,
            OutfitVisualDefinition def)
        {
            if (animator == null) return OutfitVisualApplyResult.NoAnimator;

            if (def != null
                && def.HasVisualOverride
                && def.AnimatorControllerOverride != null)
            {
                animator.runtimeAnimatorController = def.AnimatorControllerOverride;
                return OutfitVisualApplyResult.AppliedOverride;
            }

            // Default outfit / no override / missing entry -> restore default.
            if (defaultController != null)
            {
                animator.runtimeAnimatorController = defaultController;
                return OutfitVisualApplyResult.RestoredDefault;
            }

            // No captured default: do NOT assign null (would wipe the current
            // controller). Leave the Animator untouched.
            return OutfitVisualApplyResult.NoOp;
        }
    }
}
