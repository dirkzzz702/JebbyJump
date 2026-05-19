using System.Collections.Generic;
using JebbyJump.Core;
using JebbyJump.Platforms;
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

        private void Awake()
        {
            if (_container == null)
                Debug.LogError("[SequenceDisplayUI] Container not assigned.", this);
        }

        public void Show(IReadOnlyList<PlatformColor> sequence)
        {
            if (_container == null) return;
            ClearSwatches();

            if (_container.TryGetComponent<HorizontalLayoutGroup>(out var hlg))
                hlg.spacing = _swatchSpacing;

            for (int i = 0; i < sequence.Count; i++)
            {
                var go = new GameObject($"Swatch_{i}", typeof(RectTransform), typeof(Image));
                go.transform.SetParent(_container, false);
                var rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(_swatchSize, _swatchSize);
                var img = go.GetComponent<Image>();
                if (_gemSprite != null) { img.sprite = _gemSprite; img.preserveAspect = true; }
                img.color = PlatformColorPalette.GetColor(sequence[i]);
                _swatches.Add(go);
            }

            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);

        private void ClearSwatches()
        {
            foreach (var s in _swatches) Destroy(s);
            _swatches.Clear();
        }
    }
}
