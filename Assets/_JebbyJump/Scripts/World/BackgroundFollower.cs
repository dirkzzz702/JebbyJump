using UnityEngine;

namespace JebbyJump.World
{
    // Keeps the background centred on the camera horizontally so it always
    // covers the viewport regardless of how far left or right Jebby moves.
    public class BackgroundFollower : MonoBehaviour
    {
        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
        }

        private void LateUpdate()
        {
            if (_cam == null) return;
            var p = transform.position;
            p.x = _cam.transform.position.x;
            transform.position = p;
        }
    }
}
