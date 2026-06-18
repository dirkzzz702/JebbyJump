using JebbyJump.Core;
using JebbyJump.Settings;
using TMPro;
using UnityEngine;

namespace JebbyJump.Platforms
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] private PlatformColor _color;
        [SerializeField] private int _rowIndex;
        [SerializeField] private float _width = 2f;

        public PlatformColor Color => _color;
        public int RowIndex => _rowIndex;

        // P22: opt-in non-color cue (world-space text), created at runtime so the
        // prefab needs no fragile TMP YAML. Sorted above the sprite, no collider
        // / not a physics or input participant, counter-scaled for the platform's
        // non-uniform localScale, and toggled by the Memory Cues setting.
        private const int CueSortingOrder = 10;
        private TextMeshPro _cueLabel;
        private bool _subscribed;

        private void Awake()
        {
            ApplyVisualColor();
            ApplyWidth();
            EnsureCueLabel();
            RefreshCue();
        }

        private void OnEnable()
        {
            if (!_subscribed)
            {
                AccessibilitySettingsStore.MemoryCuesChanged += OnMemoryCuesChanged;
                _subscribed = true;
            }
            ApplyCueVisibility(AccessibilitySettingsStore.MemoryCues);
        }

        private void OnDisable()
        {
            if (_subscribed)
            {
                AccessibilitySettingsStore.MemoryCuesChanged -= OnMemoryCuesChanged;
                _subscribed = false;
            }
        }

        private void OnValidate() => ApplyVisualColor();

        public void Initialize(PlatformColor color, int rowIndex)
        {
            _color = color;
            _rowIndex = rowIndex;
            ApplyVisualColor();
            ApplyWidth();
            RefreshCue();
        }

        public void SetWidth(float width)
        {
            _width = width;
            ApplyWidth();
        }

        private void ApplyVisualColor()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = PlatformColorPalette.GetColor(_color);
        }

        private void ApplyWidth()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.drawMode = SpriteDrawMode.Sliced;
                sr.size = new Vector2(_width, sr.size.y);
            }
            var col = GetComponent<BoxCollider2D>();
            if (col != null)
                col.size = new Vector2(_width, col.size.y);
        }

        private void EnsureCueLabel()
        {
            if (_cueLabel != null) return;
            var go = new GameObject("CueLabel", typeof(RectTransform));
            go.transform.SetParent(transform, false);
            _cueLabel = go.AddComponent<TextMeshPro>();
            _cueLabel.alignment = TextAlignmentOptions.Center;
            _cueLabel.fontSize = 6f;
            _cueLabel.fontStyle = FontStyles.Bold;
            _cueLabel.sortingOrder = CueSortingOrder; // above the platform sprite (order 0)
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(2f, 1.2f);
            rt.localPosition = Vector3.zero;
            go.SetActive(false);
        }

        // Sets the glyph + contrast color, undoes the parent's non-uniform scale
        // so the digit is not distorted, and applies current visibility.
        private void RefreshCue()
        {
            if (_cueLabel == null) return;
            _cueLabel.text = PlatformCueMapping.CueFor(_color);
            _cueLabel.color = ContrastColor(PlatformColorPalette.GetColor(_color));

            Vector3 lossy = transform.lossyScale;
            _cueLabel.transform.localScale = new Vector3(
                SafeInverse(lossy.x), SafeInverse(lossy.y), 1f);

            ApplyCueVisibility(AccessibilitySettingsStore.MemoryCues);
        }

        private void OnMemoryCuesChanged(bool on) => ApplyCueVisibility(on);

        private void ApplyCueVisibility(bool on)
        {
            if (_cueLabel == null) return;
            _cueLabel.gameObject.SetActive(on && PlatformCueMapping.HasCue(_color));
        }

        private static float SafeInverse(float v)
            => Mathf.Approximately(v, 0f) ? 1f : 1f / v;

        // The existing public Color property shadows UnityEngine.Color inside
        // this type, so the contrast helper qualifies it explicitly.
        private static UnityEngine.Color ContrastColor(UnityEngine.Color bg)
        {
            float luma = 0.299f * bg.r + 0.587f * bg.g + 0.114f * bg.b;
            return luma > 0.5f ? UnityEngine.Color.black : UnityEngine.Color.white;
        }
    }
}
