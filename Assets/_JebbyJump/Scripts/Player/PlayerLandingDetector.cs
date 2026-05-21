using System;
using JebbyJump.Platforms;
using UnityEngine;

namespace JebbyJump.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerLandingDetector : MonoBehaviour
    {
        public event Action<Platform> LandedOnPlatform;

        [SerializeField] private float _horizontalMargin = 0.15f;
        [Tooltip("How far below the platform top surface Jebby's feet may be and still count as a landing. Prevents false landings from chest/body clip-through.")]
        [SerializeField] private float _verticalLandingTolerance = 0.08f;
        [SerializeField] private bool _debugLanding;

        private Collider2D _playerCollider;
        private PlayerMotor _motor;
        private Platform _currentPlatform;

        private void Awake()
        {
            _playerCollider = GetComponent<Collider2D>();
            _motor = GetComponent<PlayerMotor>();
            if (_motor == null)
                Debug.LogError("[PlayerLandingDetector] PlayerMotor not found on same GameObject.", this);
        }

        private void OnEnable()
        {
            if (_motor != null) _motor.Landed += OnMotorLanded;
        }

        private void OnDisable()
        {
            if (_motor != null) _motor.Landed -= OnMotorLanded;
        }

        public void ResetCurrentPlatform()
        {
            _currentPlatform = null;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            var platform = collision.collider.GetComponentInParent<Platform>();
            if (platform == _currentPlatform)
                _currentPlatform = null;
        }

        // Edge of grounded — first frame Jebby's ground check overlaps a platform.
        private void OnMotorLanded(Collider2D groundCollider)
        {
            TryProcessLanding(groundCollider, "motor-landed");
        }

        // Grounded recheck — covers the case where the rising-edge landing was
        // rejected (e.g. edge/side-slide failed the horizontal bounds check) but
        // Jebby has since slid fully onto the platform top while still grounded.
        private void FixedUpdate()
        {
            if (_motor == null || !_motor.IsGrounded) return;
            var col = _motor.CurrentGroundCollider;
            if (col == null) return;

            // Skip if we already accepted this platform.
            var platform = col.GetComponentInParent<Platform>();
            if (platform == null || platform == _currentPlatform) return;

            TryProcessLanding(col, "grounded-recheck");
        }

        private void TryProcessLanding(Collider2D groundCollider, string source)
        {
            if (groundCollider == null)
            {
                Log("reject: null ground collider", source);
                return;
            }

            // Reject upward / apex false positives.
            if (_motor != null && _motor.Velocity.y > 0.1f)
            {
                Log("reject: upward / not falling enough", source);
                return;
            }

            var platform = groundCollider.GetComponentInParent<Platform>();
            if (platform == null)
            {
                Log("reject: no platform", source);
                return;
            }
            if (platform == _currentPlatform)
            {
                Log("reject: same current platform", source);
                return;
            }
            if (!IsInsidePlatformBounds(groundCollider))
            {
                Log("reject: outside horizontal bounds", source);
                return;
            }
            if (!IsAbovePlatformTop(groundCollider))
            {
                Log("reject: below platform top (clip-through from below)", source);
                return;
            }

            _currentPlatform = platform;
            Debug.Log($"Landed on platform: Row {platform.RowIndex}, Color {platform.Color}  (source: {source})");
            LandedOnPlatform?.Invoke(platform);
        }

        private bool IsInsidePlatformBounds(Collider2D platformCollider)
        {
            float playerX = _playerCollider != null
                ? _playerCollider.bounds.center.x
                : transform.position.x;
            Bounds bounds = platformCollider.bounds;
            return playerX > bounds.min.x + _horizontalMargin
                && playerX < bounds.max.x - _horizontalMargin;
        }

        // Jebby's feet must be on (or above with tolerance) the platform top
        // surface. Rejects landings registered while Jebby's body is clipping
        // through the one-way platform from below.
        private bool IsAbovePlatformTop(Collider2D platformCollider)
        {
            if (_playerCollider == null) return true;
            float playerBottomY = _playerCollider.bounds.min.y;
            float platformTopY  = platformCollider.bounds.max.y;
            return playerBottomY >= platformTopY - _verticalLandingTolerance;
        }

        private void Log(string msg, string source)
        {
            if (!_debugLanding) return;
            Debug.Log($"[LandingDetector] {msg}  (source: {source})", this);
        }
    }
}
