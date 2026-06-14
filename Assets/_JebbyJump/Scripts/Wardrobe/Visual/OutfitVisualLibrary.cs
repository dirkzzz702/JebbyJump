using System.Collections.Generic;
using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Serialized outfit-id -> animator-override mapping. This is the asset
    // layer the static OutfitVisualCatalog cannot be (no Resources/paths/
    // GUID loading): per-outfit AnimatorOverrideControllers are referenced
    // here and the asset is wired into PlayerOutfitVisualController on the
    // player prefab. Outfit-agnostic: future outfits are new entries, no
    // code change. Outfits with no entry (or a null controller) stay no-op
    // and look like default Jebby. Cosmetic only - no gameplay effect.
    [CreateAssetMenu(
        fileName = "OutfitVisualLibrary",
        menuName = "Jebby Jump/Outfit Visual Library")]
    public sealed class OutfitVisualLibrary : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public string OutfitId;
            public RuntimeAnimatorController ControllerOverride;
        }

        [SerializeField] private List<Entry> _entries = new List<Entry>();

        public int Count => _entries.Count;

        // Editor-scaffold / test convenience; replaces an existing entry
        // for the same id instead of duplicating.
        public void AddEntry(string outfitId, RuntimeAnimatorController controller)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].OutfitId == outfitId)
                {
                    _entries[i] = new Entry
                    { OutfitId = outfitId, ControllerOverride = controller };
                    return;
                }
            }
            _entries.Add(new Entry
            { OutfitId = outfitId, ControllerOverride = controller });
        }

        public bool TryGetOverride(
            string outfitId, out RuntimeAnimatorController controller)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].OutfitId == outfitId
                    && _entries[i].ControllerOverride != null)
                {
                    controller = _entries[i].ControllerOverride;
                    return true;
                }
            }
            controller = null;
            return false;
        }

        // Read-only view of the raw entry ids (for validation / duplicate
        // detection in the QA audit + integrity tests). Mirrors
        // WardrobePreviewLibrary.EntryIds.
        public IReadOnlyList<string> EntryIds()
        {
            var ids = new List<string>(_entries.Count);
            for (int i = 0; i < _entries.Count; i++) ids.Add(_entries[i].OutfitId);
            return ids;
        }
    }
}
