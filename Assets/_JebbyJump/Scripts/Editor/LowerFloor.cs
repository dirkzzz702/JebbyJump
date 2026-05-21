using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Lower the origin floor by 0.4 units to improve scene composition while
// keeping the collider local shape + scale and gameplay behavior unchanged.
//   Floor.transform.y: -0.75  →  -1.15   (floor top surface: -0.5 → -0.9)
//   Jebby.transform.y: -0.63  →  -1.03   (capsule bottom = floor top)
//
// Jump-feasibility math: v=10, g=9.81, peak ≈ 5.1u.
// Row 0 platform top ≈ y=2.33; distance from new floor top -0.9 = 3.23u → safe.
public static class LowerFloor
{
    private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
    private const float FloorTargetY = -1.15f;
    private const float JebbyTargetY = -1.03f;

    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        var floor = FindInActiveScene("Floor");
        if (floor == null) { Debug.LogError("[LowerFloor] Floor not found."); return; }

        var jebby = FindInActiveScene("Jebby");
        if (jebby == null) { Debug.LogError("[LowerFloor] Jebby not found."); return; }

        // Move Floor — preserve scale and collider local shape.
        var fp = floor.transform.position;
        Debug.Log($"[LowerFloor] Floor y: {fp.y:F3} → {FloorTargetY:F3}");
        fp.y = FloorTargetY;
        floor.transform.position = fp;
        EditorUtility.SetDirty(floor);

        // Adjust Jebby spawn so capsule bottom rests on new floor top.
        var jp = jebby.transform.position;
        Debug.Log($"[LowerFloor] Jebby y: {jp.y:F3} → {JebbyTargetY:F3}");
        jp.y = JebbyTargetY;
        jebby.transform.position = jp;
        EditorUtility.SetDirty(jebby);

        // Verify capsule bottom math
        var cap = jebby.GetComponentInChildren<CapsuleCollider2D>(true);
        if (cap != null)
        {
            float capBottomLocal = cap.offset.y - cap.size.y * 0.5f;
            float capBottomWorld = jebby.transform.position.y + capBottomLocal * jebby.transform.localScale.y;
            Debug.Log($"[LowerFloor] Verification: capsule bottom world y = {capBottomWorld:F3} (should be -0.900)");
        }

        // Verify background still covers the new floor (no clipping)
        var bg = FindInActiveScene("Background");
        if (bg != null)
        {
            var sr = bg.GetComponent<SpriteRenderer>();
            if (sr != null)
                Debug.Log($"[LowerFloor] Background bounds.min.y = {sr.bounds.min.y:F2} (must be < floor top -0.9)");
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[LowerFloor] Done.");
    }

    private static GameObject FindInActiveScene(string name)
    {
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in all)
        {
            if (go.name != name) continue;
            if (!go.scene.IsValid()) continue;
            return go;
        }
        return null;
    }
}
