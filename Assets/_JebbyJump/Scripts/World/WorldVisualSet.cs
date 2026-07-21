using UnityEngine;

namespace JebbyJump.World
{
    // The per-world visual payload applied inside the single Game scene
    // (WorldExpansion100, phase P34C).
    //
    // Scope note: only the visuals that can be applied without touching
    // gameplay code live here for now - background and floor are plain
    // SpriteRenderer swaps. Themed PLATFORM art is deliberately absent until
    // P34H/P34J: platforms tint a shared sprite with the locked semantic
    // colour (Platform.ApplyVisualColor), so their themed base sprite is
    // introduced together with the real art that proves it.
    [System.Serializable]
    public class WorldVisualSet
    {
        [SerializeField] private Sprite _background;
        [SerializeField] private Sprite _floor;

        public Sprite Background => _background;
        public Sprite Floor => _floor;

        // A world with no background of its own falls back to World 1 rather
        // than rendering blank (see WorldThemeApplier).
        public bool HasBackground => _background != null;
        public bool HasFloor => _floor != null;
    }
}
