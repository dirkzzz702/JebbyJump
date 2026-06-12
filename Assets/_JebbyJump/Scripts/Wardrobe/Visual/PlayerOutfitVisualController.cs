using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Reads the equipped outfit id and applies the matching visual
    // definition to the player's Animator/SpriteRenderer layer.
    //
    // Overrides come from the serialized OutfitVisualLibrary (per-outfit
    // AnimatorOverrideControllers over the default JebbyAnimator). Outfits
    // without a library entry are no-op: the Animator keeps its serialized
    // default controller, so Jebby looks unchanged. Applied once in Start()
    // (scene load resets the Animator, so equipping the default outfit needs
    // no restore); there is intentionally no live mid-scene re-sync.
    //
    // Does NOT touch Animator parameters/state names or SpriteRenderer flipX
    // - those stay owned by PlayerAnimator. Cosmetic only - no gameplay.
    [DisallowMultipleComponent]
    public sealed class PlayerOutfitVisualController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        // Serialized outfit-id -> override-controller mapping; null-safe
        // (no library = every outfit stays no-op / default visuals).
        [SerializeField] private OutfitVisualLibrary _library;
        [SerializeField] private bool _applyOnStart = true;
        [SerializeField] private string _lastAppliedOutfitId;

        // The outfit id last resolved/applied.
        public string CurrentOutfitId => _lastAppliedOutfitId;

        // Whether a SpriteRenderer is wired (reserved for future sprite-swap).
        public bool HasSpriteRenderer => _spriteRenderer != null;

        private void Start()
        {
            if (_applyOnStart) ApplyEquippedOutfit();
        }

        // Applies whatever outfit is currently equipped in the store.
        public void ApplyEquippedOutfit()
            => ApplyOutfit(WardrobeStore.GetEquippedOutfitId());

        // Applies the given outfit id. null/unknown fall back to the default
        // via the resolver; the serialized library supplies the per-outfit
        // override controller (outfits without an entry stay no-op = default
        // visuals); the apply rule lives in OutfitVisualApplier.
        public void ApplyOutfit(string outfitId)
        {
            var def = OutfitVisualCatalog.GetVisualForOutfit(outfitId, _library);
            _lastAppliedOutfitId = OutfitVisualApplier.Apply(_animator, def);
        }
    }
}
