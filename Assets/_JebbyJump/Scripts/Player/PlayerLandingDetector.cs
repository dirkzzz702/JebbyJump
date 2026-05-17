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

        private void OnMotorLanded(Collider2D groundCollider)
        {
            if (_motor != null && _motor.Velocity.y > 0f) return;
            var platform = groundCollider.GetComponentInParent<Platform>();
            if (platform == null || platform == _currentPlatform) return;
            if (!IsInsidePlatformBounds(groundCollider)) return;

            _currentPlatform = platform;
            Debug.Log($"Landed on platform: Row {platform.RowIndex}, Color {platform.Color}");
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
    }
}
