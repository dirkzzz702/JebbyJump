using UnityEngine;

namespace JebbyJump.World
{
    // Clamps the Main Camera's X position after Cinemachine updates so the
    // camera edges never pass the world boundary walls.
    [DefaultExecutionOrder(100)]
    public class CameraXConfiner : MonoBehaviour
    {
        [SerializeField] private float _worldHalfWidth = 18.82f;

        private Camera _cam;

        private void Awake() => _cam = GetComponent<Camera>();

        private void LateUpdate()
        {
            if (_cam == null) return;
            float halfCamW = _cam.orthographicSize * _cam.aspect;
            float maxX     = Mathf.Max(0f, _worldHalfWidth - halfCamW);
            var   p        = transform.position;
            p.x            = Mathf.Clamp(p.x, -maxX, maxX);
            transform.position = p;
        }
    }
}
