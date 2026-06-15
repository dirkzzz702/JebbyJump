using JebbyJump.Wardrobe.Visual;
using UnityEngine;

namespace JebbyJump.UI
{
    // Applies Screen.safeArea to a target RectTransform via the pure
    // SafeAreaCalculator. Runtime-only (no [ExecuteAlways]) so the scaffolded
    // scene stays full-stretch on disk; insets are applied at play time. Cheap
    // change-detection (no per-frame layout rebuild) updates on
    // resolution/orientation/safe-area change. Editor / no-display falls back to
    // full screen. Only touches the wired target's anchors/offsets.
    [DisallowMultipleComponent]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        [SerializeField] private RectTransform _target;

        private Rect _lastSafeArea;
        private Vector2 _lastScreen;

        private void OnEnable() => Apply();

        private void Update()
        {
            var screen = new Vector2(Screen.width, Screen.height);
            if (Screen.safeArea != _lastSafeArea || screen != _lastScreen)
                Apply();
        }

        public void Apply()
        {
            var rt = _target != null ? _target : transform as RectTransform;
            if (rt == null) return;

            Rect safe = Screen.safeArea;
            var screen = new Vector2(Screen.width, Screen.height);

            SafeAreaCalculator.ComputeAnchors(
                safe, screen, out var min, out var max);
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            _lastSafeArea = safe;
            _lastScreen = screen;
        }
    }
}
