using JebbyJump.Flow;
using JebbyJump.Progression;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // Main Menu wiring. Start now opens the Level Select panel so the
    // player can pick which unlocked level to play. If the Level Select
    // is not yet wired (mid-scaffold builds) we fall back to loading the
    // Game scene directly so the build stays testable in the gap.
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private LevelSelectController _levelSelect;

        private void Awake()
        {
            if (_startButton != null)
                _startButton.onClick.AddListener(OnStartClicked);
            if (_quitButton != null)
                _quitButton.onClick.AddListener(SceneLoader.QuitGame);

            // Clear any stale selection so a fresh Main Menu always
            // starts from a clean handoff slot.
            PendingLevelSelection.Reset();
        }

        private void OnDestroy()
        {
            if (_startButton != null)
                _startButton.onClick.RemoveListener(OnStartClicked);
            if (_quitButton != null)
                _quitButton.onClick.RemoveListener(SceneLoader.QuitGame);
        }

        private void OnStartClicked()
        {
            if (_levelSelect == null)
            {
                Debug.LogWarning(
                    "[MainMenu] No LevelSelectController assigned. "
                    + "Loading Game with Level 1.");
                SceneLoader.LoadGame();
                return;
            }
            _levelSelect.Open();
        }
    }
}
