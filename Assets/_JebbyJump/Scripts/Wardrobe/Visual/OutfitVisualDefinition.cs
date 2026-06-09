using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Immutable description of how an outfit maps to player visuals.
    //
    // P11: every definition is a safe no-op (HasVisualOverride=false,
    // AnimatorControllerOverride=null) - no outfit art exists yet, so the
    // default Jebby visuals are used for all outfits.
    //
    // The concrete RuntimeAnimatorController slot is the future plug-in
    // point: an AnimatorOverrideController (which IS-A RuntimeAnimatorController)
    // can be supplied later without changing PlayerOutfitVisualController's
    // apply logic.
    public sealed class OutfitVisualDefinition
    {
        public string OutfitId { get; }
        public string DisplayName { get; }
        public bool HasVisualOverride { get; }
        public RuntimeAnimatorController AnimatorControllerOverride { get; }

        public OutfitVisualDefinition(
            string outfitId,
            string displayName,
            bool hasVisualOverride,
            RuntimeAnimatorController animatorControllerOverride)
        {
            OutfitId = outfitId;
            DisplayName = displayName;
            HasVisualOverride = hasVisualOverride;
            AnimatorControllerOverride = animatorControllerOverride;
        }
    }
}
