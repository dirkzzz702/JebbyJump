using JebbyJump.UI;
using UnityEngine;

namespace JebbyJump.Items
{
    // Active for _durationSeconds. Consumed by the next life-loss event
    // (wrong landing / future-row skip / cactus). MemoryPhaseController
    // calls TryConsume() before LoseLife() and respawns Jebby without
    // losing a life if the shield was active.
    public class BubbleShieldEffect : ActiveSkillEffect
    {
        [SerializeField] private GameFeedbackUI _feedbackUI;
        [SerializeField] private float _durationSeconds = 6f;
        // Optional bubble sprite around Jebby (art batch 006). Purely visual:
        // mirrors the existing active state; null-safe so tests/scenes without
        // the visual behave exactly as before.
        [SerializeField] private GameObject _visual;

        private bool _isActive;
        private float _remaining;

        public override bool IsActive => _isActive;

        public override void Activate()
        {
            _isActive = true;
            _remaining = _durationSeconds;
            if (_visual != null) _visual.SetActive(true);
            _feedbackUI?.ShowMessage("Shield!", 1f);
            Debug.Log($"[BubbleShield] Activated for {_durationSeconds}s.");
        }

        public override void CancelEffect()
        {
            _isActive = false;
            _remaining = 0f;
            if (_visual != null) _visual.SetActive(false);
        }

        // Returns true if the shield absorbed the hit. Single-use per activation.
        public bool TryConsume()
        {
            if (!_isActive) return false;
            _isActive = false;
            _remaining = 0f;
            if (_visual != null) _visual.SetActive(false);
            _feedbackUI?.ShowMessage("Shield Saved You!", 1f);
            Debug.Log("[BubbleShield] Consumed — life loss prevented.");
            return true;
        }

        private void Update()
        {
            if (!_isActive) return;
            _remaining -= Time.deltaTime;
            if (_remaining <= 0f)
            {
                _isActive = false;
                if (_visual != null) _visual.SetActive(false);
                Debug.Log("[BubbleShield] Expired without being consumed.");
            }
        }
    }
}
