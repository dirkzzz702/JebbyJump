using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Pivot standard: all game sprites use (0.5, 0) = bottom-centre.
// This means transform.position.y = bottom edge of the sprite in world space.
//
// Consequences:
//   Background   – transform.y = visual bottom. Reposition in scene accordingly.
//   Platform     – transform.y = sprite canvas bottom (transparent area below bar).
//                  BoxCollider2D offset centres on the bar; size covers bar height.
//   Jebby chars  – transform.y = sprite bottom (feet).
//                  CapsuleCollider2D offset.y = 0.5 → capsule bottom at feet.
//                  GroundCheck local y = -0.55 (0.0715 world below feet).
public static class FixSpritePivotsAndColliders
{
    // Platform sprite: 2508×627px, bar 200px centred (213px transparent below bar, 214 above)
    private const float CanvasH    = 627f;
    private const float BarH       = 200f;
    private const float BarBelowPx = 214f;  // transparent pixels below bar (213px above, 214px below)

    // Bar dimensions in normalised local space (sr.size.y = 1.0)
    private static float BarFraction  => BarH    / CanvasH;  // ≈ 0.319
    private static float BarCentreY   => (BarBelowPx + BarH * 0.5f) / CanvasH; // ≈ 0.501

    public static void Execute()
    {
        SetAllSpritePivots();
        FixPlatformCollider();
        FixJebbyCapsule();
        FixBackgroundPosition();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Fix] All done — pivots (0.5,0), colliders corrected, background repositioned.");
    }

    // ── Sprite pivots ─────────────────────────────────────────────────────────
    // UI sprites → (0.5, 0.5) centre.
    // All other game sprites → (0.5, 0) bottom-centre.
    private static void SetAllSpritePivots()
    {
        string[] guids = AssetDatabase.FindAssets(
            "t:Texture2D", new[] { "Assets/_JebbyJump/Art/Sprites" });

        int changed = 0;

        foreach (string guid in guids)
        {
            string path  = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;

            // UI sprites use centre pivot; game-world sprites use bottom-centre.
            bool isUI   = path.Contains("/Sprites/UI/");
            var target  = isUI ? new Vector2(0.5f, 0.5f) : new Vector2(0.5f, 0f);
            int alignID = isUI ? (int)SpriteAlignment.Center : (int)SpriteAlignment.BottomCenter;

            bool dirty = false;

            if (importer.spriteImportMode == SpriteImportMode.Single)
            {
                if (importer.spritePivot != target)
                {
                    importer.spritePivot = target;
                    dirty = true;
                }
            }
            else if (importer.spriteImportMode == SpriteImportMode.Multiple)
            {
                var sheet = importer.spritesheet;
                bool any  = false;
                for (int i = 0; i < sheet.Length; i++)
                {
                    var meta = sheet[i];
                    if (meta.alignment != alignID || meta.pivot != target)
                    {
                        meta.alignment = alignID;
                        meta.pivot     = target;
                        sheet[i]       = meta;
                        any            = true;
                    }
                }
                if (any) { importer.spritesheet = sheet; dirty = true; }
            }

            if (dirty)
            {
                importer.SaveAndReimport();
                changed++;
                string label = isUI ? "center" : "bottom-center";
                Debug.Log($"[Fix] Pivot → ({label}): {path}");
            }
        }
        Debug.Log($"[Fix] {changed} sprite(s) updated.");
    }

    // ── Platform BoxCollider2D ────────────────────────────────────────────────
    // With (0.5,0) pivot, transform.y = canvas bottom.
    // Bar centre in local y = BarCentreY ≈ 0.501.
    // Bar height in local y = BarFraction ≈ 0.319.
    private static void FixPlatformCollider()
    {
        const string path = "Assets/_JebbyJump/Prefabs/Platforms/Platform.prefab";
        using var scope   = new PrefabUtility.EditPrefabContentsScope(path);
        if (scope.prefabContentsRoot == null)
        { Debug.LogError("[Fix] Platform prefab not found: " + path); return; }

        var bc = scope.prefabContentsRoot.GetComponent<BoxCollider2D>();
        if (bc == null) { Debug.LogError("[Fix] No BoxCollider2D on Platform prefab."); return; }

        bc.size   = new Vector2(bc.size.x, BarFraction);
        bc.offset = new Vector2(bc.offset.x, BarCentreY);
        Debug.Log($"[Fix] Platform BoxCollider2D: size.y={BarFraction:F4}  offset.y={BarCentreY:F4}");
    }

    // ── Jebby CapsuleCollider2D + GroundCheck ─────────────────────────────────
    // With (0.5,0) pivot, transform.y = sprite bottom = feet.
    // Capsule size (0.5, 1.0) at scale 0.13 → half-height = 0.065 world.
    // offset.y = 0.5 → capsule centre = feet + 0.065 → capsule bottom = feet. ✓
    // GroundCheck local y = -0.55 → world offset = -0.0715 below feet. ✓
    private static void FixJebbyCapsule()
    {
        const string path = "Assets/_JebbyJump/Prefabs/Player/Jebby.prefab";
        using var scope   = new PrefabUtility.EditPrefabContentsScope(path);
        if (scope.prefabContentsRoot == null)
        { Debug.LogError("[Fix] Jebby prefab not found: " + path); return; }

        var root = scope.prefabContentsRoot;

        // Capsule
        var cap = root.GetComponentInChildren<CapsuleCollider2D>(true);
        if (cap != null)
        {
            cap.offset = new Vector2(cap.offset.x, 0.5f);
            Debug.Log("[Fix] Jebby CapsuleCollider2D.offset.y = 0.5");
        }
        else Debug.LogWarning("[Fix] CapsuleCollider2D not found on Jebby prefab.");

        // GroundCheck
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name.ToLower().Contains("ground"))
            {
                t.localPosition = new Vector3(t.localPosition.x, -0.55f, t.localPosition.z);
                Debug.Log($"[Fix] GroundCheck.localY = -0.55");
                break;
            }
        }
    }

    // ── Background scene object ───────────────────────────────────────────────
    // With bottom-centre pivot (0.5, 0), transform.y = sprite's bottom edge.
    // The background visual centre is designed to sit at world y = 7.
    // So: transform.y = 7 − naturalH*scale/2  (idempotent — absolute target).
    private const float BgVisualCentreY = 7f;

    private static void FixBackgroundPosition()
    {
        var bg = GameObject.Find("Background");
        if (bg == null)
        {
            Debug.LogWarning("[Fix] Background not found in active scene. Open Game.unity first.");
            return;
        }

        var sr = bg.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
        {
            Debug.LogWarning("[Fix] Background has no SpriteRenderer or sprite.");
            return;
        }

        float naturalH    = sr.sprite.bounds.size.y;   // full sprite height in Unity units
        float scaleY      = bg.transform.localScale.y;
        float halfHeightW = naturalH * scaleY * 0.5f;
        float targetY     = BgVisualCentreY - halfHeightW; // absolute bottom-edge world y

        var p = bg.transform.position;
        if (Mathf.Approximately(p.y, targetY)) { Debug.Log("[Fix] Background y already correct."); return; }

        float oldY = p.y;
        p.y = targetY;
        bg.transform.position = p;
        EditorSceneManager.MarkSceneDirty(bg.scene);
        Debug.Log($"[Fix] Background y: {oldY:F2} → {targetY:F2}  (centre={BgVisualCentreY}, halfH={halfHeightW:F2})");
    }
}
