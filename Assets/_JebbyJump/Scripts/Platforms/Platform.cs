using JebbyJump.Core;
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

        private void Awake()
        {
            ApplyVisualColor();
            ApplyWidth();
        }

        private void OnValidate() => ApplyVisualColor();

        public void Initialize(PlatformColor color, int rowIndex)
        {
            _color = color;
            _rowIndex = rowIndex;
            ApplyVisualColor();
            ApplyWidth();
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
    }
}
