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

        private bool _isActive;
        private float _remaining;

        public override bool IsActive => _isActive;

        public override void Activate()
        {
            _isActive = true;
            _remaining = _durationSeconds;
            _feedbackUI?.ShowMessage("Shield!", 1f);
            Debug.Log($"[BubbleShield] Activated for {_durationSeconds}s.");
        }

        public override void CancelEffect()
        {
            _isActive = false;
            _remaining = 0f;
        }

        // Returns true if the shield absorbed the hit. Single-use per activation.
        public bool TryConsume()
        {
            if (!_isActive) return false;
            _isActive = false;
            _remaining = 0f;
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
                Debug.Log("[BubbleShield] Expired without being consumed.");
            }
        }
    }
}
