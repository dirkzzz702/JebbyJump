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

        private Platform _currentPlatform;
        private Collider2D _playerCollider;

        private void Awake()
        {
            _playerCollider = GetComponent<Collider2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsTopContact(collision)) return;

            var platform = collision.collider.GetComponentInParent<Platform>();
            if (platform == null || platform == _currentPlatform) return;

            if (!IsInsidePlatformBounds(collision.collider)) return;

            _currentPlatform = platform;
            LandedOnPlatform?.Invoke(platform);
            Debug.Log($"Landed on platform: Row {platform.RowIndex}, Color {platform.Color}");
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            var platform = collision.collider.GetComponentInParent<Platform>();
            if (platform == _currentPlatform)
                _currentPlatform = null;
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

        private static bool IsTopContact(Collision2D collision)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f) return true;
            }
            return false;
        }
    }
}
