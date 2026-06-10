using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Reads the equipped outfit id and applies the matching visual
    // definition to the player's Animator/SpriteRenderer layer.
    //
    // P11: every outfit resolves to a no-op definition (no art yet), so this
    // never modifies the Animator or SpriteRenderer - Jebby looks unchanged.
    // It only records the resolved outfit id. The apply path is real and
    // ready: once a definition carries a non-null AnimatorControllerOverride
    // (a future AnimatorOverrideController), it is assigned to the Animator.
    //
    // Does NOT touch Animator parameters/state names or SpriteRenderer flipX
    // - those stay owned by PlayerAnimator. Applies in Start() so it runs
    // after sibling components have initialized in Awake().
    [DisallowMultipleComponent]
    public sealed class PlayerOutfitVisualController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;
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
        // via the resolver; the apply rule (assign a runtime controller only
        // when the definition carries a non-null override) lives in
        // OutfitVisualApplier. No-op for all P12 outfits (no art yet).
        public void ApplyOutfit(string outfitId)
        {
            var def = OutfitVisualCatalog.GetVisualForOutfit(outfitId);
            _lastAppliedOutfitId = OutfitVisualApplier.Apply(_animator, def);
        }
    }
}
