using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JebbyJump.EditorTools
{
    // Art batch 003 integration (ART-005): replaces the Placeholder.png floor
    // VISUAL with the tiled cloud-meadow ledge, without touching gameplay.
    // Approach: the Floor GameObject keeps its transform (37.64 x 0.5 scale)
    // and BoxCollider2D exactly as-is; its own SpriteRenderer is disabled and
    // cleared, and a child "FloorVisual" (world scale 1) renders the ledge in
    // Tiled draw mode, top-aligned to the collider top so Jebby stands on the
    // grass lip. Idempotent - re-running updates the child in place.
    public static class WireFloorLedge
    {
        private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
        private const string SpritePath =
            "Assets/_JebbyJump/Art/Sprites/Platforms/spr_floor_strip_01.png";
        private const string VisualName = "FloorVisual";
        private const float LedgeWorldHeight = 1.28f; // 128px @ PPU 100

        // Visual grounding (FILE-MEASURED, 2026-07-17): the ledge art's drawn
        // surface line sits ~23px below the sprite's top edge (transparent
        // wavy lip above it), and Jebby's sprites carry 106px of transparent
        // canvas below the feet (x0.13 scale = 0.14u). Raising the visual by
        // both (plus a 0.02u tuck so feet sit IN the grass) aligns the drawn
        // grass with Jebby's visible feet. Collider stays untouched.
        private const float SurfaceInsetWorld = 0.23f; // art lip above surface
        private const float FeetTuckWorld = 0.16f;     // jebby feet padding + tuck

        [MenuItem("Jebby Jump/Release/Wire Floor Ledge")]
        public static void Run()
        {
            EnsureImport();
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
            if (sprite == null)
            {
                Debug.LogError("[WireFloorLedge] ledge sprite missing: " + SpritePath);
                return;
            }

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GameObject floor = null;
            foreach (var root in scene.GetRootGameObjects())
                if (root.name == "Floor") { floor = root; break; }
            if (floor == null)
            {
                Debug.LogError("[WireFloorLedge] Floor object not found in Game.unity");
                return;
            }

            var oldRenderer = floor.GetComponent<SpriteRenderer>();
            var collider = floor.GetComponent<BoxCollider2D>();
            if (oldRenderer == null || collider == null)
            {
                Debug.LogError("[WireFloorLedge] Floor is missing SpriteRenderer/BoxCollider2D");
                return;
            }

            // Measure BEFORE disabling: world width from the old visual (fallback
            // to collider bounds if the renderer was already cleared).
            float worldWidth = oldRenderer.sprite != null
                ? oldRenderer.bounds.size.x : collider.bounds.size.x;
            float topY = collider.bounds.max.y;
            float centerX = collider.bounds.center.x;

            // Retire the placeholder visual; collider + transform untouched.
            oldRenderer.sprite = null;
            oldRenderer.enabled = false;

            var child = floor.transform.Find(VisualName);
            var go = child != null ? child.gameObject : new GameObject(VisualName);
            if (child == null) go.transform.SetParent(floor.transform, false);

            // World scale (1,1) under the non-uniform parent scale.
            var ls = floor.transform.lossyScale;
            go.transform.localScale = new Vector3(
                ls.x != 0f ? 1f / ls.x : 1f, ls.y != 0f ? 1f / ls.y : 1f, 1f);
            go.transform.position = new Vector3(centerX,
                topY + SurfaceInsetWorld + FeetTuckWorld, floor.transform.position.z);

            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;                    // pivot top-centre (importer)
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = new Vector2(worldWidth, LedgeWorldHeight);
            sr.color = Color.white;                // art carries its own colour
            sr.sortingLayerID = oldRenderer.sortingLayerID;
            sr.sortingOrder = oldRenderer.sortingOrder;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[WireFloorLedge] ledge wired: width=" + worldWidth.ToString("F2")
                + "u, top=" + topY.ToString("F2") + ", height=" + LedgeWorldHeight
                + "u; placeholder renderer disabled (collider untouched).");
        }

        private static void EnsureImport()
        {
            var imp = AssetImporter.GetAtPath(SpritePath) as TextureImporter;
            if (imp == null) return;
            var settings = new TextureImporterSettings();
            imp.ReadTextureSettings(settings);
            bool changed = false;
            if (settings.textureType != TextureImporterType.Sprite)
            { settings.textureType = TextureImporterType.Sprite; changed = true; }
            if (settings.spriteMode != (int)SpriteImportMode.Single)
            { settings.spriteMode = (int)SpriteImportMode.Single; changed = true; }
            if (settings.spritePixelsPerUnit != 100f)
            { settings.spritePixelsPerUnit = 100f; changed = true; }
            if (settings.spriteAlignment != (int)SpriteAlignment.Custom
                || settings.spritePivot != new Vector2(0.5f, 1f))
            {
                settings.spriteAlignment = (int)SpriteAlignment.Custom;
                settings.spritePivot = new Vector2(0.5f, 1f); // top-centre: hangs from collider top
                changed = true;
            }
            if (settings.spriteMeshType != SpriteMeshType.FullRect) // required for Tiled
            { settings.spriteMeshType = SpriteMeshType.FullRect; changed = true; }
            if (settings.wrapMode != TextureWrapMode.Repeat)
            { settings.wrapMode = TextureWrapMode.Repeat; changed = true; }
            if (settings.alphaIsTransparency == false)
            { settings.alphaIsTransparency = true; changed = true; }
            if (changed)
            {
                imp.SetTextureSettings(settings);
                imp.SaveAndReimport();
            }
        }
    }
}
