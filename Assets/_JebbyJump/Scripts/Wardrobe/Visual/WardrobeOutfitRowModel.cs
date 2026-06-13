using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Pure, UI-facing view data for a single wardrobe row/card. Produced by
    // WardrobeRowModelBuilder so the panel MonoBehaviour only renders it.
    // PreviewSprite is optional (UI-only) and may be null.
    public readonly struct WardrobeOutfitRowModel
    {
        public readonly string OutfitId;
        public readonly string DisplayName;
        public readonly int RequiredStars;
        public readonly bool IsUnlocked;
        public readonly bool IsEquipped;
        public readonly Sprite PreviewSprite;
        public readonly string StateText; // Equipped / Unlocked / Locked (N Stars)
        // Unlocked, non-default, and not yet acknowledged (P16 "New" badge).
        public readonly bool IsNew;

        public WardrobeOutfitRowModel(
            string outfitId, string displayName, int requiredStars,
            bool isUnlocked, bool isEquipped, Sprite previewSprite,
            string stateText, bool isNew)
        {
            OutfitId = outfitId;
            DisplayName = displayName;
            RequiredStars = requiredStars;
            IsUnlocked = isUnlocked;
            IsEquipped = isEquipped;
            PreviewSprite = previewSprite;
            StateText = stateText;
            IsNew = isNew;
        }

        // Equip is allowed only for an unlocked outfit that is not already
        // the equipped one (same rule the panel's Equip button enforces).
        public bool CanEquip => IsUnlocked && !IsEquipped;
    }
}
