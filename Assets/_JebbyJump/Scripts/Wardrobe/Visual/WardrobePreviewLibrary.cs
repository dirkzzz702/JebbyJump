using System.Collections.Generic;
using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Serialized outfit-id -> UI preview sprites mapping. UI-ONLY: the wardrobe
    // panel's thumbnail + in-panel preview source, deliberately SEPARATE from
    // OutfitVisualLibrary (which drives the gameplay Animator override).
    // Previews never touch the player Animator/SpriteRenderer. Covers ALL
    // outfits incl. the default. Null-safe: a missing entry / pose simply
    // yields no sprite.
    //
    // P15 stored only an idle sprite (row thumbnail). P18 extends each entry to
    // the full pose set for the preview carousel; TryGetPreview still returns
    // Idle so P15 rows + the P16 ceremony are unchanged.
    [CreateAssetMenu(
        fileName = "WardrobePreviewLibrary",
        menuName = "Jebby Jump/Wardrobe Preview Library")]
    public sealed class WardrobePreviewLibrary : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public string OutfitId;
            public Sprite Idle;
            public Sprite Run;
            public Sprite Jump;
            public Sprite Fall;
            public Sprite Land;
            public Sprite Hurt;
            public Sprite Victory;
        }

        [SerializeField] private List<Entry> _entries = new List<Entry>();

        public int Count => _entries.Count;

        // Idle-only entry: sets Idle and CLEARS all other pose fields
        // (replace-by-id keeps entries unique). Back-compat for P15 callers/
        // tests that only set a thumbnail.
        public void AddEntry(string outfitId, Sprite idle)
        {
            AddEntry(outfitId, idle, null, null, null, null, null, null);
        }

        // Full pose entry (used by the populate scaffold). Replace-by-id.
        public void AddEntry(string outfitId, Sprite idle, Sprite run,
            Sprite jump, Sprite fall, Sprite land, Sprite hurt, Sprite victory)
        {
            var entry = new Entry
            {
                OutfitId = outfitId, Idle = idle, Run = run, Jump = jump,
                Fall = fall, Land = land, Hurt = hurt, Victory = victory,
            };
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].OutfitId == outfitId) { _entries[i] = entry; return; }
            }
            _entries.Add(entry);
        }

        // Back-compat thumbnail accessor (P15 rows + P16 ceremony): the Idle
        // pose. False when there is no entry or no idle sprite.
        public bool TryGetPreview(string outfitId, out Sprite preview)
            => TryGetPose(outfitId, WardrobePreviewPose.Idle, out preview);

        public bool TryGetPose(
            string outfitId, WardrobePreviewPose pose, out Sprite sprite)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].OutfitId != outfitId) continue;
                sprite = SpriteFor(_entries[i], pose);
                return sprite != null;
            }
            sprite = null;
            return false;
        }

        private static Sprite SpriteFor(Entry e, WardrobePreviewPose pose)
        {
            switch (pose)
            {
                case WardrobePreviewPose.Idle: return e.Idle;
                case WardrobePreviewPose.Run: return e.Run;
                case WardrobePreviewPose.Jump: return e.Jump;
                case WardrobePreviewPose.Fall: return e.Fall;
                case WardrobePreviewPose.Land: return e.Land;
                case WardrobePreviewPose.Hurt: return e.Hurt;
                case WardrobePreviewPose.Victory: return e.Victory;
                default: return null;
            }
        }

        // Read-only view of the raw entry ids (for validation/tests).
        public IReadOnlyList<string> EntryIds()
        {
            var ids = new List<string>(_entries.Count);
            for (int i = 0; i < _entries.Count; i++) ids.Add(_entries[i].OutfitId);
            return ids;
        }
    }
}
