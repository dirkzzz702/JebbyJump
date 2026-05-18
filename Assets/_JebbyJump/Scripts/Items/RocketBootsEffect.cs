using System.Collections;
using JebbyJump.UI;
using UnityEngine;

namespace JebbyJump.Items
{
    // Phase 19: one temporary jump effect at a time.
    // Future item stacking should use a proper modifier system.
    public class RocketBootsEffect : MonoBehaviour
    {
        [SerializeField] private Player.PlayerStats _stats;
        [SerializeField] private GameFeedbackUI _feedbackUI;
        [SerializeField] private float _jumpMultiplier = 1.35f;
        [SerializeField] private float _durationSeconds = 5f;

        private Coroutine _effectCoroutine;
        private float _storedJumpForce;
        private bool _isActive;

        public void Activate()
        {
            // Cancel any running effect first, restoring stored force before capturing new baseline.
            if (_effectCoroutine != null)
            {
                StopCoroutine(_effectCoroutine);
                _effectCoroutine = null;
            }
            if (_isActive && _stats != null)
            {
                _stats.JumpForce = _storedJumpForce;
                _isActive = false;
            }
            _effectCoroutine = StartCoroutine(RunEffect());
        }

        public void CancelEffect()
        {
            if (_effectCoroutine != null)
            {
                StopCoroutine(_effectCoroutine);
                _effectCoroutine = null;
            }
            if (_isActive && _stats != null)
            {
                _stats.JumpForce = _storedJumpForce;
                _isActive = false;
            }
        }

        private IEnumerator RunEffect()
        {
            if (_stats == null) yield break;
            _storedJumpForce = _stats.JumpForce;
            _isActive = true;
            _stats.JumpForce = _storedJumpForce * _jumpMultiplier;
            Debug.Log($"[RocketBoots] Activated for {_durationSeconds}s. JumpForce: {_storedJumpForce:F1} → {_stats.JumpForce:F1}");
            _feedbackUI?.ShowMessage("Rocket Boots!", 1.5f);
            yield return new WaitForSeconds(_durationSeconds);
            _stats.JumpForce = _storedJumpForce;
            _isActive = false;
            _effectCoroutine = null;
            Debug.Log("[RocketBoots] Expired. JumpForce restored.");
        }
    }
}
