using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // Art batch 005 integration (ART-007/008): swaps the shell chrome from
    // Unity's built-in UISprite to the generated 9-slice kit, in both scenes.
    // v1 scope (approved): every Button whose target graphic still uses the
    // built-in sprite gets the pill (image colour reset to white - the art
    // carries the body colour; Selectable tint states keep working from a
    // white base), and Images named "Card" (LevelComplete/GameOver result
    // cards) get the panel sprite. Full-screen dim backdrops are intentionally
    // untouched (they stay procedural dims), as are the bespoke tint-driven
    // LevelSelect cards and wardrobe rows (follow-up pass). RectTransforms are
    // never modified. Idempotent.
    public static class WireShellUiKit
    {
        private const string PanelPath = "Assets/_JebbyJump/Art/Sprites/UI/ui_panel_soft_9s.png";
        private const string PillPath = "Assets/_JebbyJump/Art/Sprites/UI/ui_btn_pill_9s.png";
        private static readonly string[] Scenes =
        {
            "Assets/_JebbyJump/Scenes/MainMenu.unity",
            "Assets/_JebbyJump/Scenes/Game.unity",
        };

        [MenuItem("Jebby Jump/Release/Wire Shell UI Kit")]
        public static void Run()
        {
            // Vector4 border order: X=left, Y=bottom, Z=right, W=top.
            EnsureImport(PanelPath, new Vector4(64, 64, 64, 64));
            EnsureImport(PillPath, new Vector4(48, 40, 48, 40));
            var panel = AssetDatabase.LoadAssetAtPath<Sprite>(PanelPath);
            var pill = AssetDatabase.LoadAssetAtPath<Sprite>(PillPath);
            if (panel == null || pill == null)
            {
                Debug.LogError("[WireShellUiKit] kit sprites missing/not imported");
                return;
            }

            foreach (var scenePath in Scenes)
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                int buttons = 0, cards = 0;
                foreach (var root in scene.GetRootGameObjects())
                {
                    foreach (var btn in root.GetComponentsInChildren<Button>(true))
                    {
                        var img = btn.targetGraphic as Image;
                        if (img == null) continue;
                        if (img.sprite == pill) continue;               // already kit
                        if (!IsBuiltinOrNull(img.sprite)) continue;     // custom art (orbs etc.)
                        img.sprite = pill;
                        img.type = Image.Type.Sliced;
                        img.color = Color.white;
                        buttons++;
                    }
                    foreach (var t in root.GetComponentsInChildren<Transform>(true))
                    {
                        if (t.name != "Card") continue;
                        var img = t.GetComponent<Image>();
                        if (img == null || img.sprite == panel) continue;
                        if (!IsBuiltinOrNull(img.sprite)) continue;
                        img.sprite = panel;
                        img.type = Image.Type.Sliced;
                        img.color = Color.white;
                        cards++;
                    }
                }
                if (buttons > 0 || cards > 0)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                }
                Debug.Log("[WireShellUiKit] " + System.IO.Path.GetFileName(scenePath)
                    + ": " + buttons + " button(s) -> pill, " + cards + " card(s) -> panel");
            }
        }

        // True for the built-in UISprite/Background sprites and for None -
        // the placeholder chrome this kit replaces.
        private static bool IsBuiltinOrNull(Sprite s)
        {
            if (s == null) return true;
            string path = AssetDatabase.GetAssetPath(s);
            return string.IsNullOrEmpty(path) || path.Contains("unity_builtin");
        }

        private static void EnsureImport(string path, Vector4 border)
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
            if (settings.spriteMeshType != SpriteMeshType.FullRect)
            { settings.spriteMeshType = SpriteMeshType.FullRect; changed = true; }
            if (settings.spriteBorder != border)
            { settings.spriteBorder = border; changed = true; }
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
