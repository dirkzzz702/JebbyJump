using JebbyJump.Analytics;
using JebbyJump.Flow;
using JebbyJump.Progression;
using JebbyJump.Rewards;
using JebbyJump.Wardrobe;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // Main Menu wiring. Continue jumps straight to the highest unlocked
    // level; Level Select (the former Start button) opens the picker; Quit
    // exits. If Level Select is not wired (mid-scaffold builds) Start falls
    // back to loading the Game scene directly so the build stays testable.
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _wardrobeButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private LevelSelectController _levelSelect;
        [SerializeField] private SettingsPanelController _settingsPanel;
        [SerializeField] private WardrobePanelController _wardrobePanel;
        [SerializeField] private LevelCatalog _catalog;

        private void Awake()
        {
            // Harden wardrobe save state once at menu entry - BEFORE Continue
            // or any gameplay scene load can apply the equipped outfit. This
            // also normalizes a now-locked/invalid equipped id to default
            // (ongoing normalization, independent of schema version).
            WardrobePersistenceMigrator.MigrateIfNeeded(
                _catalog != null ? StarRewardStore.GetTotalStars(_catalog.Count) : 0);

            if (_continueButton != null)
                _continueButton.onClick.AddListener(OnContinueClicked);
            if (_startButton != null)
                _startButton.onClick.AddListener(OnStartClicked);
            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OnSettingsClicked);
            if (_wardrobeButton != null)
                _wardrobeButton.onClick.AddListener(OnWardrobeClicked);
            if (_quitButton != null)
                _quitButton.onClick.AddListener(SceneLoader.QuitGame);

            // Continue needs a catalog to know the valid level range.
            // With no catalog, disable it rather than guessing Level 1.
            if (_continueButton != null
                && (_catalog == null || _catalog.Count == 0))
            {
                _continueButton.interactable = false;
                Debug.LogWarning(
                    "[MainMenu] Continue disabled: LevelCatalog missing or "
                    + "empty. Assign a populated LevelCatalog to enable it.");
            }

            // Clear any stale selection so a fresh Main Menu always
            // starts from a clean handoff slot.
            PendingLevelSelection.Reset();
        }

        private void OnDestroy()
        {
            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(OnContinueClicked);
            if (_startButton != null)
                _startButton.onClick.RemoveListener(OnStartClicked);
            if (_settingsButton != null)
                _settingsButton.onClick.RemoveListener(OnSettingsClicked);
            if (_wardrobeButton != null)
                _wardrobeButton.onClick.RemoveListener(OnWardrobeClicked);
            if (_quitButton != null)
                _quitButton.onClick.RemoveListener(SceneLoader.QuitGame);
        }

        private void OnContinueClicked()
        {
            if (_catalog == null || _catalog.Count == 0)
            {
                // Should not happen (button is disabled above), but guard
                // anyway so Continue never silently launches Level 1.
                Debug.LogWarning(
                    "[MainMenu] Continue ignored: no valid LevelCatalog.");
                return;
            }

            int target = LevelProgressStore.GetContinueIndex(_catalog.Count);
            PendingLevelSelection.Index = target;
            PendingLevelSelection.Source = "continue";
            AnalyticsService.Track(AnalyticsEvents.MainMenuContinueClicked,
                AnalyticsParam.Of(AnalyticsParams.TargetLevelIndex, target),
                AnalyticsParam.Of(AnalyticsParams.TargetLevelNumber, target + 1));
            SceneLoader.LoadGame();
        }

        private void OnStartClicked()
        {
            AnalyticsService.Track(AnalyticsEvents.MainMenuLevelSelectClicked);
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

        private void OnSettingsClicked()
        {
            AnalyticsService.Track(AnalyticsEvents.MainMenuSettingsOpened);
            if (_settingsPanel == null)
            {
                Debug.LogWarning(
                    "[MainMenu] No SettingsPanelController assigned; "
                    + "Settings click ignored.");
                return;
            }
            _settingsPanel.Open();
        }

        private void OnWardrobeClicked()
        {
            // wardrobe_opened analytics is emitted by the panel's Open() so
            // it fires regardless of entry point; no emit here to avoid a
            // duplicate.
            if (_wardrobePanel == null)
            {
                Debug.LogWarning(
                    "[MainMenu] No WardrobePanelController assigned; "
                    + "Wardrobe click ignored.");
                return;
            }
            _wardrobePanel.Open();
        }
    }
}
