using JebbyJump.Flow;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _quitButton;

        private void Awake()
        {
            if (_startButton != null) _startButton.onClick.AddListener(SceneLoader.LoadGame);
            if (_quitButton != null)  _quitButton.onClick.AddListener(SceneLoader.QuitGame);
        }

        private void OnDestroy()
        {
            if (_startButton != null) _startButton.onClick.RemoveListener(SceneLoader.LoadGame);
            if (_quitButton != null)  _quitButton.onClick.RemoveListener(SceneLoader.QuitGame);
        }
    }
}
