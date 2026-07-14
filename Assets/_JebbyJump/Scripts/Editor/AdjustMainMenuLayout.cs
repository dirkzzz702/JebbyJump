using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JebbyJump.EditorTools
{
    // One-time, idempotent UI-only layout fix for the MainMenu overlaps found by the
    // live review (JJ-R1/JJ-R2): the title/button-stack collisions and the menu stack
    // rendering (and raycasting) ABOVE every opened modal panel. Sets ONLY RectTransform
    // anchoredPosition + one sibling reorder — no gameplay, script, prefab, or logic
    // change. Re-running is a no-op once the layout holds.
    public static class AdjustMainMenuLayout
    {
        private const string ScenePath = "Assets/_JebbyJump/Scenes/MainMenu.unity";

        [MenuItem("Jebby Jump/QA/Fix Main Menu Layout")]
        public static void Run()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            int changes = 0;

            // Title (y 200, h 130, spans 135..265) is overlapped by ContinueButton
            // (y 180, spans 135..225) -> lift the title clear of the stack.
            changes += SetY(scene, "TitleText", minOkY: 300f, targetY: 380f);

            // The stack steps 100 units (180/80/-20) but Wardrobe was inserted at -100
            // (spans -145..-55): 10u into Settings and 70/90 under Quit (-120).
            // Restore the 100-step: Wardrobe -120, Quit -220.
            changes += SetY(scene, "WardrobeButton", minOkY: -119f, targetY: -120f, mustBeAbove: true);
            changes += SetY(scene, "QuitButton", minOkY: -219f, targetY: -220f, mustBeAbove: true);

            // MenuSafeArea (the always-active button stack) is the LAST sibling, so it
            // renders + raycasts above every opened panel's full-screen backdrop.
            // Move it directly after TitleText so panels cover (and pointer-block) it.
            changes += ReorderStackBelowPanels(scene);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[FixMainMenuLayout] applied {changes} change(s); saved {ScenePath}");
        }

        // Sets anchoredPosition.y to targetY when the current value is on the wrong
        // side of minOkY. mustBeAbove=true means "current y > minOkY is wrong"
        // (element needs lowering); false means "current y < minOkY is wrong".
        private static int SetY(UnityEngine.SceneManagement.Scene scene, string name,
            float minOkY, float targetY, bool mustBeAbove = false)
        {
            var go = FindByName(scene, name);
            var rt = go != null ? go.transform as RectTransform : null;
            if (rt == null) return 0;
            var p = rt.anchoredPosition;
            bool wrong = mustBeAbove ? p.y > minOkY : p.y < minOkY;
            if (!wrong) return 0;
            rt.anchoredPosition = new Vector2(p.x, targetY);
            return 1;
        }

        private static int ReorderStackBelowPanels(UnityEngine.SceneManagement.Scene scene)
        {
            var stack = FindByName(scene, "MenuSafeArea");
            if (stack == null) return 0;
            var t = stack.transform;
            int idx = t.GetSiblingIndex();

            int minPanelIdx = int.MaxValue;
            foreach (var name in new[] { "LevelSelectPanel", "SettingsPanel", "WardrobePanel" })
            {
                var panel = FindByName(scene, name);
                if (panel != null && panel.transform.parent == t.parent)
                    minPanelIdx = Mathf.Min(minPanelIdx, panel.transform.GetSiblingIndex());
            }
            if (minPanelIdx == int.MaxValue || idx < minPanelIdx) return 0; // already below

            t.SetSiblingIndex(1); // directly after TitleText
            return 1;
        }

        private static GameObject FindByName(UnityEngine.SceneManagement.Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == name) return root;
                var f = FindInChildren(root.transform, name);
                if (f != null) return f.gameObject;
            }
            return null;
        }

        private static Transform FindInChildren(Transform t, string name)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var c = t.GetChild(i);
                if (c.name == name) return c;
                var r = FindInChildren(c, name);
                if (r != null) return r;
            }
            return null;
        }
    }
}
