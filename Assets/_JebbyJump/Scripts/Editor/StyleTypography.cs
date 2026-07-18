using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // Typography + Level Select polish pass (approved 2026-07-18): one style
    // system across both scenes so no plain default text remains.
    //   Display  = bold + cream->gold vertex gradient (titles, level number)
    //   Label    = bold cream with slight letter spacing (all button labels)
    //   Soft     = warm off-white (body/secondary text)
    // Also restyles LevelSelectCard.prefab (kit chrome, rounded locked
    // overlay, styled number/stats) and gives the Level Select title its own
    // band so cards no longer scroll behind it. Colours set at runtime by
    // code (rank colour, card state tints) are left untouched. Idempotent.
    public static class StyleTypography
    {
        private const string PillPath = "Assets/_JebbyJump/Art/Sprites/UI/ui_btn_pill_9s.png";
        private const string PanelPath = "Assets/_JebbyJump/Art/Sprites/UI/ui_panel_soft_9s.png";
        private const string CardPrefabPath = "Assets/_JebbyJump/Prefabs/UI/LevelSelectCard.prefab";

        private static readonly Color Cream = new Color(1f, 0.96f, 0.88f);
        private static readonly Color Gold = new Color(0.94f, 0.78f, 0.41f);
        private static readonly Color SoftBody = new Color(0.95f, 0.93f, 0.88f);
        private static readonly Color LabelCream = new Color(0.97f, 0.95f, 0.91f);

        [MenuItem("Jebby Jump/QA/Style Typography")]
        public static void Run()
        {
            var pill = AssetDatabase.LoadAssetAtPath<Sprite>(PillPath);
            var panel = AssetDatabase.LoadAssetAtPath<Sprite>(PanelPath);
            int total = 0;

            total += StyleCardPrefab(panel);

            // ---- Game.unity ----
            var game = EditorSceneManager.OpenScene(
                "Assets/_JebbyJump/Scenes/Game.unity", OpenSceneMode.Single);
            int n = 0;
            n += Display(game, "PausePanel", "Title", 46f);
            n += Display(game, "SettingsPanel", "Title", 40f);
            n += Display(game, "LevelCompletePanel", "TitleText", 50f);
            n += DisplayWarm(game, "GameOverPanel", "TitleText", 46f);
            n += Display(game, "LevelBadgeRoot", "LevelText", 30f);
            n += SwapImage(game, "LevelBadgeRoot", pill);
            n += SwapImage(game, "TutorialHintRoot", pill);
            n += Style(game, "TutorialHintRoot", "TutorialHintText", 30f, true, Cream);
            n += Style(game, "FeedbackRoot", "FeedbackText", 40f, true, Cream);
            n += Style(game, "HUDCanvas", "LiveTimerText", 0f, true, LabelCream);
            n += SoftLabels(game, "SettingsPanel");
            n += ButtonLabels(game);
            if (n > 0) EditorSceneManager.SaveScene(game);
            total += n;
            Debug.Log("[StyleTypography] Game.unity: " + n + " change group(s)");

            // ---- MainMenu.unity ----
            var menu = EditorSceneManager.OpenScene(
                "Assets/_JebbyJump/Scenes/MainMenu.unity", OpenSceneMode.Single);
            n = 0;
            n += Display(menu, "LevelSelectPanel", "Title", 44f);
            n += Display(menu, "SettingsPanel", "Title", 44f);
            n += Display(menu, "WardrobePanel", "Title", 44f);
            n += Display(menu, "UnlockCeremonyOverlay", "Title", 40f);
            n += Style(menu, "UnlockCeremonyOverlay", "OutfitName", 34f, true, Gold);
            n += Style(menu, "UnlockCeremonyOverlay", "Message", 0f, false, SoftBody);
            n += Style(menu, "WardrobePanel", "PreviewLabel", 30f, true, Cream);
            n += Style(menu, "WardrobePanel", "StateLabel", 0f, true, Gold);
            n += SoftLabels(menu, "SettingsPanel");
            n += TitleBand(menu);
            n += ButtonLabels(menu);
            if (n > 0) EditorSceneManager.SaveScene(menu);
            total += n;
            Debug.Log("[StyleTypography] MainMenu.unity: " + n + " change group(s)");

            Debug.Log("[StyleTypography] done; " + total + " total change group(s)");
        }

        // ---- treatments -----------------------------------------------------

        private static int ApplyDisplay(TMP_Text tmp, float size, Color top, Color bottom)
        {
            if (tmp == null) return 0;
            bool dirty = false;
            if (size > 0f && !Mathf.Approximately(tmp.fontSize, size))
            { tmp.fontSize = size; dirty = true; }
            if ((tmp.fontStyle & FontStyles.Bold) == 0)
            { tmp.fontStyle |= FontStyles.Bold; dirty = true; }
            if (tmp.color != Color.white) { tmp.color = Color.white; dirty = true; }
            var g = new VertexGradient(top, top, bottom, bottom);
            if (!tmp.enableVertexGradient
                || !tmp.colorGradient.topLeft.Equals(g.topLeft)
                || !tmp.colorGradient.bottomLeft.Equals(g.bottomLeft))
            {
                tmp.enableVertexGradient = true;
                tmp.colorGradient = g;
                dirty = true;
            }
            return dirty ? 1 : 0;
        }

        private static int Display(UnityEngine.SceneManagement.Scene s,
            string parent, string child, float size)
            => ApplyDisplay(FindText(s, parent, child), size, Cream, Gold);

        // Warm red-amber for Game Over (distinct outcome colour).
        private static int DisplayWarm(UnityEngine.SceneManagement.Scene s,
            string parent, string child, float size)
            => ApplyDisplay(FindText(s, parent, child), size,
                new Color(1f, 0.76f, 0.66f), new Color(0.9f, 0.42f, 0.34f));

        private static int Style(UnityEngine.SceneManagement.Scene s,
            string parent, string child, float size, bool bold, Color colour)
        {
            var tmp = FindText(s, parent, child);
            if (tmp == null) return 0;
            bool dirty = false;
            if (size > 0f && !Mathf.Approximately(tmp.fontSize, size))
            { tmp.fontSize = size; dirty = true; }
            if (bold && (tmp.fontStyle & FontStyles.Bold) == 0)
            { tmp.fontStyle |= FontStyles.Bold; dirty = true; }
            if (tmp.color != colour) { tmp.color = colour; dirty = true; }
            return dirty ? 1 : 0;
        }

        // Every Button's TMP label: bold cream with slight letter spacing.
        private static int ButtonLabels(UnityEngine.SceneManagement.Scene s)
        {
            int changed = 0;
            foreach (var root in s.GetRootGameObjects())
                foreach (var btn in root.GetComponentsInChildren<Button>(true))
                {
                    var tmp = btn.GetComponentInChildren<TMP_Text>(true);
                    if (tmp == null) continue;
                    bool dirty = false;
                    if ((tmp.fontStyle & FontStyles.Bold) == 0)
                    { tmp.fontStyle |= FontStyles.Bold; dirty = true; }
                    if (!tmp.enableVertexGradient && tmp.color != LabelCream
                        && tmp.color == Color.white)   // keep code-tinted labels
                    { tmp.color = LabelCream; dirty = true; }
                    if (tmp.characterSpacing < 2f)
                    { tmp.characterSpacing = 2f; dirty = true; }
                    if (dirty) changed++;
                }
            return changed > 0 ? 1 : 0;
        }

        // Settings row labels (Music/SFX/Mute/...): warm off-white.
        private static int SoftLabels(UnityEngine.SceneManagement.Scene s, string panelName)
        {
            var panel = Find(s, panelName);
            if (panel == null) return 0;
            int changed = 0;
            foreach (var tmp in panel.GetComponentsInChildren<TMP_Text>(true))
            {
                if (tmp.name == "Title") continue;
                if (tmp.GetComponentInParent<Button>(true) != null) continue;
                if (tmp.color == Color.white)
                { tmp.color = SoftBody; changed++; }
            }
            return changed > 0 ? 1 : 0;
        }

        private static int SwapImage(UnityEngine.SceneManagement.Scene s,
            string name, Sprite sprite)
        {
            var go = Find(s, name);
            var img = go != null ? go.GetComponent<Image>() : null;
            if (img == null || sprite == null || img.sprite == sprite) return 0;
            img.sprite = sprite;
            img.type = Image.Type.Sliced;
            img.color = Color.white;
            return 1;
        }

        // Level Select: give the title its own band - cards were scrolling
        // behind it (RUNTIME-OBSERVED in the live review). Insets the
        // ScrollView's top edge below the title row.
        private static int TitleBand(UnityEngine.SceneManagement.Scene s)
        {
            var panel = Find(s, "LevelSelectPanel");
            var scroll = panel != null ? FindChild(panel.transform, "ScrollView") : null;
            var rt = scroll != null ? scroll as RectTransform : null;
            if (rt == null) return 0;
            if (Mathf.Approximately(rt.offsetMax.y, -110f)) return 0;
            rt.offsetMax = new Vector2(rt.offsetMax.x, -110f);
            return 1;
        }

        // ---- LevelSelectCard prefab ----------------------------------------

        private static int StyleCardPrefab(Sprite panel)
        {
            var root = PrefabUtility.LoadPrefabContents(CardPrefabPath);
            try
            {
                bool dirty = false;
                // Card chrome: kit panel (code state tints multiply over it).
                var bg = root.GetComponent<Image>();
                if (bg != null && panel != null && bg.sprite != panel)
                {
                    bg.sprite = panel;
                    bg.type = Image.Type.Sliced;
                    dirty = true;
                }
                // Locked overlay follows the rounded silhouette.
                var overlay = FindChild(root.transform, "LockedOverlay");
                var oImg = overlay != null ? overlay.GetComponent<Image>() : null;
                if (oImg != null && panel != null && oImg.sprite != panel)
                {
                    oImg.sprite = panel;
                    oImg.type = Image.Type.Sliced;
                    oImg.color = new Color(0f, 0f, 0f, 0.6f);
                    dirty = true;
                }
                // Number: display gradient. Stats: soft cream; stars gold.
                dirty |= StyleChild(root, "LevelNumber", 64f, true, null, gradient: true);
                dirty |= StyleChild(root, "BestTime", 22f, false, SoftBody);
                dirty |= StyleChild(root, "BestRank", 22f, true, SoftBody);
                dirty |= StyleChild(root, "StarsText", 22f, true, new Color(1f, 0.87f, 0.45f));
                dirty |= StyleChild(root, "LockedText", 26f, true, new Color(1f, 1f, 1f, 0.92f), spacing: 4f);
                if (dirty)
                {
                    PrefabUtility.SaveAsPrefabAsset(root, CardPrefabPath);
                    Debug.Log("[StyleTypography] LevelSelectCard.prefab restyled");
                }
                return dirty ? 1 : 0;
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }

        private static bool StyleChild(GameObject root, string name, float size,
            bool bold, Color? colour, bool gradient = false, float spacing = 0f)
        {
            var t = FindChild(root.transform, name);
            var tmp = t != null ? t.GetComponent<TMP_Text>() : null;
            if (tmp == null) return false;
            bool dirty = false;
            if (size > 0f && !Mathf.Approximately(tmp.fontSize, size))
            { tmp.fontSize = size; dirty = true; }
            if (bold && (tmp.fontStyle & FontStyles.Bold) == 0)
            { tmp.fontStyle |= FontStyles.Bold; dirty = true; }
            if (colour.HasValue && tmp.color != colour.Value)
            { tmp.color = colour.Value; dirty = true; }
            if (gradient && !tmp.enableVertexGradient)
            {
                tmp.color = Color.white;
                tmp.enableVertexGradient = true;
                tmp.colorGradient = new VertexGradient(Cream, Cream, Gold, Gold);
                dirty = true;
            }
            if (spacing > 0f && tmp.characterSpacing < spacing)
            { tmp.characterSpacing = spacing; dirty = true; }
            return dirty;
        }

        // ---- lookup ---------------------------------------------------------

        private static TMP_Text FindText(UnityEngine.SceneManagement.Scene s,
            string parent, string child)
        {
            var p = Find(s, parent);
            var t = p != null ? FindChild(p.transform, child) : null;
            return t != null ? t.GetComponent<TMP_Text>() : null;
        }

        private static GameObject Find(UnityEngine.SceneManagement.Scene s, string name)
        {
            foreach (var root in s.GetRootGameObjects())
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
    }
}
