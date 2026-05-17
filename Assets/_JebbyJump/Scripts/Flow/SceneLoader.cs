using JebbyJump.Core;
using UnityEngine.SceneManagement;

namespace JebbyJump.Flow
{
    public static class SceneLoader
    {
        public static void LoadMainMenu() => SceneManager.LoadScene(SceneNames.MainMenu);
        public static void LoadGame()     => SceneManager.LoadScene(SceneNames.Game);

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
