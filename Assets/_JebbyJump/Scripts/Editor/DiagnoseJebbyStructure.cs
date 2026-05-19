using UnityEngine;
using UnityEditor;

// Inspects Jebby prefab hierarchy to find which GameObject holds the SpriteRenderer,
// then computes the correct CapsuleCollider2D offset and GroundCheck position so that
// (a) capsule bottom == sprite feet, (b) GroundCheck is just below capsule bottom.
public static class DiagnoseJebbyStructure
{
    private const string PrefabPath = "Assets/_JebbyJump/Prefabs/Player/Jebby.prefab";

    public static void Execute()
    {
        using var scope = new PrefabUtility.EditPrefabContentsScope(PrefabPath);
        var root = scope.prefabContentsRoot;
        if (root == null) { Debug.LogError("[Jeb] Prefab not found."); return; }

        Debug.Log($"[Jeb] Root: name={root.name}  localPos={root.transform.localPosition}  scale={root.transform.localScale}");

        // Find SpriteRenderer
        var sr = root.GetComponentInChildren<SpriteRenderer>(true);
        if (sr != null)
        {
            Debug.Log($"[Jeb] SpriteRenderer on: '{sr.gameObject.name}'  " +
                      $"localPos={sr.transform.localPosition}  " +
                      $"sprite={(sr.sprite != null ? sr.sprite.name : "null")}  " +
                      $"bounds={(sr.sprite != null ? sr.sprite.bounds.size.ToString() : "n/a")}");
        }

        // Find CapsuleCollider2D
        var cap = root.GetComponentInChildren<CapsuleCollider2D>(true);
        if (cap != null)
        {
            Debug.Log($"[Jeb] CapsuleCollider2D on: '{cap.gameObject.name}'  " +
                      $"size={cap.size}  offset={cap.offset}");
        }

        // Find GroundCheck
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name.ToLower().Contains("ground"))
                Debug.Log($"[Jeb] GroundCheck: '{t.name}'  localPos={t.localPosition}  parent='{t.parent?.name}'");
        }

        // Compute correct values
        if (sr != null && sr.sprite != null && cap != null)
        {
            float scale        = root.transform.localScale.y;          // 0.13
            float spriteChildY = sr.transform.localPosition.y;         // e.g. 1.0
            float naturalH     = sr.sprite.bounds.size.y;              // ≈ 14.02
            float capHalfH     = cap.size.y * 0.5f;                    // 0.5

            // sprite center in root local space: spriteChildY
            // sprite feet in root local space: spriteChildY - naturalH / 2
            // capsule bottom in root local space: capOffset.y - capHalfH
            // We want: capOffset.y - capHalfH == spriteChildY - naturalH / 2
            float neededOffset = spriteChildY - naturalH / 2f + capHalfH;
            float spriteFeetLocal = spriteChildY - naturalH / 2f;
            float gcLocal = spriteFeetLocal - 0.55f;  // same gap as original (0.55 local / 0.0715 world)

            Debug.Log($"[Jeb] scale={scale}  spriteChildLocalY={spriteChildY}  naturalH={naturalH}");
            Debug.Log($"[Jeb] → CapsuleCollider2D.offset.y should be: {neededOffset:F4} (current: {cap.offset.y:F4})");
            Debug.Log($"[Jeb] → GroundCheck.localY should be:          {gcLocal:F4}");
            Debug.Log($"[Jeb] → sprite feet local Y:                   {spriteFeetLocal:F4}");
        }
    }

    public static void Apply()
    {
        using var scope = new PrefabUtility.EditPrefabContentsScope(PrefabPath);
        var root = scope.prefabContentsRoot;
        if (root == null) { Debug.LogError("[Jeb] Prefab not found."); return; }

        var sr  = root.GetComponentInChildren<SpriteRenderer>(true);
        var cap = root.GetComponentInChildren<CapsuleCollider2D>(true);

        if (sr == null || sr.sprite == null || cap == null)
        {
            Debug.LogError("[Jeb] Missing SpriteRenderer or CapsuleCollider2D.");
            return;
        }

        float spriteChildY = sr.transform.localPosition.y;
        float naturalH     = sr.sprite.bounds.size.y;
        float capHalfH     = cap.size.y * 0.5f;

        float neededOffset = spriteChildY - naturalH / 2f + capHalfH;
        float spriteFeetLocal = spriteChildY - naturalH / 2f;
        float gcLocal = spriteFeetLocal - 0.55f;

        cap.offset = new Vector2(cap.offset.x, neededOffset);
        Debug.Log($"[Jeb] CapsuleCollider2D.offset.y = {neededOffset:F4}");

        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name.ToLower().Contains("ground"))
            {
                t.localPosition = new Vector3(t.localPosition.x, gcLocal, t.localPosition.z);
                Debug.Log($"[Jeb] GroundCheck.localY = {gcLocal:F4}");
            }
        }
        Debug.Log("[Jeb] Apply done.");
    }
}
