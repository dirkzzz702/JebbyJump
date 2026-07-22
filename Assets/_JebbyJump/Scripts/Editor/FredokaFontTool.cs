using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace JebbyJump.EditorTools
{
    // P35A — replace the default LiberationSans (Arial-clone) text with the
    // rounded, friendly Fredoka across the whole game. Two steps:
    //   1) Create Fredoka Font Asset — bakes a TMP SDF font asset from the TTF,
    //      with LiberationSans kept as a glyph fallback (♥ ★ → etc.).
    //   2) Apply Fredoka Font — points TMP's default at it and sweeps every
    //      TMP_Text in both scenes + the UI prefabs onto it (sizes/colours/
    //      styles untouched). Idempotent.
    public static class FredokaFontTool
    {
        private const string TtfPath  = "Assets/_JebbyJump/Art/Fonts/Fredoka.ttf";
        private const string SdfPath  = "Assets/_JebbyJump/Art/Fonts/Fredoka SDF.asset";
        private const string FallbackPath =
            "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";
        private static readonly string[] Scenes =
        {
            "Assets/_JebbyJump/Scenes/MainMenu.unity",
            "Assets/_JebbyJump/Scenes/Game.unity",
        };
        private const string PrefabRoot = "Assets/_JebbyJump/Prefabs";

        [MenuItem("Jebby Jump/QA/Fredoka - 1 Create Font Asset")]
        public static void Create()
        {
            var font = AssetDatabase.LoadAssetAtPath<Font>(TtfPath);
            if (font == null) { Debug.LogError("[Fredoka] TTF not imported: " + TtfPath); return; }

            var fa = TMP_FontAsset.CreateFontAsset(
                font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024,
                AtlasPopulationMode.Dynamic, enableMultiAtlasSupport: true);
            if (fa == null) { Debug.LogError("[Fredoka] CreateFontAsset returned null"); return; }
            fa.name = "Fredoka SDF";

            // Pre-bake the common character set so first render isn't janky.
            fa.TryAddCharacters(
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
                + " .,:;!?'\"-_/+()[]%&#@*=<>");

            AssetDatabase.DeleteAsset(SdfPath);
            AssetDatabase.CreateAsset(fa, SdfPath);
            if (fa.atlasTextures != null && fa.atlasTextures.Length > 0)
            {
                fa.atlasTextures[0].name = "Fredoka SDF Atlas";
                AssetDatabase.AddObjectToAsset(fa.atlasTextures[0], fa);
            }
            if (fa.material != null)
            {
                fa.material.name = "Fredoka SDF Material";
                AssetDatabase.AddObjectToAsset(fa.material, fa);
            }

            // Keep LiberationSans as a fallback for glyphs Fredoka lacks (♥ ★ →).
            var fallback = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FallbackPath);
            if (fallback != null)
            {
                fa.fallbackFontAssetTable ??= new List<TMP_FontAsset>();
                if (!fa.fallbackFontAssetTable.Contains(fallback))
                    fa.fallbackFontAssetTable.Add(fallback);
            }

            EditorUtility.SetDirty(fa);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Fredoka] Created " + SdfPath + " (dynamic SDF, LiberationSans fallback).");
        }

        [MenuItem("Jebby Jump/QA/Fredoka - 2 Apply Everywhere")]
        public static void Apply()
        {
            var fredoka = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(SdfPath);
            if (fredoka == null) { Debug.LogError("[Fredoka] SDF asset missing — run step 1 first."); return; }

            // Make Fredoka the TMP default so any unreferenced text uses it too.
            var settings = TMP_Settings.instance;
            if (settings != null)
            {
                var so = new SerializedObject(settings);
                var p = so.FindProperty("m_defaultFontAsset");
                if (p != null && p.objectReferenceValue != fredoka)
                { p.objectReferenceValue = fredoka; so.ApplyModifiedPropertiesWithoutUndo(); }
            }

            int texts = 0;
            foreach (var scenePath in Scenes)
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                int n = 0;
                foreach (var root in scene.GetRootGameObjects())
                    foreach (var tmp in root.GetComponentsInChildren<TMP_Text>(true))
                        if (tmp.font != fredoka) { tmp.font = fredoka; EditorUtility.SetDirty(tmp); n++; }
                if (n > 0) { EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); }
                texts += n;
                Debug.Log("[Fredoka] " + System.IO.Path.GetFileName(scenePath) + ": " + n + " text(s) -> Fredoka");
            }

            int prefabs = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { PrefabRoot }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var root = PrefabUtility.LoadPrefabContents(path);
                try
                {
                    bool dirty = false;
                    foreach (var tmp in root.GetComponentsInChildren<TMP_Text>(true))
                        if (tmp.font != fredoka) { tmp.font = fredoka; dirty = true; }
                    if (dirty) { PrefabUtility.SaveAsPrefabAsset(root, path); prefabs++; }
                }
                finally { PrefabUtility.UnloadPrefabContents(root); }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[Fredoka] Applied to " + texts + " scene text(s) and " + prefabs + " prefab(s). Default font set.");
        }
    }
}
