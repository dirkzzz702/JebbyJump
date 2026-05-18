using System.Collections;
using JebbyJump.UI;
using UnityEngine;

namespace JebbyJump.Items
{
    // Phase 20: one temporary movement effect at a time.
    // Future item stacking should use a proper modifier system.
    public class RocketBootsEffect : MonoBehaviour
    {
        [SerializeField] private Player.PlayerStats _stats;
        [SerializeField] private GameFeedbackUI _feedbackUI;
        [SerializeField] private float _jumpMultiplier      = 1.15f;
        [SerializeField] private float _moveSpeedMultiplier = 1.20f;
        [SerializeField] private float _durationSeconds     = 4f;

        private float _storedJumpForce;
        private float _storedMoveSpeed;
        private bool _isActive;
        private Coroutine _effectRoutine;

        public bool IsActive => _isActive;

        public void Activate()
        {
            CancelEffect();
            _effectRoutine = StartCoroutine(RunEffect());
        }

        public void CancelEffect()
        {
            if (_effectRoutine != null)
            {
                StopCoroutine(_effectRoutine);
                _effectRoutine = null;
            }
            if (_isActive) RestoreStats();
            _isActive = false;
        }

        private void RestoreStats()
        {
            if (_stats == null) return;
            _stats.JumpForce = _storedJumpForce;
            _stats.MoveSpeed = _storedMoveSpeed;
        }

        private IEnumerator RunEffect()
        {
            if (_stats == null) yield break;
            _storedJumpForce = _stats.JumpForce;
            _storedMoveSpeed = _stats.MoveSpeed;
            _isActive = true;
            _stats.JumpForce = _storedJumpForce * _jumpMultiplier;
            _stats.MoveSpeed = _storedMoveSpeed * _moveSpeedMultiplier;
            Debug.Log($"[RocketBoots] Activated for {_durationSeconds}s. JumpForce: {_storedJumpForce:F1}→{_stats.JumpForce:F1}  MoveSpeed: {_storedMoveSpeed:F1}→{_stats.MoveSpeed:F1}");
            _feedbackUI?.ShowMessage("Rocket Boots!", 1.5f);
            yield return new WaitForSeconds(_durationSeconds);
            RestoreStats();
            _isActive = false;
            _effectRoutine = null;
            Debug.Log("[RocketBoots] Expired. Stats restored.");
        }
    }
}
