using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // P35C — roll the UI01 button chrome onto every other screen (pause, result,
    // settings, level-select, wardrobe). Swaps each old dark pill button to the
    // light kit frame (honey primary for the main action per panel, cream
    // secondary otherwise), flips its cream label to dark cocoa for legibility,
    // resets the ColorBlock to a white base (the frame art carries the colour),
    // and drops any leftover interim shadow. Runtime-tinted labels (gold/coloured)
    // are left untouched.
    //
    // Panel/result-CARD backgrounds are intentionally NOT reskinned here: they
    // hold runtime-coloured content and need a visual pass. Buttons are self-
    // contained, so this is legibility-safe. Only sprites/colours change - no
    // RectTransforms - so the overlap regression is unaffected. Idempotent.
    public static class RollUiKitButtons
    {
        private const string Dir = "Assets/_JebbyJump/Art/Sprites/UI/";
        private static readonly string[] Scenes =
        {
            "Assets/_JebbyJump/Scenes/MainMenu.unity",
            "Assets/_JebbyJump/Scenes/Game.unity",
        };
        private static readonly Color Cocoa = new Color(0.29f, 0.19f, 0.12f);

        // Labels that read as the main positive action -> honey primary frame.
        private static readonly HashSet<string> PrimaryLabels = new HashSet<string>
        {
            "resume", "next level", "next", "play", "continue", "equip", "equip now", "start",
        };

        [MenuItem("Jebby Jump/QA/Roll UI Kit To Buttons")]
        public static void Run()
        {
            var pill = AssetDatabase.LoadAssetAtPath<Sprite>(Dir + "ui_btn_pill_9s.png");
            var primary = AssetDatabase.LoadAssetAtPath<Sprite>(Dir + "ui_btn_primary_9s.png");
            var secondary = AssetDatabase.LoadAssetAtPath<Sprite>(Dir + "ui_btn_secondary_9s.png");
            if (pill == null || primary == null || secondary == null)
            { Debug.LogError("[RollKit] sprites missing"); return; }

            int total = 0;
            foreach (var scenePath in Scenes)
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                int n = 0;
                foreach (var root in scene.GetRootGameObjects())
                    foreach (var btn in root.GetComponentsInChildren<Button>(true))
                    {
                        var img = btn.image != null ? btn.image : btn.GetComponent<Image>();
                        if (img == null || img.sprite != pill) continue; // only old-pill buttons

                        var label = btn.GetComponentInChildren<TMP_Text>(true);
                        string txt = label != null ? label.text.Trim().ToLowerInvariant() : "";
                        img.sprite = PrimaryLabels.Contains(txt) ? primary : secondary;
                        img.type = Image.Type.Sliced;
                        img.color = Color.white;

                        btn.transition = Selectable.Transition.ColorTint;
                        var cb = btn.colors;
                        cb.normalColor = Color.white;
                        cb.highlightedColor = Color.white;
                        cb.pressedColor = new Color(0.86f, 0.86f, 0.86f, 1f);
                        cb.selectedColor = Color.white;
                        cb.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.6f);
                        cb.colorMultiplier = 1f; cb.fadeDuration = 0.1f;
                        btn.colors = cb;

                        var shadow = btn.GetComponent<Shadow>();
                        if (shadow != null) Object.DestroyImmediate(shadow);

                        // Cream/white label -> cocoa; leave runtime-tinted labels alone.
                        if (label != null && !label.enableVertexGradient)
                        {
                            var c = label.color;
                            if (c.r > 0.8f && c.g > 0.8f && c.b > 0.75f)
                            { label.color = Cocoa; EditorUtility.SetDirty(label); }
                        }
                        EditorUtility.SetDirty(btn);
                        n++;
                    }
                if (n > 0) { EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); }
                total += n;
                Debug.Log("[RollKit] " + System.IO.Path.GetFileName(scenePath) + ": " + n + " button(s) reskinned");
            }
            Debug.Log("[RollKit] done; " + total + " button(s) on the kit.");
        }
    }
}
