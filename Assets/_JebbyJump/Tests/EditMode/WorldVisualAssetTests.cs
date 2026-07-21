using System;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JebbyJump.Tests.EditMode
{
    // P34C vertical-slice invariants: the world visual DATA is present and
    // genuinely distinct, and the applier is wired into the single Game scene.
    //
    // Both test assemblies set overrideReferences and cannot reference
    // Assembly-CSharp, so WorldThemeApplier/WorldDefinition are inspected via
    // AssetDatabase + SerializedObject + component type-name reflection rather
    // than direct types (same approach as LevelBalanceAssetTests).
    public class WorldVisualAssetTests
    {
        private const string GameScenePath =
            "Assets/_JebbyJump/Scenes/Game.unity";

        private static string[] WorldDefinitionPaths()
        {
            var guids = AssetDatabase.FindAssets("t:WorldDefinition");
            var paths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            Array.Sort(paths, StringComparer.Ordinal);
            return paths;
        }

        private static SerializedObject ByWorldNumber(int worldNumber)
        {
            foreach (var path in WorldDefinitionPaths())
            {
                var so = new SerializedObject(
                    AssetDatabase.LoadAssetAtPath<ScriptableObject>(path));
                if (so.FindProperty("_worldNumber").intValue == worldNumber)
                    return so;
            }
            return null;
        }

        [Test]
        public void EveryWorld_HasAVisualsBlock()
        {
            foreach (var path in WorldDefinitionPaths())
            {
                var so = new SerializedObject(
                    AssetDatabase.LoadAssetAtPath<ScriptableObject>(path));
                Assert.IsNotNull(so.FindProperty("_visuals"),
                    path + " missing _visuals");
                Assert.IsNotNull(so.FindProperty("_visuals._background"),
                    path + " missing _visuals._background");
            }
        }

        [Test]
        public void World1_HasBackgroundAndFloor()
        {
            var so = ByWorldNumber(1);
            Assert.IsNotNull(so, "World 1 definition not found");
            Assert.IsNotNull(so.FindProperty("_visuals._background").objectReferenceValue,
                "World 1 must ship a background");
            Assert.IsNotNull(so.FindProperty("_visuals._floor").objectReferenceValue,
                "World 1 must ship a floor");
        }

        // The whole point of the slice: two worlds that actually look different.
        [Test]
        public void World1_And_World2_HaveDifferentBackgrounds()
        {
            var w1 = ByWorldNumber(1);
            var w2 = ByWorldNumber(2);
            Assert.IsNotNull(w1); Assert.IsNotNull(w2);

            var bg1 = w1.FindProperty("_visuals._background").objectReferenceValue;
            var bg2 = w2.FindProperty("_visuals._background").objectReferenceValue;
            Assert.IsNotNull(bg1, "World 1 background unassigned");
            Assert.IsNotNull(bg2, "World 2 background unassigned");
            Assert.AreNotSame(bg1, bg2,
                "World switching is unprovable if both worlds share a background");
        }

        // Worlds 3-10 legitimately have no art yet; the runtime falls back to
        // World 1. This test documents that as intentional, not an oversight.
        [Test]
        public void WorldsWithoutArt_AreEmpty_NotBroken()
        {
            for (int n = 3; n <= 10; n++)
            {
                var so = ByWorldNumber(n);
                Assert.IsNotNull(so, "world " + n + " definition missing");
                // Property must exist (so the applier can read it) even when unassigned.
                Assert.IsNotNull(so.FindProperty("_visuals._background"),
                    "world " + n + " missing _visuals._background property");
            }
        }

        [Test]
        public void GameScene_HasWorldThemeApplier_WithReferencesBound()
        {
            var scene = EditorSceneManager.OpenScene(
                GameScenePath, OpenSceneMode.Additive);
            try
            {
                Component applier = FindComponentByTypeName(scene, "WorldThemeApplier");
                Assert.IsNotNull(applier,
                    "Game.unity has no WorldThemeApplier. Run "
                    + "'Jebby Jump/Release/Wire World Theme Applier'.");

                var so = new SerializedObject(applier);
                Assert.IsNotNull(so.FindProperty("_catalog").objectReferenceValue,
                    "WorldThemeApplier._catalog unassigned");
                Assert.IsNotNull(so.FindProperty("_levelSession").objectReferenceValue,
                    "WorldThemeApplier._levelSession unassigned");
                Assert.IsNotNull(so.FindProperty("_background").objectReferenceValue,
                    "WorldThemeApplier._background unassigned");
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        private static Component FindComponentByTypeName(Scene scene, string typeName)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var c in root.GetComponentsInChildren<Component>(true))
                {
                    if (c != null && c.GetType().Name == typeName) return c;
                }
            }
            return null;
        }
    }
}
