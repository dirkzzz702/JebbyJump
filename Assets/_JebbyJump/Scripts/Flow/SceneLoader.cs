using JebbyJump.Core;
using Unity.Profiling;
using UnityEngine.SceneManagement;

namespace JebbyJump.Flow
{
    public static class SceneLoader
    {
        // P24 instrumentation (negligible overhead; ships).
        private static readonly ProfilerMarker s_LoadMainMenu = new ProfilerMarker("JebbyJump.Scene.LoadMainMenu");
        private static readonly ProfilerMarker s_LoadGame = new ProfilerMarker("JebbyJump.Scene.LoadGame");

        public static void LoadMainMenu()
        {
            using var _ = s_LoadMainMenu.Auto();
            SceneManager.LoadScene(SceneNames.MainMenu);
        }

        public static void LoadGame()
        {
            using var _ = s_LoadGame.Auto();
            SceneManager.LoadScene(SceneNames.Game);
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}
