using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JebbyJump.EditorTools
{
    // Warm-palette pass (approved 2026-07-18): the Game scene used the old
    // prototype sky sprite (bg_sky_layer_01), a pale desaturated periwinkle
    // that read as "faint grey". Reuse the vibrant warm menu art instead.
    //
    // The menu art is landscape (24x10.8u) but the level is a vertical climb
    // (Cinemachine follows the player up ~10u in level 1), so a fixed world
    // sprite can't cover the whole column without distortion. Instead the
    // background is LOCKED to the camera (parented under Main Camera) and
    // scaled to over-fill the 16:9 viewport, so the warm sky always fills the
    // frame regardless of how far the player climbs. Idempotent.
    public static class WireGameBackground
    {
        private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
        private const string BgSpritePath =
            "Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_menu_01.png";

        // ortho 7 -> 14u tall; 1.5x of 10.8u = 16.2u tall, 36u wide -> covers
        // 16:9 (24.9u) through ~20:9 wide phones with margin.
        private static readonly Vector3 BgScale = new Vector3(1.5f, 1.5f, 1f);
        // Local z forward of the camera (which looks +z) so it sits behind the
        // z=0 gameplay plane; sorting order keeps it behind regardless.
        private static readonly Vector3 BgLocalPos = new Vector3(0f, 0f, 20f);

        [MenuItem("Jebby Jump/Release/Warm Game Background")]
        public static void Run()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GameObject bg = null, cam = null;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "Background") bg = root;
                if (root.name == "Main Camera") cam = root;
                if (bg == null) { var t = root.transform.Find("Background"); if (t) bg = t.gameObject; }
            }
            if (bg == null) { Debug.LogError("[WarmBg] Background not found"); return; }
            if (cam == null) { Debug.LogError("[WarmBg] Main Camera not found"); return; }

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(BgSpritePath);
            if (sprite == null) { Debug.LogError("[WarmBg] bg_menu_01 sprite not found"); return; }

            bool dirty = false;

            var sr = bg.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != sprite) { sr.sprite = sprite; dirty = true; }
            if (sr != null && sr.sortingOrder != -10) { sr.sortingOrder = -10; dirty = true; }
            if (sr != null && sr.color != Color.white) { sr.color = Color.white; dirty = true; }

            if (bg.transform.parent != cam.transform)
            {
                bg.transform.SetParent(cam.transform, worldPositionStays: false);
                dirty = true;
            }
            if ((bg.transform.localPosition - BgLocalPos).sqrMagnitude > 1e-4f)
            { bg.transform.localPosition = BgLocalPos; dirty = true; }
            if ((bg.transform.localScale - BgScale).sqrMagnitude > 1e-6f)
            { bg.transform.localScale = BgScale; dirty = true; }
            if (bg.transform.localRotation != Quaternion.identity)
            { bg.transform.localRotation = Quaternion.identity; dirty = true; }

            if (dirty)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log("[WarmBg] Game background -> bg_menu_01, camera-locked "
                    + BgScale.x + "x at local " + BgLocalPos);
            }
            else Debug.Log("[WarmBg] already up to date");
        }
    }
}
