using System.Collections.Generic;
using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Serialized outfit-id -> UI preview sprite mapping. UI-ONLY: this is the
    // wardrobe panel's thumbnail source and is deliberately SEPARATE from
    // OutfitVisualLibrary (which drives the gameplay Animator override).
    // Previews never touch the player's Animator/SpriteRenderer and are not
    // required for runtime outfit application. Null-safe: a missing entry or
    // null sprite simply yields no thumbnail. Unlike OutfitVisualLibrary this
    // covers ALL outfits including the default (the default has its own idle
    // preview but no override controller).
    [CreateAssetMenu(
        fileName = "WardrobePreviewLibrary",
        menuName = "Jebby Jump/Wardrobe Preview Library")]
    public sealed class WardrobePreviewLibrary : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public string OutfitId;
            public Sprite PreviewSprite;
        }

        [SerializeField] private List<Entry> _entries = new List<Entry>();

        public int Count => _entries.Count;

        // Editor-scaffold / test convenience; replaces an existing entry for
        // the same id instead of duplicating (keeps entries unique by id).
        public void AddEntry(string outfitId, Sprite preview)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].OutfitId == outfitId)
                {
                    _entries[i] = new Entry
                    { OutfitId = outfitId, PreviewSprite = preview };
                    return;
                }
            }
            _entries.Add(new Entry
            { OutfitId = outfitId, PreviewSprite = preview });
        }

        public bool TryGetPreview(string outfitId, out Sprite preview)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].OutfitId == outfitId
                    && _entries[i].PreviewSprite != null)
                {
                    preview = _entries[i].PreviewSprite;
                    return true;
                }
            }
            preview = null;
            return false;
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
