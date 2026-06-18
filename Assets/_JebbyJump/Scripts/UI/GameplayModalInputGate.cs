using JebbyJump.Inputs;
using JebbyJump.Session;
using JebbyJump.Shell;
using UnityEngine;

namespace JebbyJump.UI
{
    // P22: blocks + clears the gameplay input layer whenever a shell modal is
    // active (pause, pause->settings, level result, game over). Lives on an
    // always-active gameplay root (HUDCanvas) - NOT the MobileControlsCanvas,
    // which MobileControlsToggle deactivates on desktop - so keyboard/gamepad
    // blocking still runs there. It does BOTH:
    //   (a) disables the on-screen control CanvasGroup (pointer / raycast), and
    //   (b) tells the InputReader to ignore AND clear gameplay input (keyboard,
    //       gamepad, and on-screen touch all route through it),
    // while leaving the Pause action and the UI input module / navigation
    // active. It keys off the EXPLICIT active modal panels (not the
    // GameShellCanvas object) plus PauseState. Does not touch CanvasGroup alpha
    // (control opacity is preserved).
    public class GameplayModalInputGate : MonoBehaviour
    {
        [SerializeField] private InputReader _input;
        [SerializeField] private CanvasGroup _controlsGroup;
        [SerializeField] private GameObject _settingsPanel; // optional (pause->settings)
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private GameObject _gameOverPanel;

        private bool _blocked;
        private bool _initialized;

        private void OnEnable()
        {
            _initialized = false; // force a re-apply when (re)enabled
            Tick();
        }

        private void OnDisable()
        {
            // Restore the shared InputReader so it is never left blocked.
            if (_input != null) _input.SetGameplayInputEnabled(true);
        }

        private void Update() => Tick();

        private void Tick()
        {
            bool blocked = Evaluate();
            if (_initialized && blocked == _blocked) return;
            _blocked = blocked;
            _initialized = true;
            Apply(blocked);
        }

        private bool Evaluate()
        {
            bool settings = _settingsPanel != null && _settingsPanel.activeInHierarchy;
            bool result = _resultPanel != null && _resultPanel.activeInHierarchy;
            bool gameOver = _gameOverPanel != null && _gameOverPanel.activeInHierarchy;
            return GameplayInputBlockPolicy.ShouldBlock(
                PauseState.IsPaused, settings, result, gameOver);
        }

        private void Apply(bool blocked)
        {
            if (_controlsGroup != null)
            {
                _controlsGroup.interactable = !blocked;
                _controlsGroup.blocksRaycasts = !blocked;
            }
            if (_input != null) _input.SetGameplayInputEnabled(!blocked);
        }
    }
}
