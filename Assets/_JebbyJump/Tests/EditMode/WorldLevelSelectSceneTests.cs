using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JebbyJump.Tests.EditMode
{
    // P34D scene integrity: the Level Select panel has a 10-tab world strip,
    // and the controller's world fields are wired. Inspected via component
    // type-name reflection + SerializedObject (this assembly cannot reference
    // Assembly-CSharp), matching WorldVisualAssetTests. The world-scoping
    // arithmetic itself is covered by WorldMappingTests.
    public class WorldLevelSelectSceneTests
    {
        private const string MainMenuScenePath =
            "Assets/_JebbyJump/Scenes/MainMenu.unity";

        private static Scene Open()
            => EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Additive);

        private static Component FindByTypeName(Scene scene, string typeName)
        {
            foreach (var root in scene.GetRootGameObjects())
                foreach (var c in root.GetComponentsInChildren<Component>(true))
                    if (c != null && c.GetType().Name == typeName) return c;
            return null;
        }

        private static List<Component> AllByTypeName(Scene scene, string typeName)
        {
            var list = new List<Component>();
            foreach (var root in scene.GetRootGameObjects())
                foreach (var c in root.GetComponentsInChildren<Component>(true))
                    if (c != null && c.GetType().Name == typeName) list.Add(c);
            return list;
        }

        [Test]
        public void LevelSelect_HasTenWorldTabs()
        {
            var scene = Open();
            try
            {
                var tabs = AllByTypeName(scene, "WorldTab");
                Assert.AreEqual(10, tabs.Count,
                    "Level Select must have exactly 10 world tabs. Run "
                    + "'Jebby Jump/Scaffold/Build World Level Select'.");
            }
            finally { EditorSceneManager.CloseScene(scene, true); }
        }

        [Test]
        public void EveryWorldTab_HasButtonAndLabelBound()
        {
            var scene = Open();
            try
            {
                foreach (var tab in AllByTypeName(scene, "WorldTab"))
                {
                    var so = new UnityEditor.SerializedObject(tab);
                    Assert.IsNotNull(so.FindProperty("_button").objectReferenceValue,
                        tab.name + " has no _button");
                    Assert.IsNotNull(so.FindProperty("_label").objectReferenceValue,
                        tab.name + " has no _label");
                    Assert.IsNotNull(so.FindProperty("_selectedIndicator").objectReferenceValue,
                        tab.name + " has no _selectedIndicator");
                }
            }
            finally { EditorSceneManager.CloseScene(scene, true); }
        }

        [Test]
        public void Controller_HasWorldFieldsWired()
        {
            var scene = Open();
            try
            {
                var controller = FindByTypeName(scene, "LevelSelectController");
                Assert.IsNotNull(controller, "LevelSelectController missing from MainMenu");

                var so = new UnityEditor.SerializedObject(controller);
                Assert.IsNotNull(so.FindProperty("_worldCatalog").objectReferenceValue,
                    "_worldCatalog unwired");
                Assert.IsNotNull(so.FindProperty("_worldTabContainer").objectReferenceValue,
                    "_worldTabContainer unwired");
                Assert.IsNotNull(so.FindProperty("_worldTitle").objectReferenceValue,
                    "_worldTitle unwired");
                // Existing wiring must survive the P34D changes.
                Assert.IsNotNull(so.FindProperty("_catalog").objectReferenceValue,
                    "_catalog unwired");
                Assert.IsNotNull(so.FindProperty("_cardPrefab").objectReferenceValue,
                    "_cardPrefab unwired");
            }
            finally { EditorSceneManager.CloseScene(scene, true); }
        }

        [Test]
        public void WorldTabs_AreChildrenOfTheWiredContainer()
        {
            var scene = Open();
            try
            {
                var controller = FindByTypeName(scene, "LevelSelectController");
                var so = new UnityEditor.SerializedObject(controller);
                var container = so.FindProperty("_worldTabContainer").objectReferenceValue
                    as Transform;
                Assert.IsNotNull(container, "_worldTabContainer is not a Transform");

                int childTabs = 0;
                for (int i = 0; i < container.childCount; i++)
                    if (container.GetChild(i).GetComponent("WorldTab") != null) childTabs++;
                Assert.AreEqual(10, childTabs,
                    "the wired container must hold the 10 world tabs");
            }
            finally { EditorSceneManager.CloseScene(scene, true); }
        }
    }
}
