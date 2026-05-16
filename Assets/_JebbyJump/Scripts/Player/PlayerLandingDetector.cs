using System;
using System.Collections;
using JebbyJump.Platforms;
using UnityEngine;

namespace JebbyJump.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerLandingDetector : MonoBehaviour
    {
        public event Action<Platform> LandedOnPlatform;

        private const float ConfirmationDelay = 0.1f;

        private Platform _currentPlatform;   // confirmed landing
        private Platform _pendingPlatform;   // waiting for confirmation
        private Coroutine _confirmationRoutine;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsTopContact(collision)) return;

            var platform = collision.collider.GetComponentInParent<Platform>();
            if (platform == null || platform == _currentPlatform) return;

            // Cancel pending confirmation for a different platform
            if (_confirmationRoutine != null)
            {
                StopCoroutine(_confirmationRoutine);
                _confirmationRoutine = null;
                _pendingPlatform = null;
            }

            _pendingPlatform = platform;
            _confirmationRoutine = StartCoroutine(ConfirmLanding(platform));
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            var platform = collision.collider.GetComponentInParent<Platform>();

            if (platform == _pendingPlatform)
            {
                // Left before confirmation window closed — cancel
                if (_confirmationRoutine != null)
                {
                    StopCoroutine(_confirmationRoutine);
                    _confirmationRoutine = null;
                }
                _pendingPlatform = null;
            }

            if (platform == _currentPlatform)
                _currentPlatform = null;
        }

        private IEnumerator ConfirmLanding(Platform platform)
        {
            yield return new WaitForSeconds(ConfirmationDelay);

            if (_pendingPlatform != platform) yield break;

            _currentPlatform = platform;
            _pendingPlatform = null;
            _confirmationRoutine = null;

            LandedOnPlatform?.Invoke(platform);
            Debug.Log($"Landed on platform: Row {platform.RowIndex}, Color {platform.Color}");
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
