using System.Collections.Generic;
using JebbyJump.Core;
using JebbyJump.Platforms;
using JebbyJump.Settings;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.Sequence
{
    public class SequenceDisplayUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private Sprite _gemSprite;
        [SerializeField] private float _swatchSize = 60f;
        [SerializeField] private float _swatchSpacing = 12f;

        private readonly List<GameObject> _swatches = new();
        private readonly List<GameObject> _cueLabels = new();

        private static readonly ProfilerMarker s_Show = new ProfilerMarker("JebbyJump.Memory.BuildSwatches");

        private void Awake()
        {
            if (_container == null)
                Debug.LogError("[SequenceDisplayUI] Container not assigned.", this);
        }

        // P22: live-toggle the opt-in non-color memory cue. Subscribed once per
        // active period (paired with OnDisable) so repeated sequences never
        // stack duplicate handlers.
        private void OnEnable()
        {
            AccessibilitySettingsStore.MemoryCuesChanged += OnMemoryCuesChanged;
        }

        private void OnDisable()
        {
            AccessibilitySettingsStore.MemoryCuesChanged -= OnMemoryCuesChanged;
        }

        public void Show(IReadOnlyList<PlatformColor> sequence)
        {
            using var _ = s_Show.Auto();
            if (_container == null) return;
            ClearSwatches();

            if (_container.TryGetComponent<HorizontalLayoutGroup>(out var hlg))
                hlg.spacing = _swatchSpacing;

            bool cuesOn = AccessibilitySettingsStore.MemoryCues;
            for (int i = 0; i < sequence.Count; i++)
            {
                var go = new GameObject($"Swatch_{i}", typeof(RectTransform), typeof(Image));
                go.transform.SetParent(_container, false);
                var rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(_swatchSize, _swatchSize);
                var img = go.GetComponent<Image>();
                if (_gemSprite != null) { img.sprite = _gemSprite; img.preserveAspect = true; }
                Color swatchColor = PlatformColorPalette.GetColor(sequence[i]);
                img.color = swatchColor;
                _swatches.Add(go);

                CreateCueLabel(rt, sequence[i], swatchColor, cuesOn);
            }

            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);

        // The opt-in non-color cue label as a child of the swatch. The SAME
        // PlatformCueMapping is used by the spawned platforms so presentation
        // and response always show identical cues. raycastTarget off => no input
        // impact; contrast color chosen from the swatch luminance.
        private void CreateCueLabel(RectTransform swatch, PlatformColor color, Color swatchColor, bool visible)
        {
            string cue = PlatformCueMapping.CueFor(color);
            if (string.IsNullOrEmpty(cue)) return;

            var labelGO = new GameObject("Cue", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelGO.transform.SetParent(swatch, false);
            var lrt = labelGO.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;

            var tmp = labelGO.GetComponent<TextMeshProUGUI>();
            tmp.text = cue;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = _swatchSize * 0.6f;
            tmp.fontStyle = FontStyles.Bold;
            tmp.raycastTarget = false;
            tmp.color = ContrastColor(swatchColor);

            labelGO.SetActive(visible);
            _cueLabels.Add(labelGO);
        }

        private void OnMemoryCuesChanged(bool on)
        {
            foreach (var label in _cueLabels)
                if (label != null) label.SetActive(on);
        }

        // Black on light swatches, white on dark (Rec. 601 luma).
        private static Color ContrastColor(Color bg)
        {
            float luma = 0.299f * bg.r + 0.587f * bg.g + 0.114f * bg.b;
            return luma > 0.5f ? Color.black : Color.white;
        }

        private void ClearSwatches()
        {
            foreach (var s in _swatches) Destroy(s);
            _swatches.Clear();
            _cueLabels.Clear(); // labels are children of swatches, destroyed with them
        }
    }
}
