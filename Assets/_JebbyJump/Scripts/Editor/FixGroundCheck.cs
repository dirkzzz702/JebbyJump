using UnityEngine;
using UnityEditor;

// After Jebby sprites changed from bottom pivot to center pivot:
// - Sprite naturalHeight ≈ 14.02 local units
// - At scale 0.13: world half-height ≈ 0.911
// - CapsuleCollider2D offset.y = -6.51 puts capsule bottom at sprite feet
// - GroundCheck must be slightly below capsule bottom
//   old local y: -0.55 (for bottom-pivot setup)
//   new local y: -(0.911 + 0.0715) / 0.13 = -7.557
public static class FixGroundCheck
{
    private const string PrefabPath = "Assets/_JebbyJump/Prefabs/Player/Jebby.prefab";

    public static void Execute()
    {
        // First just print what we find
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null) { Debug.LogError("[GC] Jebby prefab not found."); return; }

        foreach (Transform child in prefab.GetComponentsInChildren<Transform>(true))
        {
            Debug.Log($"[GC] child: {child.name}  localPos={child.localPosition}");
        }
    }

    public static void Apply()
    {
        using var scope = new PrefabUtility.EditPrefabContentsScope(PrefabPath);
        if (scope.prefabContentsRoot == null) { Debug.LogError("[GC] Prefab not found."); return; }

        // Locate the GroundCheck child (name search, case-insensitive)
        Transform groundCheck = null;
        foreach (Transform child in scope.prefabContentsRoot.GetComponentsInChildren<Transform>(true))
        {
            if (child.name.ToLower().Contains("groundcheck") || child.name.ToLower().Contains("ground_check"))
            {
                groundCheck = child;
                break;
            }
        }

        if (groundCheck == null)
        {
            Debug.LogError("[GC] GroundCheck child not found on Jebby prefab.");
            return;
        }

        // After center pivot:
        //   capsule bottom world offset from transform = offset.y_world - half_height_world
        //                                              = -6.51*0.13 - 0.5*0.13 = -0.846 - 0.065 = -0.911
        //   GroundCheck should be ~0.0715 below capsule bottom  (same gap as before: 0.55 local = 0.0715 world)
        //   GroundCheck world offset = -0.911 - 0.0715 = -0.9825
        //   GroundCheck local y     = -0.9825 / 0.13 = -7.558
        const float newLocalY = -7.558f;
        var pos = groundCheck.localPosition;
        Debug.Log($"[GC] GroundCheck current localPos={pos}  → new y={newLocalY}");
        groundCheck.localPosition = new Vector3(pos.x, newLocalY, pos.z);
        Debug.Log("[GC] GroundCheck updated.");
    }
}
