using System;
using JebbyJump.Platforms;
using UnityEngine;

namespace JebbyJump.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerLandingDetector : MonoBehaviour
    {
        public event Action<Platform> LandedOnPlatform;

        // One-way platforms can cause a brief contact flicker (exit + re-enter) as Jebby
        // settles, which would double-fire the landing event. This cooldown suppresses
        // re-contact on the same platform within the threshold window.
        private const float SamePlatformCooldown = 0.5f;

        private Platform _currentPlatform;
        private Platform _recentPlatform;
        private float _recentLandingTime;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsTopContact(collision)) return;

            var platform = collision.collider.GetComponentInParent<Platform>();
            if (platform == null) return;
            if (platform == _currentPlatform) return;
            if (platform == _recentPlatform && Time.time - _recentLandingTime < SamePlatformCooldown) return;

            _currentPlatform = platform;
            _recentPlatform = platform;
            _recentLandingTime = Time.time;

            LandedOnPlatform?.Invoke(platform);
            Debug.Log($"Landed on platform: Row {platform.RowIndex}, Color {platform.Color}");
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            var platform = collision.collider.GetComponentInParent<Platform>();
            if (platform == _currentPlatform)
                _currentPlatform = null;
            // _recentPlatform and _recentLandingTime are intentionally kept to enforce the cooldown.
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
