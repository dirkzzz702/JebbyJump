using UnityEditor;
using UnityEngine;

namespace JebbyJump.EditorTools
{
    // Playtest fix (verified 2026-07-18): the cactus rendered small and
    // floating. Measured causes: the sprite's ink occupies only 63% of its
    // 1254px canvas with 257px of transparent padding BELOW the ink, while
    // the importer used the Center pivot preset - so with the spawner's
    // (self-consistent) collider math the canvas bottom sat on the platform
    // but the VISIBLE cactus hovered 0.246u above it, at just 0.57x0.76u.
    //
    // Fix: pivot the sprite at its ink base (custom 0.4948, 0.2049), grow the
    // prefab to (1.1, 1.5) -> visible ~0.79x0.95u, and fit the trigger box to
    // the ink with ~12% forgiveness. PlatformSpawner now places the pivot
    // (= visible base) directly on the platform top. Idempotent.
    public static class FixCactusGrounding
    {
        private const string SpritePath =
            "Assets/_JebbyJump/Art/Sprites/Obstacles/spr_cactus_obstacle_01.png";
        private const string PrefabPath =
            "Assets/_JebbyJump/Prefabs/Obstacles/Cactus.prefab";

        // Ink bbox (171,207)-(1070,997) on the 1254 canvas (FILE-MEASURED).
        private static readonly Vector2 InkPivot = new Vector2(0.4948f, 0.2049f);
        private static readonly Vector3 TargetScale = new Vector3(1.1f, 1.5f, 1f);
        // Ink in sprite units (PPU 1254): w 0.717, h 0.630; pivot at ink base.
        private static readonly Vector2 ColliderSize = new Vector2(0.61f, 0.57f);
        private static readonly Vector2 ColliderOffset = new Vector2(0f, 0.315f);

        [MenuItem("Jebby Jump/Release/Fix Cactus Grounding")]
        public static void Run()
        {
            // 1) importer: custom ink-base pivot.
            var imp = AssetImporter.GetAtPath(SpritePath) as TextureImporter;
            if (imp != null)
            {
                var s = new TextureImporterSettings();
                imp.ReadTextureSettings(s);
                if (s.spriteAlignment != (int)SpriteAlignment.Custom
                    || (s.spritePivot - InkPivot).sqrMagnitude > 1e-6f)
                {
                    s.spriteAlignment = (int)SpriteAlignment.Custom;
                    s.spritePivot = InkPivot;
                    imp.SetTextureSettings(s);
                    imp.SaveAndReimport();
                    Debug.Log("[FixCactus] importer pivot -> ink base " + InkPivot);
                }
            }

            // 2) prefab: scale + ink-hugging trigger box.
            var root = PrefabUtility.LoadPrefabContents(PrefabPath);
            try
            {
                bool dirty = false;
                if ((root.transform.localScale - TargetScale).sqrMagnitude > 1e-6f)
                {
                    root.transform.localScale = TargetScale;
                    dirty = true;
                }
                var box = root.GetComponent<BoxCollider2D>();
                if (box != null
                    && ((box.size - ColliderSize).sqrMagnitude > 1e-6f
                        || (box.offset - ColliderOffset).sqrMagnitude > 1e-6f))
                {
                    box.size = ColliderSize;
                    box.offset = ColliderOffset;
                    dirty = true;
                }
                if (dirty)
                {
                    PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
                    Debug.Log("[FixCactus] prefab scale " + TargetScale
                        + ", trigger " + ColliderSize + " @ " + ColliderOffset
                        + " (world ~0.67x0.86 inside the 0.79x0.95 visible ink)");
                }
                else Debug.Log("[FixCactus] prefab already up to date");
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }
    }
}
