using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Reframe the camera so Jebby sits ~35% from the bottom of the viewport
// (more headroom above for upcoming platforms). Background + floor stay
// fixed world objects; bounds clamping via CameraXConfiner is unaffected.
//
// Math (orthoSize = 7, halfH = 7, full vertical = 14):
//   35% from bottom = 0.15 below screen-centre = ScreenPosition.y = -0.15
//   With TargetOffset.y = 0 → cam.y = Jebby.y + 2.1
//   Camera Y range needed: cam.y stays within [-16.44, 36.44] (confiner).
//   At Jebby on floor (y ≈ -1.33), cam.y = 0.77 → well inside.
//   At top platform Row3 (y ≈ 12.5), cam.y = 14.6 → well inside.
public static class AdjustCameraFraming
{
    private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
    // CM 3 PositionComposer: +Y moves the camera UP in world space, which
    // makes the tracked target appear LOWER in the frame. (Empirically verified:
    // -0.15 placed Jebby at ~65% from bottom; +0.15 places him at ~35%.)
    private const float ScreenY = 0.15f; // → Jebby ~35% from bottom

    public static void Execute()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("[CamFraming] Cannot run in Play Mode. Stop play and retry.");
            return;
        }
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        var composer = Object.FindAnyObjectByType<CinemachinePositionComposer>(FindObjectsInactive.Include);
        if (composer == null)
        {
            Debug.LogError("[CamFraming] CinemachinePositionComposer not found on the virtual camera.");
            return;
        }

        var so = new SerializedObject(composer);
        var screenPos  = so.FindProperty("Composition.ScreenPosition");
        var targetOff  = so.FindProperty("TargetOffset");
        if (screenPos == null || targetOff == null)
        {
            Debug.LogError("[CamFraming] Could not locate ScreenPosition / TargetOffset properties.");
            return;
        }

        screenPos.vector2Value = new Vector2(0f, ScreenY);
        targetOff.vector3Value = Vector3.zero;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(composer);

        Debug.Log($"[CamFraming] ScreenPosition.y = {ScreenY} (Jebby ≈ 35% from bottom). " +
                  $"TargetOffset cleared. CameraXConfiner still clamps via beginCameraRendering.");

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
    }
}
