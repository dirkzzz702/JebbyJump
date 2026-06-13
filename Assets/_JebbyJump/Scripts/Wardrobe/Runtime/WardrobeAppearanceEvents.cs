using System;

namespace JebbyJump.Wardrobe
{
    // Payload for a successful equipped-outfit change. Stable ids only - no
    // Unity object references (this lives in the engine-light Runtime layer).
    public readonly struct WardrobeEquippedOutfitChanged
    {
        public readonly string PreviousOutfitId;
        public readonly string CurrentOutfitId;

        public WardrobeEquippedOutfitChanged(string previous, string current)
        {
            PreviousOutfitId = previous;
            CurrentOutfitId = current;
        }
    }

    // Local, in-process notification raised after a SUCCESSFUL equip (see
    // WardrobeEquipService). Active PlayerOutfitVisualController instances in
    // the loaded scene subscribe to re-sync their appearance live. Not raised
    // for AlreadyEquipped / Locked / UnknownOutfit, and never merely because
    // PlayerPrefs was read. No persistence, no analytics, no Unity refs.
    public static class WardrobeAppearanceEvents
    {
        public static event Action<WardrobeEquippedOutfitChanged> EquippedOutfitChanged;

        // Only WardrobeEquipService raises this (same assembly).
        internal static void Publish(WardrobeEquippedOutfitChanged change)
        {
            EquippedOutfitChanged?.Invoke(change);
        }

        // Test isolation: clears all subscribers so leaked handlers from a
        // prior test cannot fire.
        public static void ResetForTests()
        {
            EquippedOutfitChanged = null;
        }
    }
}
