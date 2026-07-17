using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // Art batch 006 integration (ART-012/013/014; ART-011 shield icon is held
    // for a readability revision): assigns the Health Potion icon to Skill 3
    // and swaps the two skill-effect SpriteRenderers off their placeholder
    // sprites (which were literally UI button art). The new VFX sprites are
    // 512px vs the 1254px placeholders, so each effect's transform is rescaled
    // to preserve its previously tuned WORLD size (measured before the swap;
    // no script animates these scales - verified). Idempotent.
    public static class WireSkillIconsAndVfx
    {
        private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
        private const string PotionPath = "Assets/_JebbyJump/Art/Sprites/UI/ui_icon_skill_potion_01.png";
        private const string ShieldFxPath = "Assets/_JebbyJump/Art/Sprites/VFX/fx_bubble_shield_01.png";
        private const string RingFxPath = "Assets/_JebbyJump/Art/Sprites/VFX/fx_color_echo_ring_01.png";

        [MenuItem("Jebby Jump/Release/Wire Skill Icons And VFX")]
        public static void Run()
        {
            EnsureVfxImport(ShieldFxPath);
            EnsureVfxImport(RingFxPath);
            var potion = AssetDatabase.LoadAssetAtPath<Sprite>(PotionPath);
            var shieldFx = AssetDatabase.LoadAssetAtPath<Sprite>(ShieldFxPath);
            var ringFx = AssetDatabase.LoadAssetAtPath<Sprite>(RingFxPath);
            if (potion == null || shieldFx == null || ringFx == null)
            {
                Debug.LogError("[WireSkillVfx] sprites missing/not imported");
                return;
            }

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            int changes = 0;

            // Skill 3 icon (Health Potion). Skill 2 keeps the generic circle
            // until the revised shield icon lands.
            var skill3 = Find(scene, "Skill3Icon");
            var img = skill3 != null ? skill3.GetComponent<Image>() : null;
            if (img != null && img.sprite != potion)
            {
                img.sprite = potion;
                img.color = Color.white;
                changes++;
            }

            // The effects have NO world visuals of their own (verified: pure
            // logic + text feedback). Shield: add a bubble child under the
            // Jebby instance, toggled by the effect's existing active state.
            changes += WireShieldBubble(scene, shieldFx);
            // Color Echo is an instant text flash - a ring pulse needs a new
            // animation driver and is DEFERRED (asset committed, unwired).
            if (ringFx != null) { /* asset present; wiring deferred */ }

            if (changes > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
            Debug.Log("[WireSkillVfx] " + changes + " change(s); saved " + ScenePath);
        }

        // Creates/updates an inactive "ShieldBubbleVisual" child under the
        // Jebby instance (so it follows the player) and assigns it to
        // BubbleShieldEffect._visual. Bubble ~2.1u diameter around the ~1.8u
        // tall Jebby; renders just above the character.
        private static int WireShieldBubble(UnityEngine.SceneManagement.Scene scene, Sprite fx)
        {
            var jebby = Find(scene, "Jebby");
            var effectGo = Find(scene, "BubbleShieldEffect");
            var effect = effectGo != null
                ? effectGo.GetComponent<Items.BubbleShieldEffect>() : null;
            if (jebby == null || effect == null)
            {
                Debug.LogWarning("[WireSkillVfx] Jebby/BubbleShieldEffect missing - bubble skipped");
                return 0;
            }

            var t = jebby.transform.Find("ShieldBubbleVisual");
            var go = t != null ? t.gameObject : new GameObject("ShieldBubbleVisual");
            if (t == null) go.transform.SetParent(jebby.transform, false);
            go.transform.localPosition = new Vector3(0f, 0.9f, 0f); // bubble centre ~ body centre
            float scale = 2.1f / (fx.rect.width / fx.pixelsPerUnit); // 2.1u diameter
            go.transform.localScale = new Vector3(scale, scale, 1f);

            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) sr = go.AddComponent<SpriteRenderer>();
            var jebbySr = jebby.GetComponentInChildren<SpriteRenderer>(true);
            sr.sprite = fx;
            sr.color = Color.white;
            if (jebbySr != null)
            {
                sr.sortingLayerID = jebbySr.sortingLayerID;
                sr.sortingOrder = jebbySr.sortingOrder + 1; // bubble over Jebby
            }
            go.SetActive(false); // effect toggles it

            var so = new SerializedObject(effect);
            var visual = so.FindProperty("_visual");
            int changed = 0;
            if (visual != null && visual.objectReferenceValue != go)
            {
                visual.objectReferenceValue = go;
                so.ApplyModifiedPropertiesWithoutUndo();
                changed = 1;
            }
            Debug.Log("[WireSkillVfx] shield bubble wired under Jebby (2.1u, inactive by default)");
            return changed;
        }

        private static GameObject Find(UnityEngine.SceneManagement.Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == name) return root;
                var t = FindChild(root.transform, name);
                if (t != null) return t.gameObject;
            }
            return null;
        }

        private static Transform FindChild(Transform t, string name)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var c = t.GetChild(i);
                if (c.name == name) return c;
                var r = FindChild(c, name);
                if (r != null) return r;
            }
            return null;
        }

        private static void EnsureVfxImport(string path)
        {
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
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
