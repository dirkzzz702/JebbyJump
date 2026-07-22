using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JebbyJump.Tests.EditMode
{
    // P34F scene integrity: the story overlay exists in MainMenu, its presenter
    // references are bound, the MainMenuController points at it, and it is
    // hidden by default. Inspected via component type-name reflection (the test
    // asmdef cannot reference Assembly-CSharp), matching the other scene tests.
    // The show/advance/persist behaviour is covered by StoryCardTests (queue +
    // seen-store); this guards the wiring the scaffold produced.
    public class StoryOverlaySceneTests
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

        [Test]
        public void StoryPresenter_Exists_WithReferencesBound()
        {
            var scene = Open();
            try
            {
                var presenter = FindByTypeName(scene, "StoryCardPresenter");
                Assert.IsNotNull(presenter,
                    "MainMenu has no StoryCardPresenter. Run "
                    + "'Jebby Jump/Scaffold/Build Story Overlay'.");

                var so = new UnityEditor.SerializedObject(presenter);
                foreach (var field in new[]
                    { "_root", "_headline", "_body", "_continueButton", "_skipButton" })
                    Assert.IsNotNull(so.FindProperty(field).objectReferenceValue,
                        "StoryCardPresenter." + field + " unwired");
            }
            finally { EditorSceneManager.CloseScene(scene, true); }
        }

        [Test]
        public void MainMenu_PointsAtTheStoryOverlay()
        {
            var scene = Open();
            try
            {
                var menu = FindByTypeName(scene, "MainMenuController");
                Assert.IsNotNull(menu, "MainMenuController missing");
                var so = new UnityEditor.SerializedObject(menu);
                Assert.IsNotNull(so.FindProperty("_storyOverlay").objectReferenceValue,
                    "MainMenuController._storyOverlay unwired");
            }
            finally { EditorSceneManager.CloseScene(scene, true); }
        }

        [Test]
        public void StoryOverlayRoot_IsHiddenByDefault()
        {
            var scene = Open();
            try
            {
                var presenter = FindByTypeName(scene, "StoryCardPresenter");
                var so = new UnityEditor.SerializedObject(presenter);
                var root = so.FindProperty("_root").objectReferenceValue as GameObject;
                Assert.IsNotNull(root, "_root is not a GameObject");
                Assert.IsFalse(root.activeSelf,
                    "story overlay must be hidden until the menu shows a card");
            }
            finally { EditorSceneManager.CloseScene(scene, true); }
        }
    }
}
