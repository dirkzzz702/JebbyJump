using UnityEngine;
using UnityEngine.Rendering;

namespace JebbyJump.World
{
    // Clamps the Main Camera's X so it never pans past the level's horizontal
    // extent (the floor spans +/- _worldHalfWidth). Runs via
    // RenderPipelineManager.beginCameraRendering so it fires after Cinemachine has
    // written its final position for the frame.
    //
    // NOTE: this used to derive its bounds from a background SpriteRenderer, but the
    // warm-palette pass (WireGameBackground) parents the background under the camera
    // so it always fills the view - which made its bounds move WITH the camera and
    // the clamp a no-op (the player could then walk off the right edge of the floor).
    // Hence an explicit fixed WORLD bound, independent of the camera-locked art.
    [DefaultExecutionOrder(100)]
    public class CameraXConfiner : MonoBehaviour
    {
        // Half the level's world width. The floor collider spans +/- this
        // (Floor scale.x 37.64, box size 1 -> +/-18.82). Same across worlds.
        [SerializeField] private float _worldHalfWidth = 18.82f;

        private Camera _cam;

        private void Awake() => _cam = GetComponent<Camera>();

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }

        private void OnBeginCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            if (cam != _cam) return;

            float halfCamW = _cam.orthographicSize * _cam.aspect;
            float minX = -_worldHalfWidth + halfCamW;
            float maxX = _worldHalfWidth - halfCamW;
            if (minX > maxX) return; // level narrower than the view - leave as-is

            var p = transform.position;
            p.x = Mathf.Clamp(p.x, minX, maxX);
            transform.position = p;
        }
    }
}
