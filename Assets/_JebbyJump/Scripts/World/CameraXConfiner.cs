using UnityEngine;

namespace JebbyJump.World
{
    // Clamps the Main Camera's viewport so it never shows beyond the background
    // sprite bounds. Background must be a fixed scene object (not camera-following).
    // Runs after Cinemachine (DefaultExecutionOrder 100).
    [DefaultExecutionOrder(100)]
    public class CameraXConfiner : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _background;

        private Camera _cam;

        private void Awake() => _cam = GetComponent<Camera>();

        private void LateUpdate()
        {
            if (_cam == null || _background == null) return;

            float halfCamW = _cam.orthographicSize * _cam.aspect;
            float halfCamH = _cam.orthographicSize;

            // Horizontal
            float minX = _background.bounds.min.x + halfCamW;
            float maxX = _background.bounds.max.x - halfCamW;

            // Vertical
            float minY = _background.bounds.min.y + halfCamH;
            float maxY = _background.bounds.max.y - halfCamH;

            var p = transform.position;

            if (minX <= maxX)
                p.x = Mathf.Clamp(p.x, minX, maxX);

            if (minY <= maxY)
                p.y = Mathf.Clamp(p.y, minY, maxY);

            transform.position = p;
        }
    }
}
