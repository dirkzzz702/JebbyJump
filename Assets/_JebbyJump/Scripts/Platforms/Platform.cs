using JebbyJump.Core;
using UnityEngine;

namespace JebbyJump.Platforms
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] private PlatformColor _color;
        [SerializeField] private int _rowIndex;

        public PlatformColor Color => _color;
        public int RowIndex => _rowIndex;

        private void Awake() => ApplyVisualColor();

        private void OnValidate() => ApplyVisualColor();

        private void ApplyVisualColor()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = PlatformColorPalette.GetColor(_color);
        }
    }
}
