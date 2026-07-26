using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // Re-skin the Level Complete + Game Over result cards with the GUI02 cream art
    // (9-slice cream card + cream/gold pill buttons + cocoa text), replacing the old
    // dark placeholder panels. The rank letter's colour is driven at runtime by
    // HUDController (re-tuned for the cream card). Idempotent.
    public static class BuildResultCards
    {
        private const string Dir = "Assets/_JebbyJump/Art/Sprites/UI/";
        private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
        private static readonly Color Cocoa = new Color(0.29f, 0.19f, 0.12f);

        [MenuItem("Jebby Jump/Scaffold/Build Result Cards")]
        public static void Run()
        {
            foreach (var s in new[] { "ui_result_card_9s", "ui_result_btn_9s", "ui_result_btn_primary_9s" })
                EnsureSprite(s + ".png");

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            int nb = 0;
            foreach (var panelName in new[] { "LevelCompletePanel", "GameOverPanel" })
            {
                var panel = FindDeep(scene, panelName);
                if (panel == null) { Debug.LogWarning("[ResultCards] missing " + panelName); continue; }
                var card = Find(panel.transform, "Card");
                if (card == null) { Debug.LogWarning("[ResultCards] no Card under " + panelName); continue; }

                // Card background -> cream 9-slice.
                var cimg = card.GetComponent<Image>();
                if (cimg != null) SkinImage(cimg, "ui_result_card_9s");

                // Cocoa all card texts (button labels handled below; the rank letter
                // is recoloured at runtime by HUDController).
                foreach (var tmp in card.GetComponentsInChildren<TMP_Text>(true))
                {
                    if (tmp.GetComponentInParent<Button>() != null) continue;
                    tmp.color = Cocoa; EditorUtility.SetDirty(tmp);
                }

                // Buttons -> cream / gold pills, cocoa bold labels, tint transition.
                foreach (var btn in card.GetComponentsInChildren<Button>(true))
                {
                    bool primary = btn.name.Contains("Next")
                        || (panelName == "GameOverPanel" && btn.name.Contains("Retry"));
                    var bimg = btn.image != null ? btn.image : btn.GetComponent<Image>();
                    if (bimg != null)
                    {
                        SkinImage(bimg, primary ? "ui_result_btn_primary_9s" : "ui_result_btn_9s");
                        btn.targetGraphic = bimg;
                        btn.transition = Selectable.Transition.ColorTint;
                        var cb = btn.colors;
                        cb.normalColor = Color.white; cb.highlightedColor = Color.white;
                        cb.pressedColor = new Color(0.88f, 0.88f, 0.88f, 1f);
                        cb.selectedColor = Color.white; cb.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.6f);
                        cb.colorMultiplier = 1f; cb.fadeDuration = 0.1f; btn.colors = cb;
                    }
                    var lbl = btn.GetComponentInChildren<TMP_Text>(true);
                    if (lbl != null) { lbl.color = Cocoa; lbl.fontStyle |= FontStyles.Bold; EditorUtility.SetDirty(lbl); }
                    EditorUtility.SetDirty(btn);
                    nb++;
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Canvas.ForceUpdateCanvases();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            Debug.Log($"[ResultCards] re-skinned Level-Complete + Game-Over cards + {nb} buttons.");
        }

        private static void SkinImage(Image img, string sprite)
        {
            var sp = Sprite(sprite);
            if (sp == null) return;
            img.sprite = sp; img.type = Image.Type.Sliced; img.fillCenter = true;
            img.color = Color.white; img.useSpriteMesh = false; img.SetAllDirty();
            EditorUtility.SetDirty(img);
        }

        private static Sprite Sprite(string n) => AssetDatabase.LoadAssetAtPath<Sprite>(Dir + n + ".png");

        private static void EnsureSprite(string file)
        {
            var imp = AssetImporter.GetAtPath(Dir + file) as TextureImporter;
            if (imp == null) return;
            if (imp.textureType != TextureImporterType.Sprite)
            { imp.textureType = TextureImporterType.Sprite; imp.SaveAndReimport(); }
            AssetDatabase.ImportAsset(Dir + file, ImportAssetOptions.ForceUpdate);
        }

        private static GameObject FindDeep(UnityEngine.SceneManagement.Scene s, string name)
        {
            foreach (var root in s.GetRootGameObjects())
            { if (root.name == name) return root; var t = Find(root.transform, name); if (t != null) return t.gameObject; }
            return null;
        }
        private static Transform Find(Transform t, string name)
        {
            for (int i = 0; i < t.childCount; i++)
            { var c = t.GetChild(i); if (c.name == name) return c; var r = Find(c, name); if (r != null) return r; }
            return null;
        }
    }
}
