using JebbyJump.Analytics;
using JebbyJump.Flow;
using JebbyJump.Inputs;
using JebbyJump.Level;
using JebbyJump.Sequence;
using JebbyJump.Session;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // In-game pause flow. Pause uses Time.timeScale = 0 (which freezes the
    // timer, skill cooldowns, effect durations, physics, and the memory
    // coroutine) plus PauseState (which the input handlers read to ignore
    // jump/skill/move while paused). Pause is disabled while the result or
    // game-over panel is open and re-enabled when a new attempt begins.
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private Button _pauseButton;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private MemoryPhaseController _phaseController;
        [SerializeField] private LevelProgressTracker _progressTracker;
        [SerializeField] private InputReader _input;
        [SerializeField] private SettingsPanelController _settingsPanel;

        private bool _canPause = true;
        private bool _settingsOpen;
        public bool IsPaused { get; private set; }

        private void Awake()
        {
            // A scene must never start frozen, even if a prior scene left
            // timeScale at 0 due to an abnormal exit.
            Time.timeScale = 1f;
            PauseState.SetPaused(false);

            if (_pausePanel != null) _pausePanel.SetActive(false);

            if (_pauseButton != null)
                _pauseButton.onClick.AddListener(Pause);
            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(Resume);
            if (_restartButton != null)
                _restartButton.onClick.AddListener(RestartLevel);
            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OnSettingsClicked);
            if (_mainMenuButton != null)
                _mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }

        private void OnEnable()
        {
            if (_phaseController != null)
            {
                _phaseController.MemoryPhaseStarted += OnRunActive;
                _phaseController.LevelCompleted += OnRunEnded;
            }
            if (_progressTracker != null)
                _progressTracker.GameOver += OnRunEnded;
            if (_input != null)
                _input.PauseEvent += TogglePause;
        }

        private void OnDisable()
        {
            if (_phaseController != null)
            {
                _phaseController.MemoryPhaseStarted -= OnRunActive;
                _phaseController.LevelCompleted -= OnRunEnded;
            }
            if (_progressTracker != null)
                _progressTracker.GameOver -= OnRunEnded;
            if (_input != null)
                _input.PauseEvent -= TogglePause;
        }

        private void OnDestroy()
        {
            if (_pauseButton != null)
                _pauseButton.onClick.RemoveListener(Pause);
            if (_resumeButton != null)
                _resumeButton.onClick.RemoveListener(Resume);
            if (_restartButton != null)
                _restartButton.onClick.RemoveListener(RestartLevel);
            if (_settingsButton != null)
                _settingsButton.onClick.RemoveListener(OnSettingsClicked);
            if (_mainMenuButton != null)
                _mainMenuButton.onClick.RemoveListener(ReturnToMainMenu);

            // Never leave the engine frozen if this object is torn down.
            Time.timeScale = 1f;
            PauseState.SetPaused(false);
        }

        // A new attempt is starting (initial run, retry, or next level):
        // pausing is allowed again.
        private void OnRunActive()
        {
            _canPause = true;
            if (_pauseButton != null) _pauseButton.interactable = true;
        }

        // Result or game-over panel is up: block pausing, and if somehow
        // paused, resume so the panel is interactable.
        private void OnRunEnded()
        {
            _canPause = false;
            if (_pauseButton != null) _pauseButton.interactable = false;
            if (IsPaused) Resume();
        }

        private void TogglePause()
        {
            // While Settings is open from Pause, ignore the pause hotkey.
            // Toggling here would either unpause behind the open Settings
            // panel or hide it - both leave hidden/confusing state. Back
            // is the only intended exit from Settings.
            if (_settingsOpen) return;
            if (IsPaused) Resume();
            else Pause();
        }

        public void Pause()
        {
            if (IsPaused || !_canPause) return;
            IsPaused = true;
            PauseState.SetPaused(true);
            Time.timeScale = 0f;
            if (_pausePanel != null) _pausePanel.SetActive(true);
            TrackPause(AnalyticsEvents.PauseOpened);
        }

        public void Resume()
        {
            if (!IsPaused) return;
            IsPaused = false;
            _settingsOpen = false;
            PauseState.SetPaused(false);
            Time.timeScale = 1f;
            if (_pausePanel != null) _pausePanel.SetActive(false);
            TrackPause(AnalyticsEvents.PauseResumed);
        }

        public void RestartLevel()
        {
            TrackPause(AnalyticsEvents.PauseRestartClicked);
            // Restore time first so the memory-phase coroutine can run.
            UnpauseTimeOnly();
            if (_pausePanel != null) _pausePanel.SetActive(false);
            _phaseController?.RestartLevel();
        }

        public void ReturnToMainMenu()
        {
            TrackPause(AnalyticsEvents.PauseMainMenuClicked);
            // PendingLevelSelection is reset by MainMenuController.Awake in
            // the next scene, so no stale selection is replayed.
            UnpauseTimeOnly();
            SceneLoader.LoadMainMenu();
        }

        // Open Settings while keeping the game paused. Hide the Pause
        // panel for the duration; restore it on Back via the close
        // callback. Does NOT touch Time.timeScale or PauseState - the
        // pause invariants must hold across the Settings round-trip.
        private void OnSettingsClicked()
        {
            if (_settingsPanel == null)
            {
                Debug.LogWarning(
                    "[Pause] No SettingsPanelController assigned; "
                    + "Settings click ignored.");
                return;
            }
            if (_settingsOpen) return;
            _settingsOpen = true;
            TrackPause(AnalyticsEvents.PauseSettingsOpened);
            if (_pausePanel != null) _pausePanel.SetActive(false);
            _settingsPanel.Open(() =>
            {
                _settingsOpen = false;
                // Only restore the Pause panel if still paused; a Resume/
                // Restart/Main Menu that ran meanwhile must not resurrect it.
                if (IsPaused && _pausePanel != null)
                    _pausePanel.SetActive(true);
            });
        }

        private void UnpauseTimeOnly()
        {
            IsPaused = false;
            _settingsOpen = false;
            PauseState.SetPaused(false);
            Time.timeScale = 1f;
        }

        // Pause-flow events all carry the current level for context, read
        // from the session-scoped LevelContext mirror.
        private static void TrackPause(string eventName)
        {
            AnalyticsService.Track(eventName,
                AnalyticsParam.Of(AnalyticsParams.LevelIndex, LevelContext.CurrentIndex),
                AnalyticsParam.Of(AnalyticsParams.LevelNumber, LevelContext.CurrentNumber));
        }
    }
}
