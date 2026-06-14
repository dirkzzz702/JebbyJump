using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Applies the equipped outfit's visuals to the player's Animator and keeps
    // them in sync live. Overrides come from the serialized OutfitVisualLibrary
    // (per-outfit AnimatorOverrideControllers over the default JebbyAnimator).
    //
    // Lifecycle:
    //   Awake     - capture the prefab's default Animator controller (before
    //               any override is applied) so the default can be restored.
    //   OnEnable  - subscribe to WardrobeAppearanceEvents for live re-sync.
    //   Start     - apply the currently stored equipped outfit (spawn path).
    //   OnDisable - unsubscribe (no static subscriber leak).
    //   OnDestroy - defensive unsubscribe.
    //
    // P17: a successful equip (via WardrobeEquipService) raises the change
    // event; active instances in the same loaded scene re-apply immediately.
    // Switching to the default outfit (or any outfit with no override) restores
    // the captured JebbyAnimator. A controller swap may restart the current
    // animation (no state-preservation - documented limitation). Cosmetic only;
    // never touches Animator parameters/triggers/states or SpriteRenderer flipX.
    [DisallowMultipleComponent]
    public sealed class PlayerOutfitVisualController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        // Serialized outfit-id -> override-controller mapping; null-safe
        // (no library = every outfit stays default visuals).
        [SerializeField] private OutfitVisualLibrary _library;
        [SerializeField] private bool _applyOnStart = true;
        [SerializeField] private string _lastAppliedOutfitId;

        // The prefab's default runtime controller (JebbyAnimator), captured at
        // Awake before any override is applied. Used to restore default.
        private RuntimeAnimatorController _defaultAnimatorController;
        private bool _subscribed;

        public string CurrentOutfitId => _lastAppliedOutfitId;
        public bool HasSpriteRenderer => _spriteRenderer != null;

        private void Awake()
        {
            if (_animator != null)
                _defaultAnimatorController = _animator.runtimeAnimatorController;
        }

        private void OnEnable()
        {
            if (_subscribed) return;
            WardrobeAppearanceEvents.EquippedOutfitChanged += OnEquippedOutfitChanged;
            _subscribed = true;
        }

        private void OnDisable() => Unsubscribe();
        private void OnDestroy() => Unsubscribe();

        private void Unsubscribe()
        {
            if (!_subscribed) return;
            WardrobeAppearanceEvents.EquippedOutfitChanged -= OnEquippedOutfitChanged;
            _subscribed = false;
        }

        private void Start()
        {
            if (_applyOnStart) ApplyEquippedOutfit();
        }

        private void OnEquippedOutfitChanged(WardrobeEquippedOutfitChanged change)
        {
            ApplyOutfit(change.CurrentOutfitId);
        }

        // Applies whatever outfit is currently equipped. Uses the migrator's
        // read-only "effective" id so an unsupported FUTURE save falls back to
        // Classic in memory without rewriting the save. Stars-free (no Stars
        // dependency here); a supported save was already lock-normalized at menu
        // init, so the stored id is trustworthy.
        public void ApplyEquippedOutfit()
            => ApplyOutfit(WardrobePersistenceMigrator.GetEffectiveOutfitId());

        // Applies the given outfit id: assigns its override if one exists,
        // otherwise restores the captured default controller (so a previous
        // override never lingers). null/unknown normalize to default via the
        // resolver.
        public void ApplyOutfit(string outfitId)
        {
            var def = OutfitVisualCatalog.GetVisualForOutfit(outfitId, _library);
            OutfitVisualApplier.Apply(_animator, _defaultAnimatorController, def);
            _lastAppliedOutfitId = def.OutfitId;
        }
    }
}
